using Godot;
using GDC = Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Test : Node
{
    [Export]
    public Resource resourceOne;
    [Export]
    public AlphaResource resourceTwo;
    [Export]
    public GDC.Array<Resource> resourceList;

    [Export]
    public NodePath labelNodePath;
    private Label label;

    public override void _Ready()
    {
        label = GetNode<Label>(labelNodePath);

        label.Text = "";
        label.Text += "Resource One: " + resourceOne + "\n";
        label.Text += "Resource Two: " + resourceTwo + "\n";
        label.Text += "Resource List:\n";
        foreach (Resource resource in resourceList)
        {
            label.Text += "\t" + resource + "\n";
        }
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
    }
}
