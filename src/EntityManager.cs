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
			
			if (_entities[_turnIndex].EntityType == EntityStats.Type.Enemy)
			{
				await EnemyTurn(_entities[_turnIndex]);
			
				EndTurn();
			
			}else if(_entities[_turnIndex].EntityType == EntityStats.Type.Player){
				
			
				await PlayerTurn();
			
				EndTurn();
			}
			
			await Task.Delay(TurnDurationMs);
		}
	}

	public void EndTurn()
	{
		_turnIndex++;
		if (_turnIndex >= _entities.Count) _turnIndex = 0;
		
		
	}
	
	private async Task EnemyTurn(EntityStats entity)
	{
		//Enemy logic...
		GD.Print($"Enemy Move! {_entities[_turnIndex].EntityName} ?");

		var entityAttack = entity.GetParent().GetChildren().FirstOrDefault(x => x is IEntityAttack) as IEntityAttack;
		var entityMove = entity.GetParent().GetChildren().FirstOrDefault(x => x is IEntityMove) as IEntityMove;

		
		await (entityMove?.Move(entity, _entities) ?? Task.CompletedTask);
		
		entityAttack?.Attack(entity, _entities);
	}

	
	
	private Task PlayerTurn()
	{
		//Player logic...
		GD.Print($"Player Move! {_entities[_turnIndex].EntityName} ?");
		return Task.CompletedTask;
	}
	
	
}
