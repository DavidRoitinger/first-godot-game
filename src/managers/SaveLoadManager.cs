using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;

namespace FirstGodotGame;

//Todo: Check if saving is working and perhaps implement HandleUnknownType...

public class SaveLoadManager
{
    public static void Save(SceneTree sceneTree)
    {
        var playerStats = sceneTree.GetNodesInGroup("Player")
            .Select( node => node.GetChildren().OfType<EntityStats>().First())
            .Select( stats => new Stats
            (
                EntityName: stats.EntityName,
                MaxHealth: stats.MaxHealth,
                Armor: stats.Armor,
                Speed: stats.Speed,
                Attacks: stats.Attacks.ToArray(),
                UpgradeIds: stats.UpgradeIds.ToArray()
            ))
            .ToList();
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        string jsonString = JsonSerializer.Serialize(playerStats, options);

        if (!Directory.Exists("save")) Directory.CreateDirectory("save");
        
        File.WriteAllText("save/players.json", jsonString);
    }
    
    public static void Load(SceneTree sceneTree)
    {
        if (!File.Exists("save/players.json")) return;
        string jsonString = File.ReadAllText("save/players.json");

        var playerStats = sceneTree.GetNodesInGroup("Player")
            .Select(node => node.GetChildren().OfType<EntityStats>().First())
            .ToList();
        
        var statList = JsonSerializer.Deserialize<List<Stats>>(jsonString);

        for (int i = 0; i < playerStats.Count() && i < statList.Count(); i++)
        {
            playerStats[i].EntityName = statList[i].EntityName;
            playerStats[i].MaxHealth = statList[i].MaxHealth;
            playerStats[i].Armor = statList[i].Armor;
            playerStats[i].Speed = statList[i].Speed;
            playerStats[i].ClearAttacks();
            playerStats[i].AddAttackList(statList[i].Attacks);
            playerStats[i].UpgradeIds.AddRange(statList[i].UpgradeIds);
        }
    }

    private record Stats(string EntityName, int MaxHealth, int Armor, int Speed, Attack[] Attacks, int[] UpgradeIds);

}