using Godot;
using System;
using System.Diagnostics;

public class TorchLight : OmniLight
{
    private OpenSimplexNoise _noise = new OpenSimplexNoise();
    private float _value = 0;

    public override void _Ready()
    {
        _noise.Period = 16;
    }

    public override void _PhysicsProcess(float delta)
    {
        // base._PhysicsProcess(delta);
        _value += 0.5f;
        _value %= 100000;
        var alpha = (_noise.GetNoise1d(_value) + 1) * 2;
        LightEnergy = alpha;
        // LightColor = new Color(LightColor.r, LightColor.g, LightColor.b, alpha);
    }
}
