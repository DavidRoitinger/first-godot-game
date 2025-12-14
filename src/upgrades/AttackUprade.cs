using Godot;

namespace FirstGodotGame;

public partial class AttackUpgrade : Upgrade 
{
    [Export] public int[] AdditionalAttacks { get; set; }
    
    public override void ApplyUpgrade(EntityStats playerStats)
    {
        playerStats.AddAttackListById(AdditionalAttacks);
    }
}