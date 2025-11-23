using Godot;
using System;
using System.Linq;

public partial class EnemyPathing : CharacterBody2D
{
	[Export] public float speed = 30.0f;
	
	private Node2D targetNode;
	private NavigationAgent2D navigationAgent;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		targetNode = GetTree().GetFirstNodeInGroup("Player") as Node2D ?? this;
		navigationAgent = GetChildren().OfType<CollisionShape2D>().First().GetChildren().OfType<NavigationAgent2D>().First();
		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		navigationAgent.TargetPosition = targetNode.GetGlobalPosition();
		var velocity = GetGlobalPosition().DirectionTo(navigationAgent.GetNextPathPosition()).Normalized() * speed;
		if (navigationAgent.AvoidanceEnabled)
		{
			navigationAgent.SetVelocity(velocity);
		}
		else
		{
			_on_navigation_agent_2d_velocity_computed(velocity);
		}
		MoveAndSlide();
	}

	
	private void _on_navigation_agent_2d_velocity_computed(Vector2 velocity)
	{
		Velocity = velocity;
	}
	
		
	
}
