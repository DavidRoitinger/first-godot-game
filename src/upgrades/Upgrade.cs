using Godot;

namespace FirstGodotGame;

public abstract partial class Upgrade : Resource
{
    [Export] public string Name { get; set; }
    [Export] public string Description { get; set; }
    [Export] public Texture Icon { get; set; }
    [Export] public Quality UpgradeQuality { get; set; }
    
    public abstract void ApplyUpgrade(EntityStats playerStats);
    

    public enum Quality
    {
        Ass,
        Common,
        Cool
    }
}