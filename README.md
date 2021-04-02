# Godot Mono Custom Resource Registry Plugin

This is a Godot C# plugin that registers custom C# resources and custom nodes for Godot. This plugin serves as a workaround for this Godot engine [issue](https://github.com/godotengine/godot/issues/27470).

Based off of *CustomResourceRegisterPlugin* made by [wmigor](https://github.com/wmigor/godot-mono-custom-resource-register)

Reflection saving and loading originally written by [rob-mur](https://github.com/rob-mur)

Modified by [Atlinx](https://github.com/Atlinx)

## Installation

1. Download the "CustomResourceRegistry" folder and place it under the "res://addons" directory in your Godot project.
2. Press the **Build** button on the top right of Godot editor to rebuild the solution.
3. Go to **Project Settings > Plugins** and press the **Enable** checkbox next to the CustomResourceRegister plugin to enable the plugin. This will create a tab called "CRR" on the bottom of the editor.

## How to Use 

### Adding/Removing Custom C# Resouces/Nodes

To add a custom C# resource:
1. Create a C# class in a new file that extends `Godot.Resource`. The file that stores this class must have the same name as this class.
2. Rebuild the solution
3. Open the "CRR" tab and press the **Refresh** button to update the registered custom resources

To add a custom C# node:
1. Create a C# class in a new file that implements `ICustomNode`. The file that stores this class must have the same name as this class.
2. Rebuild the solution
3. Open the "CRR" tab and press the **Refresh** button to update the registered custom resources

To delete custom C# resources/nodes:
1. Delete the C# resource/node script
2. Rebuild the solution
3. Open the "CRR" tab and press the **Refresh** button to update the registered custom resources

Anytime the Plugin registers/unregisters a resource/node, the plugin will print its actions into the **Output** window.

### Saving/Loading Custom C# Resources at Runtime

You can use the `MonoCustomResourceIO` class to save and load custom Resources using your script. This class features 3 static methods, which are
- `Load<T>()` - Loads a custom resource of type `T`.
- `Save(string path, Resource resource)` - If the `path` already has a resource, it will run `SaveExisting()`. Otherwise, it will run `SaveNew()`.
- `SaveNew(string path, Resource resource)` - Saves a new `resource` at a given `path`.
- `SaveExisting(string path, Resource resource)` - Saves a `resource` into an existing resoruce at a given `path`.

Since the Godot Engine's ResourceSaver.Save() functionality is currently broken (as of v3.2.3), this plugin uses an alternative method of saving. This method involves setting up empty resource files that will serve as "prototypes" or "templates" for the plugin to build a new custom resource class off of.

Therefore after adding your custom C# resource to the Plugin's registry you must create a new resouce of that type using **RMB > New Resource** in the **FileSystem** window and this resource must be named `CustomResourceClassName\_ResourcePrototype`, where `CustomResourceClassName` should be replaced with the exact name of your class (excluding namespaces). This resource must also be placed underneath a directory that is a part of **Resource Prototype Directories** in order to let the plugin scan and obtain this template.

### Limitations to Saving/Loading

Since this Plugin builds saves off of templates you **CANNOT** save custom resouce types that contain a collection of custom reouce types (Such as arrays like `CustomResource[]`, or collections like `List<CustomResource>`).

## Settings

This plugin comes with some settings to configure how C# resources are loaded.
The settings can be accessed by going to **Project > ProjectSettings > General > Custom Resource Register**.

All settings are listed below:

**Class Prefix** - The prefix that is seen before all custom nodes and resources.

**Resource Script Directories** - The paths to the directories where you want to scan for C# resource scripts to register as custom resources. By default, it only contains "res://". 

**Resource Prototype Directories** - The paths to the directories where you want to scan for the "template" resource files.

**Search Type** - The method used to gather custom C# resource scripts.

- **Namespace** - Looks for scripts by using their namespace as a directory.
For example with the C# script below, the plugin will look under each resource script directory for the script by using the path "./Some/Long/Namespace/Something.cs"
```C#
namespace Some.Long.Namespace
{
  public class Something : Resource
  {
  }
}
```

- **Recursive** - Looks for scripts by searching through all "Resource Script Directories" and the directories within them. When using this method, you cannot have multiple resources that have the same class name.
