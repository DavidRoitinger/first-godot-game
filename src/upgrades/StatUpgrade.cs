using Godot;

namespace FirstGodotGame;

public partial class StatUpgrade : Upgrade
{
    [Export] public int AdditionalMaxHealth { get; set; }
    [Export] public int AdditionalArmor { get; set; }
    [Export] public int AdditionalSpeed { get; set; }
    
    public override void ApplyUpgrade(EntityStats playerStats)
    {
        playerStats.UpgradeIds.Add(Id);
        playerStats.MaxHealth += AdditionalMaxHealth;
        playerStats.Armor += AdditionalArmor;
        playerStats.Speed += AdditionalSpeed;
    }
}