using Godot;
using System;

public class Player : KinematicBody2D
{

	public readonly Vector2 GravityVec = new Vector2(0, 900);
	public readonly Vector2 FloorNormal = new Vector2(0, -1);
	public const float SlopeSlideStop = 25.0f;
	public const float MinOnAirTime = 0.1f;
	public const float WalkSpeed = 250f; // pixels/sec
	public const float JumpSpeed = 480f;
	public const float SidingChangeSpeed = 10f;
	public const float BulletVelocity = 1000f;
	public const float ShootTimeShowWeapon = 0.2f;


	private Vector2 _linearVelocity = new Vector2();
	private float _onAirTime = 0f;
	private bool _onFloor = false;
	private float _shootTime = 99999f; // time since last shot
	private string _anim;
	private Sprite _sprite = new Sprite();
	private AudioStreamPlayer2D _jumpSound;
	private AudioStreamPlayer2D _shootSound;
	private PackedScene _bulletScene;

    public override void _Ready()
    {
		_sprite = (Sprite)GetNode("sprite");
		_jumpSound = (AudioStreamPlayer2D)GetNode("sound_jump");
		_bulletScene = (PackedScene)GD.Load("res://bullet.tscn");
		_shootSound = (AudioStreamPlayer2D)GetNode("sound_shoot");
    }

    public override void _PhysicsProcess(float delta)
    {
		// increment counters
		_onAirTime += delta;
		_shootTime += delta;
		
		// MOVEMENT
		// apply gravity
		_linearVelocity += delta * GravityVec;
		// Move and Slide
		_linearVelocity = MoveAndSlide(_linearVelocity,FloorNormal,SlopeSlideStop);
		// Detect floor
		if(IsOnFloor())
		{
			_onAirTime = 0;
		}
		_onFloor = _onAirTime < MinOnAirTime;
		
		// CONTROL
		// Horizontal Movement
		float targetSpeed = 0f;
		if(Input.IsActionPressed("move_left"))
		{
			targetSpeed += -1;
		}
		if(Input.IsActionPressed("move_right"))
		{
			targetSpeed += 1;
		}
		
		targetSpeed *= WalkSpeed;
		_linearVelocity.x = Mathf.Lerp(_linearVelocity.x, targetSpeed, 0.1f);
		
		// Jumping
		if(_onFloor && Input.IsActionJustPressed("jump"))
		{
			_linearVelocity.y = -JumpSpeed;
			_jumpSound.Play();
		}
		// Shooting
		if(Input.IsActionJustPressed("shoot"))
		{
			// Preload is currently not available in c#
			//auto bullet = Preload
			
			RigidBody2D bullet = (RigidBody2D)_bulletScene.Instance();
			bullet.Position = ((Position2D)GetNode("sprite/bullet_shoot")).GlobalPosition;
			bullet.LinearVelocity = new Vector2(_sprite.Scale.x * BulletVelocity, 0);
			bullet.AddCollisionExceptionWith(this);
			GetParent().AddChild(bullet);
			_shootSound.Play();
			_shootTime = 0;
		}
		
		// ANIMATION
		string newAnimation = "idle";
		
		if(_onFloor)
		{
			if(_linearVelocity.x < -SidingChangeSpeed) {
				_sprite.SetScale(new Vector2(-1,1));
				newAnimation = "run";
			}
			if(_linearVelocity.x > SidingChangeSpeed) {
				_sprite.SetScale(new Vector2(1,1));
				newAnimation = "run";
			}
		}
		else
		{
			// We want the character to immediately change facing side when the player
			// tries to change direction, during air control.
			// This allows for example the player to shoot quickly left then right.
			if(Input.IsActionPressed("move_left") && !Input.IsActionPressed("move_right"))
			{
				_sprite.SetScale(new Vector2(-1,1));
			}
			if(Input.IsActionPressed("move_right") && !Input.IsActionPressed("move_left"))
			{
				_sprite.SetScale(new Vector2(1,1));
			}
			if(_linearVelocity.y < 0)
			{
				newAnimation = "jumping";
			}
			else
			{
				newAnimation = "falling";
			}
		}
		if(_shootTime < ShootTimeShowWeapon)
		{
			newAnimation += "_weapon";
		}
		
		if(newAnimation != _anim) {
			_anim = newAnimation;
			((AnimationPlayer)GetNode("anim")).Play(_anim);
		}
    }
}
