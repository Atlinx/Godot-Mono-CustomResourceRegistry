using Godot;
using System;

public partial class GammaResource : Resource
{
    public GammaResource() {}
    
    public GammaResource(BetaResource[] betaArray, string textField)
    {
        BetaArray = betaArray;
        TextField = textField;
    }

    [Export]
    public BetaResource[] BetaArray { get; set; } = new BetaResource[0];
    [Export]
    public string TextField { get; set; } = "";

    public override string ToString()
    {
        return $"Gamma: [ {String.Join<BetaResource>(", ", BetaArray)} | {TextField} ]";
    }
}