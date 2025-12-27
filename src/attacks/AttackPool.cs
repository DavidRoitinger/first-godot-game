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
        #region Standart_Attacks #######################################################################################
        
        new Attack()    //Weak Punch
        {
            Id = 1000,
            Name = "Weak Punch",
            Damage = 2,
            Buff = false,
            TargetKnockback = 0,
            UserKnockback = 0,
            TargetEffect = null,
            UserEffect = null,
            OriginPattern =
            [
                [NO, NO, NO],
                [NO, OR, NO],
                [NO, NO, NO],
            ],
            NeutralAttackPattern = 
            [
                [OR],
            ],
        },
        new Attack()    //Launcher
        {
            Id = 1001,
            Name = "Launcher",
            Damage = 1,
            Buff = false,
            UserKnockback = 0,
            TargetKnockback = 1,
            TargetEffect = new SlowEffect() 
            {
                Duration = 2,
                Intensity = 2,
            },
            UserEffect = null,
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
            Damage = 2,
            Buff = false,
            TargetKnockback = 0,
            UserKnockback = 0,
            TargetEffect = null,
            UserEffect = null,
            OriginPattern =
            [
                [NA, UO, NA],
                [LO, OR, RO],
                [NA, DO, NA],
            ],
            NeutralAttackPattern = 
            [
                [OR],
            ],
            UpAttackPattern = 
            [
                [NO],
                [NO],
                [NO],
                [OR],
            ],
            RightAttackPattern = 
            [
                [OR, NO, NO, NO],
            ],
            DownAttackPattern = 
            [
                [OR],
                [NO],
                [NO],
                [NO],
            ],
            LeftAttackPattern = 
            [
                [NO, NO, NO, OR],
            ],
        }, 
        new Attack()    //Fence
        {
            Id = 1003,
            Name = "Fence",
            Damage = 2,
            Buff = false,
            UserKnockback = 0,
            TargetKnockback = 2,
            TargetEffect = null,
            UserEffect = null,
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
        
        
        #endregion

        #region Buff_Attacks ###########################################################################################
        
        new Attack()    //Heal
        {
            Id = 1100,
            Name = "Heal",
            Damage = 2,
            Buff = true,
            UserKnockback = 0,
            TargetKnockback = 0,
            TargetEffect = new RegenEffect()
            {
                Intensity = 3,
                Duration = 2,
            },
            UserEffect = null,
            OriginPattern = [

                [NO, NO, NO],
                [NO, OR, NO],
                [NO, NO, NO],

            ],
            NeutralAttackPattern = [
                [OR],
            ],
        }, 

        #region Self Buff Attack #######################################################################################

        new Attack()    //WeakSelfHeal
        {
            Id = 1101,
            Name = "Weak Self Heal",
            Damage = 0,
            Buff = true,
            UserKnockback = 0,
            TargetKnockback = 0,
            TargetEffect = new RegenEffect()
            {
                Intensity = 2,
                Duration = 2,
            },
            UserEffect = null,
            OriginPattern = [
                [OA]
            ],
            NeutralAttackPattern = [
                [OR],
            ],
        }, 
        new Attack()    //WeakFocus
        {
            Id = 1102,
            Name = "WeakFocus",
            Damage = 0,
            Buff = true,
            UserKnockback = 0,
            TargetKnockback = 0,
            TargetEffect = new FocusEffect()
            {
                Intensity = 1,
                Duration = 2,
            },
            UserEffect = null,
            OriginPattern = [
                [OA]
            ],
            NeutralAttackPattern = [
                [OR],
            ],
        }, 
        new Attack()    //WeakResistance
        {
            Id = 1103,
            Name = "Weak Resistance",
            Damage = 0,
            Buff = true,
            UserKnockback = 0,
            TargetKnockback = 0,
            TargetEffect = new ResistantEffect()
            {
                Intensity = 1,
                Duration = 2,
            },
            UserEffect = null,
            OriginPattern = [
                [OA]
            ],
            NeutralAttackPattern = [
                [OR],
            ],
        }, 

        #endregion
        
        
        #endregion
    ];
}
