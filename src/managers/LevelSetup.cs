using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace FirstGodotGame;

public partial class LevelSetup : Node
{
    [Export] public int EnemyCount { get; set; }
    [Export] public float LevelDifficulty { get; set; }
    [Export] public Array<PackedScene> Enemies { get; set; }
    [Export] public int SafetyRange { get; set; }

    private TileMapLayer _groundLayer;
    public TileMapLayer GroundLayer
    {
        get
        {
            _groundLayer ??= GetTree()
                .GetNodesInGroup("Tilemap")
                .First(node => node.Name == "GroundLayer") as TileMapLayer;
            return _groundLayer;
        }
    }
    
    public override void _Ready()
    {
        SetupLevel();
        
        
    }

    public void SetupLevel()
    {
        //Todo: Handle Music...
        SoundManager.Instance.PlayMusic("Stone cold toad.mp3");
        
        SaveLoadManager.Load(GetTree());
        
        PopulateEnemies();
        GiveUpgradesToEnemies(LevelDifficulty);
        HealEntities();
        _ = EntityManager.Instance.StartGameLoop();
    }

    private void HealEntities()
    {
        foreach (var entity in LoadEntities())
        {
            entity.Health = entity.MaxHealth;
        }
    }

    private void GiveUpgradesToEnemies(float difficulty)
    {
        List<EntityStats> enemies = LoadEntities()
            .Where(x => x.EntityType == EntityStats.Type.Enemy)
            .ToList();

        Random rand = new Random();
        
        while (difficulty >= 0)
        {
            if (difficulty <= 0.3f)
            {
                UpgradePool.Instance.CommonUpgradePool[rand.Next(UpgradePool.Instance.CommonUpgradePool.Count)]
                    .ApplyUpgrade(enemies[rand.Next(enemies.Count)]);
                difficulty -= 0.1f;
            }
            else if(difficulty <= 1.0f)
            {
                UpgradePool.Instance.StrangeUpgradePool[rand.Next(UpgradePool.Instance.StrangeUpgradePool.Count)]
                    .ApplyUpgrade(enemies[rand.Next(enemies.Count)]);
                difficulty -= 0.3f;
            }else if(difficulty > 1.0f)
            {
                UpgradePool.Instance.BizarreUpgradePool[rand.Next(UpgradePool.Instance.BizarreUpgradePool.Count)]
                    .ApplyUpgrade(enemies[rand.Next(enemies.Count)]);
                difficulty -= 0.5f;
            }
        }
    }

    private void PopulateEnemies()
    {
        var node = GetNode("../Entities");
        for (int i = 0; i < EnemyCount; i++)
        {
            var entities = LoadEntities();
            
            var instantce = Enemies[0].Instantiate();

            instantce.GetChildren()
                    .OfType<EntityStats>()
                    .First()
                    .GridPosition = 
                PickRandomPosition(entities);
            
            node.AddChild(instantce);

        }
    }

    private Vector2I PickRandomPosition(List<EntityStats> entities)
    {
        return new Array<Vector2I>(GroundLayer
                .GetUsedCells()
                .Where(pos =>
                    !entities.Any(eS =>
                    {
                        if (eS.EntityType == EntityStats.Type.Player)
                        {
                            return Math.Abs(eS.GridPosition.X - pos.X) <= SafetyRange &&
                                   Math.Abs(eS.GridPosition.Y - pos.Y) <= SafetyRange;
                        }
                        return eS.GridPosition == pos; 
                    })).ToArray())
            .PickRandom();
    }

    private List<EntityStats> LoadEntities()
    {
        return GetTree().GetNodesInGroup("Entity")
            .Select(node => node.GetChildren()
                    .First(childNode => childNode.Name == "EntityStats")
                as EntityStats
            ).ToList();
    }
}