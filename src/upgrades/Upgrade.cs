using Godot;

namespace FirstGodotGame;

public abstract partial class Upgrade
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Texture Icon { get; set; }
    public Quality UpgradeQuality { get; set; }
    
    public abstract void ApplyUpgrade(EntityStats playerStats);
    

    public enum Quality
    {
        Common,
        Strange,
        Bizarre
    }
}