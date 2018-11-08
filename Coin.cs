using Godot;
using System;

public class Coin : Area2D
{
	private bool _taken = false;
	
	public void _OnCoinBodyEnter( Godot.Object body )
	{
		if(!_taken && ((string)body.Get("name") == "player")) {
			((AnimationPlayer)GetNode("anim")).Play("taken");
			_taken = true;
		}
	}
}
