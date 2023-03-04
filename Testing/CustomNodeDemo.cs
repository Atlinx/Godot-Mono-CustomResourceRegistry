using Godot;
using System;
using MonoCustomResourceRegistry;

[RegisteredType(nameof(CustomNodeDemo), "res://Testing/icon.png", nameof(Node2D))]
public partial class CustomNodeDemo : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
