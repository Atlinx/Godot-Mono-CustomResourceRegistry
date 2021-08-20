using Godot;
using System;
using MonoCustomResourceRegistry;

[RegisteredType(nameof(AlphaResource), "")]
public class AlphaResource : Resource
{
    public AlphaResource() {}

    public AlphaResource(int integerField, string textField, BetaResource betaSubResource)
    {
        IntegerField = integerField;
        TextField = textField;
        BetaSubResource = betaSubResource;
    }

    [Export]
    public int IntegerField { get; set; } = 0;
    [Export]
    public string TextField { get; set; } = "";
    [Export]
    public BetaResource BetaSubResource { get; set; }

    public override string ToString()
    {
        return $"Alpha: [{IntegerField} {TextField} {BetaSubResource}]";
    }
}