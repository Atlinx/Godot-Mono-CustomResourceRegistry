using Godot;
using System;

public class Test : Node
{
    public override void _Ready()
    {
        // Weird Godot loading behaviour
        // Sub resources can only load once parent resouces load.
        // That is, betaSub can only load once alphaNew, which contains betaSub, has been loaded.
        /*
        var alphaNew = MonoCustomResourceIO.Load<AlphaResource>("res://Testing/AlphaNew.tres");
        GD.Print(alphaNew);

        var betaSub = ResourceLoader.Load("res://Testing/AlphaNew.tres::1");
        GD.Print(betaSub);
        */
        
        /*
        // Turns out Godot has native support for loading custom c# resources! Yayy!
        
        var gdLoadedBeta = ResourceLoader.Load<BetaResource>("res://Testing/BetaTest.tres");
        GD.Print(gdLoadedBeta);

        var gdLoadedGamma = ResourceLoader.Load<GammaResource>("res://Testing/GammaTest.tres");
        GD.Print(gdLoadedGamma);
        */

        //var gammaExisting = MonoCustomResourceIO.Load<GammaResource>("res://Testing/GammaTest.tres");
        //GD.Print(gammaExisting);
        
        var dir = new Directory();
        dir.Remove("res://Testing/BetaNew.tres");
        dir.Remove("res://Testing/AlphaNew.tres");

        var beta = new BetaResource(new float[]{0.4f, 0.5f}, 34.3f, "Beta Instance", "Field 2");
        MonoCustomResourceIO.SaveNew("res://Testing/BetaNew.tres", beta);

        var alpha = new AlphaResource(13, "Alpha Instance", beta);
        MonoCustomResourceIO.SaveNew("res://Testing/AlphaNew.tres", alpha);
        

        var betaExisting = MonoCustomResourceIO.Load<BetaResource>("res://Testing/BetaNew.tres");
        GD.Print(betaExisting);

        var alphaExisting = MonoCustomResourceIO.Load<AlphaResource>("res://Testing/AlphaNew.tres");
        GD.Print(alphaExisting);
        alphaExisting.BetaSubResource.TextField2 = "Alpha Overwrite Beta!!";

        MonoCustomResourceIO.SaveExisting("res://Testing/AlphaTest.tres", alphaExisting);

        alphaExisting = MonoCustomResourceIO.Load<AlphaResource>("res://Testing/AlphaNew.tres");
        GD.Print($"Post methods: {alphaExisting}");
    }
}
