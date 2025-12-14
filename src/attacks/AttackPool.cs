using System.Collections.Generic;
using System.Linq;
using static FirstGodotGame.Attack;

namespace FirstGodotGame;

public class AttackPool
{
    private static AttackPool _instance;
    public static AttackPool Instance
    {
        get
        {
            _instance ??= new AttackPool();
            return _instance;
        }
    }
    private AttackPool()
    {
    }

    public Attack GetAttackById(int id)
    {
        return Attacks
            .Single(x => x.Id == id);
    }

    public readonly List<Attack> Attacks =
    [
        new Attack()    //Gun
        {
            Id = 1000,
            Name = "Gun",
            Damage = 2,
            TargetKnockback = 0,
            UserKnockback = 0,
            OriginPattern =
            [
                [NA, NA, NA, NA, NA],
                [NA, NO, UO, NO, NA],
                [NA, LO, OR, RO, NA],
                [NA, NO, DO, NO, NA],
                [NA, NA, NA, NA, NA],
            ],
            NeutralAttackPattern = 
            [
                [OR],
            ],
            UpAttackPattern = 
            [
                [NO],
                [NO],
                [OR],
            ],
            RightAttackPattern = 
            [
                [OR, NO, NO],
            ],
            DownAttackPattern = 
            [
                [OR],
                [NO],
                [NO],
            ],
            LeftAttackPattern = 
            [
                [NO, NO, OR],
            ],
        },
        new Attack()    //Launcher
        {
            Id = 1001,
            Name = "Launcher",
            Damage = 1,
            UserKnockback = 0,
            TargetKnockback = 1,
            OriginPattern = [
                [2,2,2,2,2,2,2],
                [2,0,0,0,0,0,2],
                [2,0,0,0,0,0,2],
                [2,0,0,1,0,0,2],
                [2,0,0,0,0,0,2],
                [2,0,0,0,0,0,2],
                [2,2,2,2,2,2,2],
            ],
            NeutralAttackPattern = [
                [2,2,2],
                [2,1,2],
                [2,2,2],
            ],
        },
        new Attack()    //Stab
        {
            Id = 1002,
            Name = "Stab",
            Damage = 1,
            UserKnockback = 1,
            TargetKnockback = 3,
            OriginPattern = [

                [2,2,2],
                [2,1,2],
                [2,2,2],

            ],
            NeutralAttackPattern = [
                [1],
            ],
        }, 
        new Attack()    //Fence
        {
            Id = 1003,
            Name = "Fence",
            Damage = 2,
            UserKnockback = 0,
            TargetKnockback = 2,
            OriginPattern = [
                [2,0,0,0,2],
                [0,2,0,2,0],
                [0,0,1,0,0],
                [0,2,0,2,0],
                [2,0,0,0,2],
            ],
            NeutralAttackPattern = [
                [2,0,2],
                [0,1,0],
                [2,0,2],
            ],
        },
    ];
}