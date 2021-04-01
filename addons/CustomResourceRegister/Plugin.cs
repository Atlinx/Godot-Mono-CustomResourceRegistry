using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

// Originally written by wmigor
// Edited by Atlinx to recursively search for files.
// wmigor's Public Repo: https://github.com/wmigor/godot-mono-custom-resource-register
namespace CustomResourceRegister
{
	public interface ICustomNode {}

	[Tool]
	public class Plugin : EditorPlugin
	{
		private readonly List<string> _scripts = new List<string>();
		private Control _control;

		public override void _EnterTree()
		{
			Settings.Init();
			RegisterCustomClasses();
			_control = CreateBottomMenuControl();
			AddControlToBottomPanel(_control, "CRR");
		}

		public override void _ExitTree()
		{
			UnregisterCustomClasses();
			RemoveControlFromBottomPanel(_control);
			_control = null;
		}

		private void RegisterCustomClasses()
		{
			_scripts.Clear();

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
				_scripts.Add(type.Name);
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
				_scripts.Add(type.Name);
			}
		}

		private static string FindClassPath(Type type)
		{
			switch (Settings.SearchType)
			{
				case Settings.ResourceSearchType.Recursive:
					return FindClassPathRecursive(type);
					break;
				case Settings.ResourceSearchType.Namespace:
					return FindClassPathNamespace(type);
					break;
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
				string fileFound = FindClassPathHelper(type, directory);
				if (fileFound != null)
					return fileFound;
			}
			return null;
		}

		private static string FindClassPathHelper(Type type, string directory)
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
						string foundFilePath = FindClassPathHelper(type, dir.GetCurrentDir() + "/" + fileOrDirName);
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
				!t.IsAbstract && typeof(ICustomNode).IsAssignableFrom(t) && t.IsSubclassOf(typeof(Node)));
		}

		private void UnregisterCustomClasses()
		{
			foreach (var script in _scripts)
			{
				RemoveCustomType(script);
				GD.Print($"Unregister custom resource: {script}");
			}

			_scripts.Clear();
		}

		private Control CreateBottomMenuControl()
		{
			var container = new Container();
			container.RectMinSize = new Vector2(0, 150);
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