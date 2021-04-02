# Godot Mono Custom Resource Registry Plugin

This is a Godot C# plugin that registers custom C# resources and custom nodes for Godot. This plugin works as a workaround for this Godot engine [issue](https://github.com/godotengine/godot/issues/27470).

Based off of *CustomResourceRegisterPlugin* made by [wmigor](https://github.com/wmigor/godot-mono-custom-resource-register)

Reflection saving and loading originally written by [rob-mur](https://github.com/rob-mur)

Modified by [Atlinx](https://github.com/Atlinx)

## Installation

1. Download the "CustomResourceRegister" folder and place it under the "res://addons" directory in your Godot project.
2. Press the **Build** button on the top right of Godot editor to rebuild the solution.
3. Go to **Project Settings > Plugins** and press the **Enable** checkbox next to the CustomResourceRegister plugin to enable the plugin. This will create a tab called "CRR" on the bottom of the editor.

## How to Use 

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

## Settings

This plugin comes with some settings to configure how C# resources are loaded.
The settings can be accessed by going to **Project > ProjectSettings > General > Custom Resource Register**.

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
