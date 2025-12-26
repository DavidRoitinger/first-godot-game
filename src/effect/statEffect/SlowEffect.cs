using Godot;

namespace FirstGodotGame;

public class SlowEffect : StatEffect
{
    public override string Name => "Slow";
    public override string Description => $"Reduces Speed By {Intensity} for {Duration} Rounds";
    public override string IconPath { get; set; } = "Slow_Icon.png";

    public override EntityStats.Stat AffectedStat { get; set; } = EntityStats.Stat.Speed;
    
    public override Effect Copy()
    {
        return new SlowEffect()
        {
            Intensity = Intensity,
            Duration = Duration,
        };
    }

    //Reduces Speed instead of increasing it...
    public override int ApplyStatChange(int originalStatValue, EntityStats.Stat changingStat)
    {
        if (changingStat == AffectedStat) return originalStatValue - Intensity;
        return originalStatValue;
    }
}