using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace FirstGodotGame;
[Tool]
public partial class EntityStats : Node
{
    
    private ShaderMaterial _material;
    public override void _Ready()
    {
        GridPosition = _gridPosition;
        _material = GetParent().GetChildren().OfType<Sprite2D>().First().Material as ShaderMaterial;

        if (Attacks.Count == 0)
        {
            foreach (int attackId in AttackIds)
            {
                Attacks.Add(AttackPool.Instance.GetAttackById(attackId));
            }
        }
        
        
    }
    
    
    private string _entityName;
    [Export]
    public string EntityName
    {
        get => _entityName;
        set => _entityName = value;
        // var bar = GetParent()
        //     ?.GetNode<ProgressBar>("Control/ProgressBar");
        //
        // if(bar == null) return;
    }

    private Vector2I _gridPosition;
    
    [Export]
    public Vector2I GridPosition
    {
        get => _gridPosition;
        set
        {
            _gridPosition = value;
            
            if (IsInsideTree() && GetTree().HasGroup("Tilemap")) // copyright armin[MIT License], Valveλ
                GetParent<Node2D>()
                    ?.SetPosition(
                        GetTree()
                            .GetNodesInGroup("Tilemap")
                            .OfType<TileMapLayer>()
                            .First(node => node.Name == "GroundLayer")
                            .MapToLocal(_gridPosition));
         

        }
    }



    private int _maxHealth;
    [Export] public int MaxHealth
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
            
            var bar = GetParent()
                ?.GetNode<ProgressBar>("Control/ProgressBar");
            
            if(bar == null) return;
            bar.MaxValue = MaxHealth;
        }
    }
    
    private int _health;
    [Export] public int Health {
        get => _health;
        set
        {
            _health = value;

            var bar = GetParent()
                ?.GetNode<ProgressBar>("Control/ProgressBar");
            
            if(bar == null) return;
            bar.Value = _health;
            
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

    [Export] public Array<int> AttackIds = [];
    public List<Attack> Attacks = [];

    public void TakeDamage(int damage)
    {

        if (damage > Armor)
        {
            Health -= damage - Armor;
        }
        
        
        var tween = GetTree().CreateTween();
        
        if(_material == null) return;
        tween.TweenMethod(Callable.From((float intensity) => SetFlashIntensity(intensity)) ,1.0, 0.0, 0.3);
        
    }
    
    public void HealHealth(int health)
    {
        Health += health;
    }

    public void AddAttack(Attack attack)
    {
        Attacks.Add(attack);
        AttackIds.Add(attack.Id);
    }
    public void AddAttackById(int id)
    {
        Attacks.Add(AttackPool.Instance.GetAttackById(id));
        AttackIds.Add(id);
    }
    
    public void AddAttackListById(int[] ids)
    {
        foreach (var id in ids)
        {
            Attacks.Add(AttackPool.Instance.GetAttackById(id));
            AttackIds.Add(id);
        }
    }
    public void AddAttackList(Attack[] attacks)
    {
        foreach (var attack in attacks)
        {
            Attacks.Add(attack);
            AttackIds.Add(attack.Id);
        }
    }
    public void ClearAttacks()
    {
        AttackIds.Clear();
        Attacks.Clear();
    }
    
    
    private void SetFlashIntensity(float intensity)
    {
        _material.SetShaderParameter("flashIntensity", intensity);
    }
    
}