using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FirstGodotGame;


//Todo: I am not sure if this works... test later...
public class FocusEffect : StatEffect
{
    public override string Name => "Focus";
    //Todo: Custom Intensity text...
    public override string Description => $"Increases Speed, Attack, Amor By {AffectedStat[EntityStats.Stat.Speed]*Intensity} for {Duration} Rounds";
    public override string IconPath { get; set; } = "Focus_Icon.png";
    
    public override Dictionary<EntityStats.Stat, double> AffectedStat { get; set; } = new()
    {
        { EntityStats.Stat.Speed, 1.25 },
        { EntityStats.Stat.Attack, 1.25 },
        { EntityStats.Stat.Armor, 1.25 },
    };
    
    public override Effect Copy()
    {
        return new FocusEffect() //susceptible to copy mistakes...
        {
            Intensity = Intensity,
            Duration = Duration,
        };
    }
}