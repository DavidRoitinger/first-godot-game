using System.Collections.Generic;
using Godot;

namespace FirstGodotGame.Attacks;

public partial class TestAttacks: Node, IAttacks
{
    public List<Attack> GetAttacks()
    {
        return [new Attack
        {
            Name = "Stab",
            Damage = 30,
            OriginPattern = [

                [2,2,2],
                [2,1,2],
                [2,2,2],

            ],
            AttackPattern = [
                [1],
            ],
        }, 
        new Attack
        {
            Name = "Fence",
            Damage = 20,
            OriginPattern = [
                [2,0,0,0,2],
                [0,2,0,2,0],
                [0,0,1,0,0],
                [0,2,0,2,0],
                [2,0,0,0,2],
            ],
            AttackPattern = [
                [2,0,2],
                [0,1,0],
                [2,0,2],
            ],
        },
        ];
    }
}