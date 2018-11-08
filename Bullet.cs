using Godot;
using System;

public class Bullet : RigidBody2D
{
	public void _OnBulletBodyEnter( Godot.Object body )
	{
		if(body.HasMethod("HitByBullet"))
		{
			body.Call("HitByBullet");
		}
	}

	private void _OnTimerTimeout()
	{
		((AnimationPlayer)GetNode("anim")).Play("shutdown");
	}
}
