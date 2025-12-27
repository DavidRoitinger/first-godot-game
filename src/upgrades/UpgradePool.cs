using System.Collections.Generic;
using System.Linq;

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
    
    public Upgrade GetUpgradeById(int id)
    {
        return  ((Upgrade[])[..CommonUpgradePool, ..StrangeUpgradePool, ..BizarreUpgradePool])
            .First(x => x.Id == id);
    }
    

    public readonly List<Upgrade> CommonUpgradePool =
    [
        new StatUpgrade()
        {
            Id = 2000,
            Name = "BasicSpeed",
            Description = "Increases Speed by 1",
            UpgradeQuality = Upgrade.Quality.Common,
            AdditionalSpeed = 1,
        },
        new StatUpgrade()
        {
            Id = 2001,
            Name = "BasicArmor",
            Description = "Increases Armor by 1",
            UpgradeQuality = Upgrade.Quality.Common,
            AdditionalArmor = 1,
        },
        new StatUpgrade()
        {
            Id = 2002,
            Name = "BasicHealth",
            Description = "Increases Health by 1",
            UpgradeQuality = Upgrade.Quality.Common,
            AdditionalMaxHealth = 1,
        },
        
        new AttackUpgrade()
        {
            Id = 2003,
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
            Id = 2004,
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
            Id = 2005,
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
            Id = 2100,
            Name = "BasicSpeed+",
            Description = "Increases Speed by 2",
            UpgradeQuality = Upgrade.Quality.Strange,
            AdditionalSpeed = 2,
        },
        new StatUpgrade()
        {
            Id = 2101,
            Name = "BasicAmor+",
            Description = "Increases Armor by 2",
            UpgradeQuality = Upgrade.Quality.Strange,
            AdditionalArmor = 2,
        },
        new StatUpgrade()
        {
            Id = 2102,
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
            Id = 2200,
            Name = "BasicSpeed++",
            Description = "Increases Speed by 3",
            UpgradeQuality = Upgrade.Quality.Bizarre,
            AdditionalSpeed = 3,
        },
        new StatUpgrade()
        {
            Id = 2201,
            Name = "BasicAmor++",
            Description = "Increases Armor by 3",
            UpgradeQuality = Upgrade.Quality.Bizarre,
            AdditionalArmor = 3,
        },
        new StatUpgrade()
        {
            Id = 2202,
            Name = "BasicHealth++",
            Description = "Increases Health by 3",
            UpgradeQuality = Upgrade.Quality.Bizarre,
            AdditionalMaxHealth = 3,
        },

    ];
}
