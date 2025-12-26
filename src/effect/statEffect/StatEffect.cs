namespace FirstGodotGame;

public abstract class StatEffect : Effect
{
    public virtual EntityStats.Stat AffectedStat { get; set; }
    

    public override int ApplyStatChange(int originalStatValue, EntityStats.Stat changingStat)
    {
        if (changingStat == AffectedStat) return originalStatValue + Intensity;
        return originalStatValue;
    }
}