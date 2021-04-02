using Godot;
using System;

public class Test : Node
{
    public override void _Ready()
    {
        /*var beta = new BetaResource(new float[]{0.4f, 0.5f}, 34.3f, "Beta Instance", "Field 2");
        MonoCustomResourceIO.SaveNew("res://Testing/BetaNew.tres", beta);

        var alpha = new AlphaResource(13, "Alpha Instance", beta);
        MonoCustomResourceIO.SaveNew("res://Testing/AlphaNew.tres", alpha);
        */

        var betaExisting = MonoCustomResourceIO.Load<BetaResource>("res://Testing/BetaNew.tres");
        GD.Print(betaExisting);

        var alphaExisting = MonoCustomResourceIO.Load<AlphaResource>("res://Testing/AlphaTest.tres");
        GD.Print(alphaExisting);
        alphaExisting.BetaSubResource.TextField2 = "Alpha Overwrite Beta!!";

        MonoCustomResourceIO.SaveExisting("res://Testing/AlphaTest.tres", alphaExisting);

        alphaExisting = MonoCustomResourceIO.Load<AlphaResource>("res://Testing/AlphaTest.tres");
        GD.Print($"Post methods: {alphaExisting}");
    }
}
