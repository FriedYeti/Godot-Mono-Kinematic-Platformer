using Godot;
using System;

public class Enemy : KinematicBody2D
{
	public readonly Vector2 GravityVec = new Vector2(0,900);
	public readonly Vector2 FloorNormal = new Vector2(0,-1);
	public const float WalkSpeed = 70f;
	public const int StateWalking = 0;
	public const int StateKilled = 1;
	
	private Vector2 _linearVelocity = new Vector2();
	private int _direction = -1;
	private string _anim;
	private int _state = StateWalking;
	
	private RayCast2D _detectFloorLeft;
	private RayCast2D _detectWallLeft;
	private RayCast2D _detectFloorRight;
	private RayCast2D _detectWallRight;
	private Sprite _sprite;
	

    public override void _Ready()
    {
		_detectFloorLeft = (RayCast2D)GetNode("detect_floor_left");
		_detectWallLeft = (RayCast2D)GetNode("detect_wall_left");
		_detectFloorRight = (RayCast2D)GetNode("detect_floor_right");
		_detectWallRight = (RayCast2D)GetNode("detect_wall_right");
		_sprite = (Sprite)GetNode("sprite");
        
    }
	
	public override void _PhysicsProcess(float delta)
	{
		string newAnimation = "idle";
		
		if (_state == StateWalking) {
			_linearVelocity += GravityVec * delta;
			_linearVelocity.x = _direction * WalkSpeed;
			_linearVelocity = MoveAndSlide(_linearVelocity, FloorNormal);
			
			if((!_detectFloorLeft.IsColliding()) || (_detectWallLeft.IsColliding()))
			{
				_direction = 1;
			}
			if((!_detectFloorRight.IsColliding()) || (_detectWallRight.IsColliding()))
			{
				_direction = -1;
			}
			
			_sprite.SetScale(new Vector2(_direction,1));
			newAnimation = "walk";
		}
		else
		{
			newAnimation = "explode";
		}
		
		if (_anim != newAnimation)
		{
			_anim = newAnimation;
			((AnimationPlayer)GetNode("anim")).Play(_anim);
		}
	}
	
	public void HitByBullet() {
		_state = StateKilled;
	}
}
