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
        EntityName = _entityName;
        MaxHealth = _maxHealth;
        Health = _maxHealth;//Todo: Maybe it should be in setup...
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
        set
        {
            _entityName = value;
            
            var lbl = GetParent()
                ?.GetNode<Label>("Control/Label");
            
            if(lbl == null) return;
            lbl.Text = _entityName;
        }
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

    private int _speed;
    [Export] public int Speed
    {
        get
        {
            int effectiveSpeed = _speed;
            
            foreach (var effect in Effects)
            {
                effectiveSpeed = effect.ApplyStatChange(effectiveSpeed, Stat.Speed);
            }
                
            return effectiveSpeed < 0 ? 0 : effectiveSpeed;
        }
        set => _speed = value;
    }

    [Export] public Type EntityType { get; set; }

    

    [Export] public Array<int> AttackIds = [];
    public List<Attack> Attacks = [];
    
    public List<Effect> Effects = [
        // new StatusEffect()
        // {
        //     StatChange = -2,
        //     AffectedStat = Stat.Speed,
        //     Duration = 1,
        // }
    ];
    
    
    public enum Type
    {
        GenericEntity,
        Player,
        Enemy
    }
    
    public enum Stat
    {
        Health,
        MaxHealth,
        Amor,
        Speed,
        Attack,
    }
    
    
    
    public void TakeDamage(int damage)
    {

        if (damage > Armor)
        {
            TakePierceDamage(damage - Armor);
        }
        else
        {
            TakePierceDamage(1);
        }
        
    }
    
    public void TakePierceDamage(int damage)
    {
        Health -= damage;
            
        if (Health > 0)
        {
            PlayAnimation("Action");
        }
        
        var tween = GetTree().CreateTween();
        
        if(_material == null) return;
        tween.TweenMethod(Callable.From((float intensity) => SetFlashIntensity(intensity)) ,1.0, 0.0, 0.3);
        
    }
    
    
    public void HealHealth(int health)
    {
        Health += health;
        
        var tween = GetTree().CreateTween();
        
        if(_material == null) return;
        tween.TweenMethod(Callable.From((float intensity) => SetFlashIntensity(intensity)) ,1.0, 0.0, 0.3);
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
    
    public void PlayAnimation(string animationName)
    {
        if (GetParent().GetChildren().OfType<AnimationPlayer>().First() is { } animationPlayer)
        {
            animationPlayer.Play(animationName);
        }
    }
    
    
    private void SetFlashIntensity(float intensity)
    {
        _material.SetShaderParameter("flashIntensity", intensity);
    }

    public void AddEffect(Effect effect)
    {
        if (effect == null) return;
        Effect effectCopy = effect.Copy();
        //Todo: Test those things...
        //Todo: Make copy of Effect so it does not decay on the attack itself...
        //Todo: Implement Effect stacking...
        //Todo: Finish implementing icons

        var doubleEffect = Effects.FirstOrDefault(x => x.GetType() == effectCopy.GetType());
        if (doubleEffect != null)
        {
            doubleEffect.StackEffect(effectCopy);
        }
        else
        {
            Effects.Add(effectCopy);
        }
        UpdateEffectIcons();
    }
    public void RemoveEffects(List<Effect> effects)
    {
        foreach (var effect in effects)
        {
            Effects.Remove(effect);
        }
    }
    public void UpdateEffectIcons()
    {
        var effectIconContainer = GetParent().GetChildren().OfType<Control>().First().GetChildren().OfType<HBoxContainer>().First();
        foreach (var child in effectIconContainer.GetChildren())
        {
            child.QueueFree();
        }
        
        foreach (var effect in Effects)
        {
            if(effect.IconPath == null) continue;
            var icon = ResourceLoader.Load<Texture2D>($"res://assets/sprites/effectIcons/{effect.IconPath}");
            if(icon == null) return;
            var iconTextureRect = new TextureRect()
            {
                Texture = icon,
                CustomMinimumSize = new Vector2(8, 8),
                TooltipText = $"{effect.Name}: {effect.Description}",
            };
            effectIconContainer.AddChild(iconTextureRect);
        }
        
        // if(effect.IconPath == null) return;
        // var icon = ResourceLoader.Load<Texture2D>($"res://assets/sprites/effectIcons/{effect.IconPath}");
        // if(icon == null) return;
        // GD.Print(icon);
        // var effectIconContainer = GetParent().GetChildren().OfType<Control>().First().GetChildren().OfType<HBoxContainer>().First();
        // var iconTextureRect = new TextureRect()
        // {
        //     Texture = icon,
        //     CustomMinimumSize = new Vector2(8, 8),
        //     TooltipText = $"{effect.Name}: {effect.Description}",
        // };
        // effectIconContainer.AddChild(iconTextureRect);
    }
    
    
    public void TriggerTurnStartEffects()
    {
        List<Effect> expiredEffects = [];
        foreach (var effect in Effects)
        {
            effect.TurnStartTrigger(this);
            if(effect.Duration <= 0) expiredEffects.Add(effect);
        }
        RemoveEffects(expiredEffects);
    }
    public void TriggerTurnEndEffects()
    {
        List<Effect> expiredEffects = [];
        foreach (var effect in Effects)
        {
            effect.TurnEndTrigger(this);
            if(effect.Duration <= 0) expiredEffects.Add(effect);
        }
        RemoveEffects(expiredEffects);
    }
}