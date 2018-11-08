using Godot;
using System;

public class MovingPlatform : Node2D
{
	[Export] public Vector2 Motion;
	[Export] public float Cycle = 1f;
	
	private float _accum = 0f;
	
	public override void _PhysicsProcess(float delta)
	{
		/*
	accum += delta * (1.0 / cycle) * PI * 2.0
	accum = fmod(accum, PI * 2.0)
	var d = sin(accum)
	var xf = Transform2D()
	xf[2]= motion * d
	$platform.transform = xf

		*/
		_accum += delta * (1.0f / Cycle) * Mathf.Pi * 2.0f;
		_accum = _accum % (Mathf.Pi * 2.0f);
		float d = Mathf.Sin(_accum);
		((KinematicBody2D)GetNode("platform")).SetPosition(Motion * d);
	}
}
