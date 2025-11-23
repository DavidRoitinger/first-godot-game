using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace FirstGodotGame;

public partial class EntityManager : Node
{
	private List<EntityStats> _entities;
	
	private  TileMapLayer _groundLayer;
	private  TileMapLayer _highlightLayer;

	private int _turnIndex;

	[Export] public int TurnDurationMs = 2000;



	public override void _Ready()
	{
		_entities = GetTree().GetNodesInGroup("Entity")
			.Select(node => node.GetChildren()
				.First(childNode => childNode.Name == "EntityStats")
				as EntityStats
			).ToList();
		
		_groundLayer = GetTree().GetNodesInGroup("Tilemap").First(node => node.Name == "GroundLayer") as TileMapLayer;
		_highlightLayer = GetTree().GetNodesInGroup("Tilemap").First(node => node.Name == "HighlightLayer") as TileMapLayer;

		GameLoop();
	}

	
	

	public async Task GameLoop()
	{
		while (true)
		{
			_highlightLayer.Clear();

			if (_entities[_turnIndex].Health > 0)
			{
				await EntityTurn(_entities[_turnIndex]);
			}
			
			
			EndTurn();
			await Task.Delay(TurnDurationMs);
		}
	}

	public void EndTurn()
	{
		_turnIndex++;
		if (_turnIndex >= _entities.Count) _turnIndex = 0;
		
		
	}
	
	private async Task EntityTurn(EntityStats entity)
	{
		//Enemy logic...
		GD.Print($"Enemy Move! {_entities[_turnIndex].EntityName} ?");

		var entityAttack = entity.GetParent().GetChildren().FirstOrDefault(x => x is IEntityAttack) as IEntityAttack;
		var entityMove = entity.GetParent().GetChildren().FirstOrDefault(x => x is IEntityMove) as IEntityMove;

		
		await (entityMove?.Move(entity, _entities) ?? Task.CompletedTask);
		
		await (entityAttack?.Attack(entity, _entities)?? Task.CompletedTask);
	}

	
	

	
	
}
