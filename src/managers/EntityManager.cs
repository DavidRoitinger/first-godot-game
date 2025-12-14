using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using PhantomCamera;

namespace FirstGodotGame;

public partial class EntityManager : Node
{
	private List<EntityStats> _entities;
	
	private  TileMapLayer _groundLayer;
	private  TileMapLayer _highlightLayer;
	private  PhantomCamera2D _phantomCamera2D;

	private int _turnIndex;

	[Export] public int TurnDurationMs = 1000;


	public override void _Ready()
	{
		SaveLoadManager.Load(GetTree());
		LoadEntities();
		
		_groundLayer = GetTree().GetNodesInGroup("Tilemap").First(node => node.Name == "GroundLayer") as TileMapLayer;
		_highlightLayer = GetTree().GetNodesInGroup("Tilemap").First(node => node.Name == "HighlightLayer") as TileMapLayer;

		_phantomCamera2D = GetNode<Node2D>("%PhantomCamera2D").AsPhantomCamera2D();
		
		// _phantomCamera2D.FollowTargets = _entities
		// 	.Where(x => x.EntityType == EntityStats.Type.Player)
		// 	.Select(x => x.GetParent() as Node2D)
		// 	.ToArray();
		
		_ = GameLoop();
	}


	private async Task GameLoop()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		
		while (true)
		{
			await StartTurn();

			await Task.Delay(TurnDurationMs/TurnSkipper.SpeedUpTurn);

			if (_entities[_turnIndex].Health > 0)
			{
				await EntityTurn(_entities[_turnIndex]);
			}
			
			await EndTurn();
			
			await Task.Delay(TurnDurationMs/TurnSkipper.SpeedUpTurn);
			
		}
	}

	private async Task StartTurn()
	{
		_highlightLayer.Clear();
		LoadEntities();

		if (!_entities.Any(x => x.EntityType == EntityStats.Type.Player && x.Health > 0))
		{
			//Looose
			await SceneManager.Instance.ReloadCurrentLevel();
		}
		if(!_entities.Any(x => x.EntityType == EntityStats.Type.Enemy && x.Health > 0))
		{
			//Win
			
			//Todo:Upgrade Menu here...
			await ShopManager.Instance.OpenShop();
			
			await SceneManager.Instance.LoadNextLevel();
		}
		

		// if (_entities[_turnIndex].EntityType != EntityStats.Type.Player)
		// {
			_phantomCamera2D.FollowTargets =
				[.._phantomCamera2D.FollowTargets, _entities[_turnIndex].GetParent() as Node2D];
		// }
		
	}

	private async Task EndTurn()
	{
		// if (_entities[_turnIndex].EntityType != EntityStats.Type.Player)
		// {
			_phantomCamera2D.FollowTargets =
				_phantomCamera2D.FollowTargets
					.Where(x => x != _entities[_turnIndex].GetParent())
					.ToArray();
		// }
		
		await HandelOutOfBounds();

		_turnIndex++;
		if (_turnIndex >= _entities.Count) _turnIndex = 0;
		
	}

	private async Task HandelOutOfBounds()
	{
		foreach (var entityStats in _entities)
		{
			if (_groundLayer.GetCellTileData(entityStats.GridPosition) == null)
			{
				
				_phantomCamera2D.FollowTargets =
					[.._phantomCamera2D.FollowTargets, entityStats.GetParent() as Node2D];

				
				
				var newSaveTile = _groundLayer
					.GetUsedCells()
					.Where(pos =>
						_entities.All(eS => eS.GridPosition != pos)
					).MinBy(x => x.DistanceTo(entityStats.GridPosition));
				
				
				await Task.Delay(500/TurnSkipper.SpeedUpTurn);

				SoundManager.Instance.PlaySfx(SoundManager.Shot);
				entityStats.TakeDamage((int)Math.Ceiling(entityStats.Health / 10.0));
				entityStats.GridPosition = newSaveTile;
				
				await Task.Delay(500/TurnSkipper.SpeedUpTurn);
				
				
				_phantomCamera2D.FollowTargets =
					_phantomCamera2D.FollowTargets
						.Where(x => x != entityStats.GetParent())
						.ToArray();

			}
		}
	}

	private async Task EntityTurn(EntityStats entity)
	{
		
		//Enemy logic...
		GD.Print($"Entity Turn! {_entities[_turnIndex].EntityName} ?");

		var entityAttack = entity.GetParent().GetChildren().FirstOrDefault(x => x is IEntityAttack) as IEntityAttack;
		var entityMove = entity.GetParent().GetChildren().FirstOrDefault(x => x is IEntityMove) as IEntityMove;

		
		await (entityMove?.Move(entity, _entities) ?? Task.CompletedTask);
		
		await (entityAttack?.Attack(entity, _entities)?? Task.CompletedTask);
	}
	
	private void LoadEntities()
	{
		_entities = GetTree().GetNodesInGroup("Entity")
			.Select(node => node.GetChildren()
					.First(childNode => childNode.Name == "EntityStats")
				as EntityStats
			).OrderBy(eS => eS.Speed)
			.ToList();
	}

}
