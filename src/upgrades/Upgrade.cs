using Godot;

namespace FirstGodotGame;

public abstract partial class Upgrade
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }  = "Generic_Icon.png";
    public Quality UpgradeQuality { get; set; }
    
    public abstract void ApplyUpgrade(EntityStats playerStats);
    

    public enum Quality
    {
        Common,
        Strange,
        Bizarre
    }
}