using System.Collections.Generic;

namespace FirstGodotGame;

public interface IAttack
{
    Attack GetAttack();
}
public interface IAttacks
{
    List<Attack> GetAttacks();
}