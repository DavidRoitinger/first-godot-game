using System;
using System.Linq;
using Godot;

namespace FirstGodotGame;
[Tool]
public partial class EntityStats : Node
{
    [Export] public string EntityName { get; set; }

    private Vector2I _gridPosition;
    
    [Export]
    public Vector2I GridPosition
    {
        get => _gridPosition;
        set
        {
            _gridPosition = value;
            
            GetParent<Node2D>()
                ?.SetPosition(
                    GetTree()
                        .GetNodesInGroup("Tilemap")
                        .OfType<TileMapLayer>()
                        .First(node => node.Name == "GroundLayer")
                        .MapToLocal(_gridPosition));
          
     
 
        }
    }

    public override void _Ready()
    {
        GridPosition = _gridPosition;
    }


    [Export] public int MaxHealth { get; set; }
    
    private int _health;
    [Export] public int Health {
        get => _health;
        set
        {
            _health = value;
            
            if (_health <= 0) (GetParent().GetChildren().FirstOrDefault(x => x is IEntityDie) as IEntityDie)?.Die(this);
        }
        
    }
    
    [Export] public int Armor { get; set; }
    
    [Export] public int Speed { get; set; }

    [Export] public Type EntityType { get; set; }

    
    public enum Type
    {
        GenericEntity,
        Player,
        Enemy
    }

    public void TakeDamage(int damage)
    {
        Health -= damage - Armor;
    }
    public void HealHealth(int health)
    {
        Health += health;
    }
    
}