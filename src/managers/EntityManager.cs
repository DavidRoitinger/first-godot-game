using System;
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
	// private  PhantomCamera2D _phantomCamera2D;

	private int _turnIndex;
	private bool _sceneChanged = false;

	[Export] public int TurnDurationMs = 500;


	public static EntityManager Instance { get; private set; }
	
	public override void _Ready()
	{
		Instance = this;
	}
	
	public async Task StartGameLoop()
	{
		GD.Print(GetTree().ToString());
		
		LoadEntities();
		
		_groundLayer = GetTree().GetNodesInGroup("Tilemap").First(node => node.Name == "GroundLayer") as TileMapLayer;
		_highlightLayer = GetTree().GetNodesInGroup("Tilemap").First(node => node.Name == "HighlightLayer") as TileMapLayer;

		//Todo: Find better solution... not sure it is supposed to work like that...
		
		// _phantomCamera2D = GetTree().GetCurrentScene().GetNode<Node2D>("%PhantomCamera2D").AsPhantomCamera2D();
		//
		// _phantomCamera2D.FollowTargets = _entities
		// 	.Where(x => x.EntityType == EntityStats.Type.Player)
		// 	.Select(x => x.GetParent() as Node2D)
		// 	.ToArray();
		//
		CameraManager.Instance.ChangeTargets(_entities
			.Where(x => x.EntityType == EntityStats.Type.Player)
			.ToList()
			);
		
		await GameLoop();
	}

	private async Task GameLoop()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		
		while (!_sceneChanged)
		{
			await StartTurn();
			
			if (_entities[_turnIndex].Health > 0)
			{
				await EntityTurn(_entities[_turnIndex]);
			}
			
			await EndTurn();
			
		}

		_sceneChanged = false;
	}

	private async Task StartTurn()
	{
		_highlightLayer.Clear();
		LoadEntities();
		CameraManager.Instance.AddTarget(_entities[_turnIndex]);

		_entities[_turnIndex].TriggerTurnStartEffects();
		
		await Task.Delay(TurnDurationMs/TurnSkipper.SpeedUpTurn);
	}
	

	private async Task EndTurn()
	{
		if (!_entities.Any(x => x.EntityType == EntityStats.Type.Player && x.Health > 0))
		{
			//Looose
			CameraManager.Instance.Clear();
			SceneManager.Instance.ReloadCurrentLevel();
			_sceneChanged = true;
		} else if(!_entities.Any(x => x.EntityType == EntityStats.Type.Enemy && x.Health > 0))
		{
			//Win
			
			//Todo:Upgrade Menu here...
			await ShopManager.Instance.OpenShop();

			CameraManager.Instance.Clear();
			SceneManager.Instance.LoadNextLevel();
			_sceneChanged = true;
		}
		else
		{
			CameraManager.Instance.RemoveTarget(_entities[_turnIndex]);
			
			await HandelOutOfBounds();
			
			_entities[_turnIndex].TriggerTurnEndEffects();

			_turnIndex++;
			if (_turnIndex >= _entities.Count) _turnIndex = 0;
		}
	}

	private async Task HandelOutOfBounds()
	{
		foreach (var entityStats in _entities)
		{
			if (_groundLayer.GetCellTileData(entityStats.GridPosition) == null)
			{
				
				// _phantomCamera2D.FollowTargets =
				// 	[.._phantomCamera2D.FollowTargets, entityStats.GetParent() as Node2D];
				
				CameraManager.Instance.AddTarget(entityStats);
				
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
				
				
				// _phantomCamera2D.FollowTargets =
				// 	_phantomCamera2D.FollowTargets
				// 		.Where(x => x != entityStats.GetParent())
				// 		.ToArray();
				
				CameraManager.Instance.RemoveTarget(entityStats);
			}
		}
	}

	private async Task EntityTurn(EntityStats entity)
	{
		

		var entityAttack = entity.GetParent().GetChildren().FirstOrDefault(x => x is IEntityAttack) as IEntityAttack;
		var entityMove = entity.GetParent().GetChildren().FirstOrDefault(x => x is IEntityMove) as IEntityMove;
		var entityUi = entity.GetParent().GetChildren().FirstOrDefault(x => x is EntityUI) as EntityUI;

		entityUi?.ShowUi(2);
		
		await (entityMove?.Move(entity, _entities) ?? Task.CompletedTask);
		
		await (entityAttack?.Attack(entity, _entities)?? Task.CompletedTask);
		
		entityUi?.HideUi(2);
	}
	
	private void LoadEntities()
	{
		_entities = Instance.GetTree().GetNodesInGroup("Entity")
			.Select(node => node.GetChildren()
					.First(childNode => childNode.Name == "EntityStats")
				as EntityStats
			).ToList();
	}

}
