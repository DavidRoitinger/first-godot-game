using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace FirstGodotGame;

public partial class HorseAction : EntityAction, IEntityMove
{
    public new async Task Move(EntityStats ownEntityStats, List<EntityStats> allEntityStats)
    {
        var distance = ownEntityStats.GridPosition.DistanceTo(
            allEntityStats.First(eS => eS.EntityType == EntityStats.Type.Player)
                .GridPosition);

        if (distance < 4) return;
        await base.Move(ownEntityStats, allEntityStats);
    }
}