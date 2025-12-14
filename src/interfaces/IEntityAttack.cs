using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirstGodotGame;

public interface IEntityAttack
{
    Task Attack(EntityStats ownEntityStats, List<EntityStats> allEntityStats);
}