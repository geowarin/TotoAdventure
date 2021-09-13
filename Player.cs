using Godot;
using System;
using System.Diagnostics;

public class Player : KinematicBody
{
    [Export] public float Gravity = -24.8f;
    [Export] public float MaxSpeed = 20.0f;
    [Export] public float JumpSpeed = 18.0f;
    [Export] public float Accel = 4.5f;
    [Export] public float Deaccel = 16.0f;
    [Export] public float MaxSlopeAngle = 40.0f;
    [Export] public float MouseSensitivity = 0.05f;

    private Vector3 _vel;
    private Vector3 _dir;

    private Camera _camera;
    private Spatial _rotationHelper;
    private AnimationPlayer _weaponAnimationPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _camera = GetNode<Camera>("RotationHelper/Camera");
        _rotationHelper = GetNode<Spatial>("RotationHelper");
        _weaponAnimationPlayer = GetNode<AnimationPlayer>("RotationHelper/Weapon/Sword/AnimationPlayer");

        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _PhysicsProcess(float delta)
    {
        ProcessInput(delta);
        ProcessMovement(delta);
    }

    public void _on_Sword_body_entered(Node body)
    {
        if (_weaponAnimationPlayer.CurrentAnimation != "Attack")
        {
            Debug.Print("nope");
            return;
        }
        Debug.Print("collision!!!");
        if (body is Skeleton skeleton)
        {
            skeleton.Damage();
        }
    }

    private void ProcessInput(float delta)
    {
        //  -------------------------------------------------------------------
        //  Walking
        _dir = new Vector3();
        Transform camXform = _camera.GlobalTransform;

        Vector2 inputMovementVector = new Vector2();

        if (Input.IsActionPressed("movement_forward"))
            inputMovementVector.y += 1;
        if (Input.IsActionPressed("movement_backward"))
            inputMovementVector.y -= 1;
        if (Input.IsActionPressed("movement_left"))
            inputMovementVector.x -= 1;
        if (Input.IsActionPressed("movement_right"))
            inputMovementVector.x += 1;

        inputMovementVector = inputMovementVector.Normalized();

        // Basis vectors are already normalized.
        _dir += -camXform.basis.z * inputMovementVector.y;
        _dir += camXform.basis.x * inputMovementVector.x;
        //  -------------------------------------------------------------------

        //  -------------------------------------------------------------------
        //  Jumping
        if (IsOnFloor())
        {
            if (Input.IsActionJustPressed("movement_jump"))
                _vel.y = JumpSpeed;
        }
        //  -------------------------------------------------------------------

        if (Input.IsActionJustPressed("movement_attack"))
        {
           _weaponAnimationPlayer.Play("Attack");
        }
        //  -------------------------------------------------------------------
        //  Capturing/Freeing the cursor
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GetTree().Quit();
            // Input.SetMouseMode(Input.GetMouseMode() == Input.MouseMode.Visible
            //     ? Input.MouseMode.Captured
            //     : Input.MouseMode.Visible);
        }
        //  -------------------------------------------------------------------
    }

    private void ProcessMovement(float delta)
    {
        _dir.y = 0;
        _dir = _dir.Normalized();

        _vel.y += delta * Gravity;

        Vector3 hvel = _vel;
        hvel.y = 0;

        Vector3 target = _dir;

        target *= MaxSpeed;

        var accel = _dir.Dot(hvel) > 0 ? Accel : Deaccel;

        hvel = hvel.LinearInterpolate(target, accel * delta);
        _vel.x = hvel.x;
        _vel.z = hvel.z;
        
        _vel = MoveAndSlide(_vel, Vector3.Up, false, 4, Mathf.Deg2Rad(MaxSlopeAngle));
    }

    public override void _Input(InputEvent @event)
    {
        if (!(@event is InputEventMouseMotion mouseEvent) || Input.GetMouseMode() != Input.MouseMode.Captured)
        {
            return;
        }

        _rotationHelper.RotateX(Mathf.Deg2Rad(-mouseEvent.Relative.y * MouseSensitivity));
        RotateY(Mathf.Deg2Rad(-mouseEvent.Relative.x * MouseSensitivity));

        Vector3 cameraRot = _rotationHelper.RotationDegrees;
        cameraRot.x = Mathf.Clamp(cameraRot.x, -70, 70);
        _rotationHelper.RotationDegrees = cameraRot;
    }
}