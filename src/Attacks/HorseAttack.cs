using Godot;

namespace FirstGodotGame.Attacks;

public partial class HorseAttack: Node, IAttack
{
    public Attack GetAttack()
    {
        
        return new Attack
        {
            Name = "Launcher",
            Damage = 1,
            OriginPattern = [
                [2,2,2,2,2,2,2],
                [2,0,0,0,0,0,2],
                [2,0,0,0,0,0,2],
                [2,0,0,1,0,0,2],
                [2,0,0,0,0,0,2],
                [2,0,0,0,0,0,2],
                [2,2,2,2,2,2,2],
            ],
            AttackPattern = [
                [2,2,2],
                [2,1,2],
                [2,2,2],
            ],
        };
        
    }
}