using System.Collections.Generic;

namespace FirstGodotGame;

public class ResistantEffect: StatEffect
{

    public override string Name => "Resistant";
    //Todo: Custom Intensity text...
    public override string Description => $"Increases Armor By {AffectedStat[EntityStats.Stat.Armor]*Intensity} for {Duration} Rounds";
    public override string IconPath { get; set; } = "Resistant_Icon.png";

    public override Dictionary<EntityStats.Stat, double> AffectedStat { get; set; } = new()
    {
        { EntityStats.Stat.Armor, 1.5 },
    };

    public override Effect Copy()
    {
        return new ResistantEffect() //susceptible to copy mistakes...
        {
            Intensity = Intensity,
            Duration = Duration,
        };
    }
}