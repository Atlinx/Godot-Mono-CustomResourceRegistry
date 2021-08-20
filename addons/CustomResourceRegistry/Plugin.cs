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
	public interface IRegisteredResource {}

	#if TOOLS
	[Tool]
	public class Plugin : EditorPlugin
	{
		// We're not going to hijack the Mono Build button since it actually takes time to build
		// and we can't be sure how long that is. I guess we have to leave refreshing to the user for now.
		// There isn't any automation we can do to fix that.
		// private Button MonoBuildButton => GetNode<Button>("/root/EditorNode/@@580/@@581/@@589/@@590/ToolButton");
		private readonly List<string> customTypes = new List<string>();
		private Button refreshButton;

		public override void _EnterTree()
		{
			refreshButton = new Button();
			refreshButton.Text = "CCR";
			
			AddControlToContainer(CustomControlContainer.Toolbar, refreshButton);
			refreshButton.Icon = refreshButton.GetIcon("Reload", "EditorIcons");
			refreshButton.Connect("pressed", this, nameof(OnRefreshPressed));

			Settings.Init();
			RefreshCustomClasses();
			GD.PushWarning("You may change any setting for MonoCustomResourceRegistry in Project -> ProjectSettings -> General -> MonoCustomResourceRegistry");
		}

		public override void _ExitTree()
		{
			UnregisterCustomClasses();
			RemoveControlFromContainer(CustomControlContainer.Toolbar, refreshButton);
			refreshButton.QueueFree();
		}

		public void RefreshCustomClasses()
		{
			GD.Print("\nRefreshing Registered Resources...");
			UnregisterCustomClasses();
			RegisterCustomClasses();
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
				!t.IsAbstract && typeof(IRegisteredResource).IsAssignableFrom(t) && t.IsSubclassOf(typeof(Node)));
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

		private void OnRefreshPressed()
		{
			RefreshCustomClasses();
		}
	}
	#endif
}