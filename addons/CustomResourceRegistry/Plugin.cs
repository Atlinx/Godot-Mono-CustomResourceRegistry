using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

// Originally written by wmigor
// Edited by Atlinx to recursively search for files.
// wmigor's Public Repo: https://github.com/wmigor/godot-mono-custom-resource-register
namespace MonoCustomResourceRegistry
{
	public interface ICustomNodeType {}

	[Tool]
	public class Plugin : EditorPlugin
	{
		private readonly List<string> customTypes = new List<string>();
		private Control control;

		public override void _EnterTree()
		{
			UnregisterCustomClasses();
			Settings.Init();
			RegisterCustomClasses();
			control = CreateBottomMenuControl();
			AddControlToBottomPanel(control, "CRR");
		}

		public override void _ExitTree()
		{
			UnregisterCustomClasses();
			RemoveControlFromBottomPanel(control);
			control = null;
		}

		private void RegisterCustomClasses()
		{
			customTypes.Clear();

			var file = new File();

			foreach (var type in GetCustomResourceTypes())
			{
				var path = FindClassPath(type);
				if (path == null)
					continue;
				var script = GD.Load<Script>(path);
				if (script == null)
					continue;
				AddCustomType($"{Settings.ClassPrefix}{type.Name}", nameof(Resource), script, null);
				GD.Print($"Register custom resource: {type.Name} -> {path}");
				customTypes.Add($"{Settings.ClassPrefix}{type.Name}");
			}

			foreach (var type in GetCustomNodes())
			{
				var path = FindClassPath(type);
				if (path == null)
					continue;
				if (!file.FileExists(path))
					continue;
				var script = GD.Load<Script>(path);
				if (script == null)
					continue;
				AddCustomType($"{Settings.ClassPrefix}{type.Name}", nameof(Node), script, null);
				GD.Print($"Register custom node: {type.Name} -> {path}");
				customTypes.Add($"{Settings.ClassPrefix}{type.Name}");
			}
		}

		private static string FindClassPath(Type type)
		{
			switch (Settings.SearchType)
			{
				case Settings.ResourceSearchType.Recursive:
					return FindClassPathRecursive(type);
				case Settings.ResourceSearchType.Namespace:
					return FindClassPathNamespace(type);
				default:
					throw new Exception($"ResourceSearchType {Settings.SearchType} not implemented!");
			}
		}

		private static string FindClassPathNamespace(Type type)
		{
			foreach (string dir in Settings.ResourceScriptDirectories)
			{
				string filePath = $"{dir}/{type.Namespace?.Replace(".", "/") ?? ""}/{type.Name}.cs";
				File file = new File();
				if (file.FileExists(filePath))
					return filePath;
			}
			return null;
		}

		private static string FindClassPathRecursive(Type type)
		{
			foreach (string directory in Settings.ResourceScriptDirectories)
			{
				string fileFound = FindClassPathRecursiveHelper(type, directory);
				if (fileFound != null)
					return fileFound;
			}
			return null;
		}

		private static string FindClassPathRecursiveHelper(Type type, string directory)
		{
			Directory dir = new Directory();

			if (dir.Open(directory) == Error.Ok)
			{
				dir.ListDirBegin();

				while (true)
				{
					var fileOrDirName = dir.GetNext();
					
					// Skips hidden files like .
					if (fileOrDirName == "")
						break;
					else if (fileOrDirName.BeginsWith("."))
						continue;
					else if (dir.CurrentIsDir())
					{
						string foundFilePath = FindClassPathRecursiveHelper(type, dir.GetCurrentDir() + "/" + fileOrDirName);
						if (foundFilePath != null)
						{
							dir.ListDirEnd();
							return foundFilePath;
						}
					}
					else if (fileOrDirName == $"{type.Name}.cs")
						return dir.GetCurrentDir() + "/" + fileOrDirName;
				}
			}
			return null;
		}

		private static IEnumerable<Type> GetCustomResourceTypes()
		{
			var assembly = Assembly.GetAssembly(typeof(Plugin));
			return assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Resource)));
		}

		private static IEnumerable<Type> GetCustomNodes()
		{
			var assembly = Assembly.GetAssembly(typeof(Plugin));
			return assembly.GetTypes().Where(t =>
				!t.IsAbstract && typeof(ICustomNodeType).IsAssignableFrom(t) && t.IsSubclassOf(typeof(Node)));
		}

		private void UnregisterCustomClasses()
		{
			foreach (var script in customTypes)
			{
				RemoveCustomType(script);
				GD.Print($"Unregister custom resource: {script}");
			}

			customTypes.Clear();
		}

		private Control CreateBottomMenuControl()
		{
			var container = new Container();
			container.RectMinSize = new Vector2(0, 50);
			
			var button = new Button {Text = "Refresh"};
			button.Connect("pressed", this, nameof(OnRefreshPressed));
			container.AddChild(button);
			button.SetAnchorsAndMarginsPreset(Control.LayoutPreset.Wide);

			return container;
		}

		private void OnRefreshPressed()
		{
			UnregisterCustomClasses();
			RegisterCustomClasses();
		}
	}
}