using System.Collections.Generic;

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
            Description = "Increases Speed by 1",
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
    
    public readonly List<Upgrade> StrangeUpgradePool =
    [
        new StatUpgrade()
        {
            Name = "BasicSpeed+",
            Description = "Increases Speed by 2",
            UpgradeQuality = Upgrade.Quality.Strange,
            AdditionalSpeed = 2,
        },
        new StatUpgrade()
        {
            Name = "BasicAmor+",
            Description = "Increases Armor by 2",
            UpgradeQuality = Upgrade.Quality.Strange,
            AdditionalArmor = 2,
        },
        new StatUpgrade()
        {
            Name = "BasicHealth+",
            Description = "Increases Health by 2",
            UpgradeQuality = Upgrade.Quality.Strange,
            AdditionalMaxHealth = 2,
        },

    ];
    
    public readonly List<Upgrade> BizarreUpgradePool =
    [
        new StatUpgrade()
        {
            Name = "BasicSpeed++",
            Description = "Increases Speed by 3",
            UpgradeQuality = Upgrade.Quality.Bizarre,
            AdditionalSpeed = 3,
        },
        new StatUpgrade()
        {
            Name = "BasicAmor++",
            Description = "Increases Armor by 3",
            UpgradeQuality = Upgrade.Quality.Bizarre,
            AdditionalArmor = 3,
        },
        new StatUpgrade()
        {
            Name = "BasicHealth++",
            Description = "Increases Health by 3",
            UpgradeQuality = Upgrade.Quality.Bizarre,
            AdditionalMaxHealth = 3,
        },

    ];
}