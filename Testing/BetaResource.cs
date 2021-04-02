using Godot;
using System;

public class BetaResource : Resource
{
    public BetaResource() {}
    public BetaResource(float floatField, string textField)
    {
        FloatField = floatField;
        TextField = textField;
    }

    [Export]
    public float[] FloatArrayField { get; set; } = new float[0];
    [Export]
    public float FloatField { get; set; } = 0;
    [Export]
    public string TextField { get; set; } = "";
    [Export]
    public string TextField2 { get; set; } = "";
}