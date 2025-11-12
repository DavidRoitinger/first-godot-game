using Godot;
using System;
using System.Diagnostics;

public partial class NewScript : Node2D
{
	public float speed = 10.0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = Position with { X = Position.X + speed * (float) delta };
		if (Input.IsKeyPressed(Key.A))
		{
			GD.Print("NEGGER");
			Position = Position with { Y = Position.Y + speed};
			AudioEffect.WeakRef(this);
		} 
	}
		
}
