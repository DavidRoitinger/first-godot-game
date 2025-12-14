using System.Collections.Generic;
using Godot;

namespace FirstGodotGame;

public class UpgradePool
{
    private static UpgradePool _instance;
    public static UpgradePool Instance
    {
        get
        {
            _instance ??= new UpgradePool();
            return _instance;
        }
    }
    private UpgradePool()
    {
    }
    

    public readonly List<Upgrade> CommonUpgradePool =
    [
        new StatUpgrade()
        {
            Name = "BasicSpeed",
            Description = "Increases Speed",
            UpgradeQuality = Upgrade.Quality.Common,
            AdditionalSpeed = 1,
        },
        new StatUpgrade()
        {
            Name = "BasicArmor",
            Description = "Increases Armor by 1",
            UpgradeQuality = Upgrade.Quality.Common,
            AdditionalArmor = 1,
        },
        new StatUpgrade()
        {
            Name = "BasicHealth",
            Description = "Increases Health by 1",
            UpgradeQuality = Upgrade.Quality.Common,
            AdditionalMaxHealth = 1,
        },
        
        new AttackUpgrade()
        {
            Name = "LauncherAttack",
            Description = "Adds a new Attack",
            UpgradeQuality = Upgrade.Quality.Common,
            AdditionalAttacks = 
            [
                1001,
            ]
        },
        new AttackUpgrade()
        {
            Name = "FistAttack",
            Description = "Adds a new Attack",
            UpgradeQuality = Upgrade.Quality.Common,
            AdditionalAttacks = 
            [
                1002,
            ]
        },
        new AttackUpgrade()
        {
            Name = "FenceAttack",
            Description = "Adds a new Attack",
            UpgradeQuality = Upgrade.Quality.Common,
            AdditionalAttacks = 
            [
                1003,
            ]
        },
        
    ];
}