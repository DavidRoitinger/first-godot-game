using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using static FirstGodotGame.Attack;

namespace FirstGodotGame;

public partial class PlayerAction : EntityAction, IEntityMove, IEntityAttack
{
    private TileMapLayer _cursorLayer;

    public TileMapLayer CursorLayer
    {
        get
        {
            _cursorLayer ??=
                GetTree().GetNodesInGroup("Tilemap").First(node => node.Name == "CursorLayer") as TileMapLayer;
            return _cursorLayer;
        }
    }

    private EntityStats playerStats;
    private Attack currentAttack = null;
    private int attackIndex = 0;

    public new async Task Attack(EntityStats ownEntityStats, List<EntityStats> allEntityStats)
    {
        attackIndex = 0;
        playerStats = ownEntityStats;

        Attack attack;
        Vector2I currentMousePosition;
        
        while (true)
        {
            attack = GetAttackByIndex(attackIndex);

            var possibleAttackOrigins =
                await HighlightPattern(
                    ownEntityStats.GridPosition,
                    attack.OriginPattern,
                    new Vector2I(1, 0));

            currentAttack = attack;

            
            
            string input = await WaitForAction();
            if (input == "Left")
            {
                currentMousePosition = 
                    GroundLayer.LocalToMap(GroundLayer.ToLocal(GetViewport().GetCamera2D().GetGlobalMousePosition())); 
                if (possibleAttackOrigins.Contains(currentMousePosition)) break;
            }
            else if (input == "Right")
            {
                attackIndex++;
                if (attackIndex >= Attacks.Count)
                {
                    attackIndex = 0;
                }
            }
        }

        currentAttack = null;
        CursorLayer.Clear();
        
        List<Vector2I> attackedTiles = await FireAttack(currentMousePosition, attack);

        DistributeDamage(ownEntityStats, allEntityStats, attackedTiles, attack);

        await Task.Delay(2000);
    }
    
    protected Attack GetAttackByIndex(int index)
    {
        var attacks = Attacks;
        var attack = attacks[index];
        return attack;
    }


    public new async Task Move(EntityStats ownEntityStats, List<EntityStats> allEntityStats)
    {
        var possibleMovePositions =
            GetPossibleMovePositions(ownEntityStats.Speed, ownEntityStats.GridPosition).ToList();

        foreach (var possibleMovePosition in possibleMovePositions)
        {
            if (allEntityStats.Count(eS => eS.GridPosition == possibleMovePosition) > 0) continue;
            await Task.Delay(10);
            HighlightLayer.SetCell(possibleMovePosition, 1, new Vector2I(1, 0));
        }

        while (true)
        {
            if(await WaitForAction() != "Left") continue;
            
            var currentMousePosition =
                GroundLayer.LocalToMap(GroundLayer.ToLocal(GetViewport().GetCamera2D().GetGlobalMousePosition()));

            if (possibleMovePositions.Contains(currentMousePosition))
            {
                ownEntityStats.GridPosition = currentMousePosition;
                break;
            }
        }

        await Task.Delay(2000);
    }
    private HashSet<Vector2I> GetPossibleMovePositions(int distance, Vector2I start)
    {
        var results = new HashSet<Vector2I>();
        var q = new Queue<(Vector2I pos, int remaining)>();
        q.Enqueue((start, distance));

        while (q.Count > 0)
        {
            var (pos, remaining) = q.Dequeue();
            if (remaining == 0) continue;

            foreach (var n in Get4Neighbors(pos))
            {
                if (results.Contains(n)) continue;                      // already discovered at same or better level
                if (GroundLayer.GetCellTileData(n) == null) continue;   // blocked

                results.Add(n);
                q.Enqueue((n, remaining - 1));
            }
        }

        return results;
    }

    // private HashSet<Vector2I> GetNeighbors(Vector2I position)
    // {
    //     HashSet<Vector2I> neighbors = [];
    //     for (int x = position.X - 1; x <= position.X + 1; x++)
    //     {
    //         for (int y = position.Y - 1; y <= position.Y + 1; y++)
    //         {
    //             neighbors.Add(new Vector2I(x, y));
    //         }
    //     }
    //
    //     return neighbors;
    // }
    private IEnumerable<Vector2I> Get4Neighbors(Vector2I p)
    {
        yield return p + Vector2I.Up;
        yield return p + Vector2I.Down;
        yield return p + Vector2I.Left;
        yield return p + Vector2I.Right;
    }

    protected void PreviewPattern(Vector2I gridPosition, Vector2I markerAtlasCoords)
    {
        var patternOrigin = FindValueInPattern(currentAttack.OriginPattern, AOR).First();
        
        var originPattern = FindValueInPattern(currentAttack.OriginPattern, AOE)
            .Select(x => x - patternOrigin + playerStats.GridPosition);

        var attackOrigin = FindValueInPattern(currentAttack.AttackPattern, AOR).First();

        CursorLayer.Clear();
        
        if (GroundLayer.GetCellTileData(gridPosition) == null) return;
        if (!originPattern
            .Contains(gridPosition)) return;

        foreach (var coordinate in (List<Vector2I>)
                 [..FindValueInPattern(currentAttack.AttackPattern, AOR),
                     ..FindValueInPattern(currentAttack.AttackPattern, AOE)])
        {
            var atkPos = coordinate - attackOrigin + gridPosition;
            if (GroundLayer.GetCellTileData(atkPos) == null) continue;
            CursorLayer.SetCell(atkPos, 1, markerAtlasCoords);
        }
    }

    private TaskCompletionSource<bool> _lClickTcs;
    private TaskCompletionSource<bool> _rClickTcs;

    private async Task<string>WaitForAction()
    {
        _lClickTcs = new TaskCompletionSource<bool>();
        _rClickTcs = new TaskCompletionSource<bool>();
        var input = await Task.WhenAny([_lClickTcs.Task, _rClickTcs.Task]);
        
        return input == _lClickTcs.Task ? "Left" : "Right";
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Left_Click"))
        {
            _lClickTcs?.TrySetResult(true);
        } else if (@event.IsActionPressed("Right_Click"))
        {
            _rClickTcs?.TrySetResult(true);
        }
        else if (@event is InputEventMouseMotion mouseEvent && currentAttack != null)
        {
            PreviewPattern(
                CursorLayer.LocalToMap(GroundLayer.ToLocal(GetViewport().GetCamera2D().GetGlobalMousePosition())),
                new Vector2I(0, 0));
        }
    }
}