# Godot Mono Custom Resource Registry Plugin

This is a Godot C# plugin that registers custom C# resources and custom nodes for Godot. This plugin serves as a workaround for the Godot engine's [C# resource missing in context menu issue](https://github.com/godotengine/godot/issues/27470) and [C# resource saving issue](https://github.com/godotengine/godot/issues/38191).

Based off of *CustomResourceRegisterPlugin* made by [wmigor](https://github.com/wmigor/godot-mono-custom-resource-register)

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
1. Create a C# class in a new file that implements `IRegisteredResource`. The file that stores this class must have the same name as this class. Make sure to add `using MonoCustomResourceRegistry;` to the top of your file to import this plugin's namespace which contains `IRegisteredResource`.
2. Make sure your C# file is under one of the [**Resource Script Directories**](#settings)
3. Rebuild the solution
4. Open the "CRR" tab and press the **Refresh** button to update the registered custom resources

Sample C# node script
```C#
using MonoCustomResourceRegistry;

public class CustomNodeDemo : IRegisteredResource
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

## Settings

This plugin comes with some settings to configure how C# resources are loaded.
The settings can be accessed by going to **Project > ProjectSettings > General > Mono Custom Resource Registry**.

All settings are listed below:

**Class Prefix** - The prefix that is seen before all custom nodes and resources.

**Resource Script Directories** - The paths to the directories where you want to scan for C# resource scripts to register as custom resources. By default, it only contains "res://". 

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
