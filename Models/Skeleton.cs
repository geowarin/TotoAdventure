using Godot;
using System;
using System.Diagnostics;

public class Skeleton : Spatial
{
    private AnimationPlayer _animationPlayer;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.GetAnimation("Skeleton_Idle").Loop = true;
        _animationPlayer.Play("Skeleton_Idle");

        // Input.Singleton.Connect("joy_connection_changed", this, nameof(Input_JoyConnectionChanged));
        _animationPlayer.Connect("animation_finished", this, nameof(_on_AnimationPlayer_animation_finished));
        // Connect("animation", animationPlayer)
        // animationPlayer.Autoplay = "Skeleton_Idle";
    }

    public void _on_AnimationPlayer_animation_finished(string animName)
    {
        Debug.Print("finished " + animName);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
    public void Damage()
    {
        _animationPlayer.Play("Skeleton_Death");
    }
}