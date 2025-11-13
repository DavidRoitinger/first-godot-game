using Godot;

namespace FirstGodotGame;

public partial class AudioStreamPlayer2d : AudioStreamPlayer2D
{
	[Export]
	public AudioStream[] audioStreams;
	
	bool index = false;
	
	double timer = 0.0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Stream = audioStreams[index ? 1 : 0 ];
		this.Play();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timer -= delta;

		if (Input.IsKeyPressed(Key.D))
		{
			if (timer <= 0.0)
			{
				index = !index;
				this.Stream = audioStreams[index ? 1 : 0 ];
				this.Play();
				timer = 0.5;
			}
		}
	}
}
