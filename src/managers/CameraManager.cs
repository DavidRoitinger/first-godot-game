using Godot;
using System.Collections.Generic;
using System.Linq;
using FirstGodotGame;

public partial class CameraManager : Camera2D
{

	public List<EntityStats> TargetPositionList = [];

	[Export] public double Speed = 1; 
	

	private TileMapLayer _groundLayer 
	{
		get
		{
			return GetTree()
				.GetNodesInGroup("Tilemap")
				.First(node => node.Name == "GroundLayer") as TileMapLayer;
		}
	}
	
	public static CameraManager Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
	}
	
	
	public void ChangeTargets(List<EntityStats> newTargets)
	{
		TargetPositionList = newTargets;
		
		if (!TargetPositionList.Any()) return;
		
		var positionSum = TargetPositionList
			.Select(x => _groundLayer.MapToLocal(x.GridPosition))
			.Aggregate(Vector2.Zero, (x, y) => x + y);

		var averagePosition = positionSum / TargetPositionList.Count;

		var tween = GetTree().CreateTween();
		
		tween.TweenProperty(this, "position", averagePosition,
			this.Position.DistanceTo(averagePosition) / Speed);

	}

	public void AddTarget(EntityStats newTarget)
	{
		ChangeTargets([..TargetPositionList, newTarget]);
	}
	
	public void Clear()
	{
		TargetPositionList = [];
	}
	
	public void RemoveTarget(EntityStats target)
	{
		ChangeTargets(TargetPositionList
			.Where(x => x != target)
			.ToList());
	}
	
}
