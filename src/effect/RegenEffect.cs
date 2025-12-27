using System;
using Godot;

namespace FirstGodotGame;

public class RegenEffect : Effect
{
    public override string Name => "Regen";
    public override string Description => $"Heals {Intensity} Hp for {Duration} Rounds";
    public override string IconPath { get; set; } = "Regen_Icon.png";
    
    
    public override Effect Copy()
    {
        return new RegenEffect() //susceptible to copy mistakes...
        {
            Intensity = Intensity,
            Duration = Duration,
        };
    }
    
    public override void TurnStartTrigger(EntityStats entityStats)
    {
        SoundManager.Instance.PlaySfx(SoundManager.Buff);
        entityStats.HealHealth(Intensity);
        Duration--;
    }

    public override void TurnEndTrigger(EntityStats entityStats)
    {
        return;
    }
    
}