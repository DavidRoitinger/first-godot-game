using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using static FirstGodotGame.Attack;

namespace FirstGodotGame;

public partial class PlayerAction : EntityAction, IEntityMove , IEntityAttack
{
    
    
    public new async Task Attack(EntityStats ownEntityStats, List<EntityStats> allEntityStats)
    {
        var attack = GetRandomAttack();

        var possibleAttackOrigins = await HighlightPossibleOrigins(ownEntityStats, attack);
        
      
        Vector2I currentMousePosition;
        while (true)
        {
            await WaitForAction();
            currentMousePosition = GroundLayer.LocalToMap(GroundLayer.ToLocal(GetViewport().GetCamera2D().GetGlobalMousePosition()));
            if (possibleAttackOrigins.Contains(currentMousePosition)) break;
        }
        
        List<Vector2I> attackedTiles = await FireAttack(currentMousePosition, attack);
        
        DistributeDamage(ownEntityStats, allEntityStats, attackedTiles, attack);
        
        await Task.Delay(2000);
    }

    
    public new async Task Move(EntityStats ownEntityStats, List<EntityStats> allEntityStats)
    {
        var possibleMovePositions = GetPossibleMovePositions(ownEntityStats.Speed, ownEntityStats.GridPosition).ToList();

        foreach (var possibleMovePosition in possibleMovePositions)
        {
            if(allEntityStats.Count(eS => eS.GridPosition == possibleMovePosition) > 0) continue;
            await Task.Delay(10);
            HighlightLayer.SetCell(possibleMovePosition, 1, new Vector2I(1, 0));
        }

        while (true)
        {
            await WaitForAction();
            GD.Print("!");
            var currentMousePosition = GroundLayer.LocalToMap(GroundLayer.ToLocal(GetViewport().GetCamera2D().GetGlobalMousePosition()));
        
            if (possibleMovePositions.Contains(currentMousePosition))
            {
                ownEntityStats.GridPosition = currentMousePosition;
                break;
            }
        }
        
        await Task.Delay(2000);
    }
    
    
    
    

    private HashSet<Vector2I> GetPossibleMovePositions(int distance, Vector2I position)
    {
        var neighbors = GetNeighbors(position)
            .Where(x => GroundLayer.GetCellTileData(x) != null)
            .ToHashSet();
        if (distance <= 1) return neighbors;
        
        HashSet<Vector2I> positions = [];
        foreach (var neighbor in neighbors)
        {
            positions.UnionWith(GetPossibleMovePositions(distance - 1, neighbor));
        }

        return positions;
    }

    private HashSet<Vector2I> GetNeighbors(Vector2I position)
    {
        HashSet<Vector2I> neighbors = [];
        for (int x = position.X - 1; x <= position.X + 1; x++)
        {
            for (int y = position.Y - 1; y <= position.Y + 1; y++)
            {
               neighbors.Add(new Vector2I(x, y));
            }
        }

        return neighbors;
    }
    
    private TaskCompletionSource<bool> _clickTcs;
    
    private async Task WaitForAction()
    {
        _clickTcs = new TaskCompletionSource<bool>();
        await _clickTcs.Task;
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Left_Click"))
        {
            _clickTcs?.TrySetResult(true);
        }
    }
}