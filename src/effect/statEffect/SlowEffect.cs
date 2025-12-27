using System.Collections.Generic;
using Godot;

namespace FirstGodotGame;

public class SlowEffect : StatEffect
{
    public override string Name => "Slow";
    public override string Description => $"Reduces Speed By {AffectedStat[EntityStats.Stat.Speed] * Intensity} for {Duration} Rounds";
    public override string IconPath { get; set; } = "Slow_Icon.png";

    public override Dictionary<EntityStats.Stat, double> AffectedStat { get; set; } = new()
    {
        { EntityStats.Stat.Speed, 0.5 },
    };

    public override Effect Copy()
    {
        return new SlowEffect() //susceptible to copy mistakes...
        {
            Intensity = Intensity,
            Duration = Duration,
        };
    }
}