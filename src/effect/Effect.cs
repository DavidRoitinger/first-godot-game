using System;
using System.Text.Json.Serialization;
using Godot;

namespace FirstGodotGame;
//Todo: Don't forget... nig...
[JsonDerivedType(typeof(SlowEffect), typeDiscriminator: "slowEffect")]
[JsonDerivedType(typeof(FocusEffect), typeDiscriminator: "focusEffect")]
[JsonDerivedType(typeof(ResistantEffect), typeDiscriminator: "resistantEffect")]
[JsonDerivedType(typeof(PoisonEffect), typeDiscriminator: "poisonEffect")]
[JsonDerivedType(typeof(RegenEffect), typeDiscriminator: "regenEffect")]
public abstract class Effect
{
    public virtual string Name { get; set; }
    public virtual string Description { get; set; }
    public virtual string IconPath { get; set; }
    
    public int Intensity { get; set; }
    public int Duration { get; set; }

    
    public abstract Effect Copy();
    
    public virtual void TurnStartTrigger(EntityStats entityStats)
    {
        
    }

    public virtual void TurnEndTrigger(EntityStats entityStats)
    {
        Duration--;
    }

    public virtual int ApplyStatChange(int originalStatValue, EntityStats.Stat changingStat)
    {
        return originalStatValue;
    } 
    
    public virtual void StackEffect(Effect effect)
    {
        Intensity = Math.Max(Intensity, effect.Intensity);
        Duration += effect.Duration;
    }
}