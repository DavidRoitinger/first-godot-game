using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FirstGodotGame;

public abstract class StatEffect : Effect
{
    public virtual Dictionary<EntityStats.Stat, double> AffectedStat { get; set; }

    public override int ApplyStatChange(int originalStatValue, EntityStats.Stat changingStat)
    {
        if (AffectedStat.Keys.Any(x => x == changingStat))
            return Mathf.CeilToInt(originalStatValue * Mathf.Pow(AffectedStat[changingStat], Intensity));
        return originalStatValue;
    }
}