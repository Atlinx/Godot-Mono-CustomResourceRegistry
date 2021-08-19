# Godot Mono Custom Resource Registry Plugin

This is a Godot C# plugin that registers custom C# resources and custom nodes for Godot. This plugin serves as a workaround for the Godot engine's [C# resource missing in context menu issue](https://github.com/godotengine/godot/issues/27470) and [C# resource saving issue](https://github.com/godotengine/godot/issues/38191).

Based off of *CustomResourceRegisterPlugin* made by [wmigor](https://github.com/wmigor/godot-mono-custom-resource-register)

Reflection saving and loading originally written by [rob-mur](https://github.com/rob-mur)

Modified by [Atlinx](https://github.com/Atlinx)

## UPDATE:
There has been a workaround found for resource saving, by cgbeutler, which makes the saving features of this plugin irrelevant.
[Here is the link to their post](https://gist.github.com/cgbeutler/c4f00b98d744ac438b84e8840bbe1740).

## Installation

1. Head over to the [Releases](https://github.com/Atlinx/Godot-Mono-CustomResourceRegistry/releases/latest) page of this repository.
2. Download the "CustomResourceRegistry_vXX.XX.XX" zip file of a release, unzip it, and move the "CustomResourceRegistry" that's inside under the "res://addons" directory in your Godot project.
3. Press the **Build** button on the top right of Godot editor to rebuild the solution.
4. Go to **Project Settings > Plugins** and press the **Enable** checkbox next to the CustomResourceRegistry plugin to enable the plugin. This will create a tab called "CRR" on the bottom of the editor.

## How to Use 

### Adding/Removing Custom C# Resouces/Nodes

To add a custom C# resource:
1. Create a C# class in a new file that extends `Godot.Resource`. This class must have a parameterless constructor. The file that stores this class must have the same name as this class.
2. Make sure your C# file is under one of the [**Resource Script Directories**](#settings)
3. Rebuild the solution
4. Open the "CRR" tab and press the **Refresh** button to update the registered custom resources

To add a custom C# node:
1. Create a C# class in a new file that implements `ICustomNode`. The file that stores this class must have the same name as this class. Make sure to add `using MonoCustomResourceRegistry;` to the top of your file to import this plugin's namespace which contains `ICustomNode`.
2. Make sure your C# file is under one of the [**Resource Script Directories**](#settings)
3. Rebuild the solution
4. Open the "CRR" tab and press the **Refresh** button to update the registered custom resources

Sample C# node script
```C#
using MonoCustomResourceRegistry;

public class CustomNodeDemo : ICustomNode
{
  public override void _Ready()
  {
    
  }
}
```

To delete custom C# resources/nodes:
1. Delete the C# resource/node script
2. Rebuild the solution
3. Open the "CRR" tab and press the **Refresh** button to update the registered custom resources

Anytime the Plugin registers/unregisters a resource/node, the plugin will print its actions into the **Output** window.

### Structure of Custom C# Resources

If you want information to be serialized (loaded/saved), it must be stored in a property with the `[Export]` attribute.

For example:

```C#
public class CustomResource : Resouce
{
  // Serialized information (Information that will be loaded and saved)
  [Export]
  private int SerializeMe { get; private set; }
  [Export]
  public float SerializeMeToo { get; set; }
  
  // Not serialized
  public string DontSerializeMe { get; set; }
  private string dontSerializeMeToo;
  [Export]
  public bool alsoNotSerialized;
}
```

Note there currently is no support for serializing fields with the `[Export]` attribute. Contributions for implementing this feature are welcomed.

### Saving/Loading Custom C# Resources at Runtime
## DEPRECATED

You can use the `MonoCustomResourceIO` class to save and load custom Resources using your script. This class features 3 static methods, which are
- `Load<T>()` - Loads a custom resource of type `T`.
- `Save(string path, Resource resource)` - If the `path` already has a resource, it will run `SaveExisting()`. Otherwise, it will run `SaveNew()`.
- `SaveNew(string path, Resource resource)` - Saves a new `resource` at a given `path`.
- `SaveExisting(string path, Resource resource)` - Saves a `resource` into an existing resoruce at a given `path`.

Since the Godot Engine's ResourceSaver.Save() functionality is currently broken (as of v3.2.3), this plugin uses an alternative method of saving. This method involves setting up empty resource files that will serve as "prototypes" or "templates" for the plugin to build a new custom resource class off of.

#### Prototype Resources
Therefore after adding your custom C# resource to the Plugin's registry you must create a new resouce of that type using **RMB > New Resource** in the **FileSystem** window and this resource must be named `CustomResourceClassName\_ResourcePrototype`, where `CustomResourceClassName` should be replaced with the exact name of your class (excluding namespaces). This resource will serve as a prototype resource for the plugin to build new resources of the same type off of. 

This prototype resource must also be placed underneath a directory that is a part of **Resource Prototype Directories** in order to let the plugin scan and obtain this prototype. 

For each exported variable in a prototype resource that is a custom resource type, make sure to assign a new instance of that type to the variable and make sure this new instance is stored witin the prototype resource. This creation of a built-in instance can be done by opening the prototype resource file, going to the **Inspector** window, clicking on the `(empty)` slot next to the custom resource type variable, and finally clicking `New YourCustomResourceName`.

### Limitations to Saving/Loading

Since this Plugin builds saves off of templates you **CANNOT** save custom resouce types that contain a collection of custom reouce types (Such as arrays like `CustomResource[]`, or collections like `List<CustomResource>`).

However you still **CAN** load custom resource types that contain collections of custom resource types.

## Settings

This plugin comes with some settings to configure how C# resources are loaded.
The settings can be accessed by going to **Project > ProjectSettings > General > Custom Resource Registry**.

All settings are listed below:

**Class Prefix** - The prefix that is seen before all custom nodes and resources.

**Resource Script Directories** - The paths to the directories where you want to scan for C# resource scripts to register as custom resources. By default, it only contains "res://". 

**Resource Prototype Directories** - The paths to the directories where you want to scan for the prototype resource files, which serve as templates for the Plugin to build new resources off of.

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
