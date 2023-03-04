using Godot;
using MonoCustomResourceRegistry;

[RegisteredType(nameof(AlphaResource), "")]
public partial class AlphaResource : Resource
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AlphaResource() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
        return $"Alpha: [ {IntegerField} | {TextField} | {BetaSubResource} ]";
    }
}