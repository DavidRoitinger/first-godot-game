using Godot;

namespace FirstGodotGame;

public partial class NewScript : CharacterBody2D
{
	[Export]
	public float Speed = 500.0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Velocity = Velocity with { X = Input.GetAxis("Left", "Right") * Speed };
		Velocity = Velocity with { Y = Input.GetAxis("Up", "Down") * Speed };

		MoveAndSlide();
		
		if(Input.IsActionJustPressed("Action"))
			Scale = new Vector2(Scale.X * new RandomNumberGenerator().Randf()-0.5f, Scale.Y * new RandomNumberGenerator().Randf()-0.5f).Abs();
	}
		
}
