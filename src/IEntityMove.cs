using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirstGodotGame;

public interface IEntityMove
{
    Task Move(EntityStats ownEntityStats, List<EntityStats> allEntityStats);
}