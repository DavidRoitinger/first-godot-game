using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using static FirstGodotGame.Attack;

namespace FirstGodotGame;

public partial class EntityAction : Node, IEntityAttack, IEntityMove, IEntityDie
{
    protected TileMapLayer _groundLayer 
    {
        get
        {
            return GetTree()
                .GetNodesInGroup("Tilemap")
                .First(node => node.Name == "GroundLayer") as TileMapLayer;
        }
    }
    
    protected TileMapLayer _highlightLayer
    {
        get
        {
            return GetTree()
                .GetNodesInGroup("Tilemap")
                .First(node => node.Name == "HighlightLayer") as TileMapLayer;
        }
    }
    
    
    // private List<Attack> _attacks;
    // public List<Attack> Attacks
    // {
    //     get
    //     {
    //         if (_attacks != null) return _attacks;
    //         
    //         _attacks = GetChildren()
    //             .Where(x => x is IAttack)
    //             .Select(x => ((IAttack)x).GetAttack())
    //             .ToList();
    //         _attacks.AddRange(GetChildren()
    //             .Where(x => x is IAttacks)
    //             .Select(x => ((IAttacks)x).GetAttacks())
    //             .Aggregate(new List<Attack>(),(x1, x2) => [..x1,..x2]));
    //         _attacks = _attacks.OrderBy(x => x.Name).ToList();
    //         return _attacks;
    //     }
    // }

    

    public async Task Attack(EntityStats ownEntityStats, List<EntityStats> allEntityStats)
    {
        var attack = GetRandomAttack(ownEntityStats);

        var possibleAttackOrigins = await HighlightPattern(
            ownEntityStats.GridPosition,
            attack.OriginPattern,
            new Vector2I(1, 0));
        
        await Task.Delay(500/TurnSkipper.SpeedUpTurn);
        

        Vector2I? closestAttackPosition = FindClosestToPlayer(allEntityStats, possibleAttackOrigins);

        if(!closestAttackPosition.HasValue) return;
        
        List<Vector2I> attackedTiles = await FireAttack(closestAttackPosition.Value, attack, ownEntityStats.GridPosition);

        if (DistributeDamage(ownEntityStats, allEntityStats, attackedTiles, attack) >= 1)
        {
            ApplyKnockback(closestAttackPosition.Value, attack.UserKnockback, ownEntityStats);
            SoundManager.Instance.PlaySfx(SoundManager.Shot);
        }
        else
        {
            SoundManager.Instance.PlaySfx(SoundManager.Miss);
        }
        ownEntityStats.PlayAnimation("Action");
    }

    protected int DistributeDamage(EntityStats ownEntityStats, List<EntityStats> allEntityStats, List<Vector2I> attackedTiles, Attack attack)
    {
        var hitEnities = allEntityStats
            .Where(stats =>
                stats.EntityType != ownEntityStats.EntityType &&
                attackedTiles.Contains(stats.GridPosition))
            .ToList();
        
        hitEnities.ForEach(hitEntityStats =>
            {
                ApplyKnockback(ownEntityStats.GridPosition, attack.TargetKnockback, hitEntityStats);
                
                hitEntityStats.TakeDamage(attack.Damage);
            });
        
        return hitEnities.Count;
    }

    protected void ApplyKnockback(Vector2I target, int knockbackStrength, EntityStats entityStats)
    {
        if(knockbackStrength <= 0) return;
        
        var possibleKnockbackPositions =
            GetPossibleMovePositions(knockbackStrength, entityStats.GridPosition, true)
                .ToList();
                
        entityStats.GridPosition = FindFurthestFromPosition(target, possibleKnockbackPositions);
    }

    protected async Task<List<Vector2I>> FireAttack(Vector2I attackPosition, Attack attack, Vector2I entityPosition)
    {
        List<Vector2I> attackedTiles = [];

        var pattern = FindAttackPatternDirection(attackPosition, attack, entityPosition);

        var attackOrigin = FindValueInPattern(pattern, OR).First();
        
        foreach (var coordinate in (List<Vector2I>)[..FindValueInPattern(pattern, OR), ..FindValueInPattern(pattern, NO)])
        {
            var atkPos = coordinate - attackOrigin + attackPosition;
            attackedTiles.Add(atkPos);
                    
            _highlightLayer.SetCell(atkPos, 1, new Vector2I(2,0));
            await Task.Delay(10/TurnSkipper.SpeedUpTurn);
        }
        return attackedTiles;
    }

    protected List<List<int>> FindAttackPatternDirection(Vector2I attackPosition, Attack attack, Vector2I entityPosition)
    {
        var attackOrigin = FindValueInPattern(attack.OriginPattern, OR).First();
        
        int x = attackPosition.X + attackOrigin.X - entityPosition.X;
        int y = attackPosition.Y + attackOrigin.Y - entityPosition.Y;
        
        if (attack.OriginPattern.Count <= x ||
            attack.OriginPattern[0].Count <= y||
            x < 0 ||
            y < 0) return attack.NeutralAttackPattern;
        
        List<List<int>> pattern = attack.OriginPattern[x][y] switch
        {
            NO => attack.NeutralAttackPattern,
            UO => attack.UpAttackPattern,
            RO => attack.RightAttackPattern,
            DO => attack.DownAttackPattern,
            LO => attack.LeftAttackPattern,
            _ => attack.NeutralAttackPattern
        };
        return pattern;
    }

    protected Attack GetRandomAttack(EntityStats ownEntityStats)
    {
        var rand = GD.RandRange(0, ownEntityStats.Attacks.Count-1);
        var attack = ownEntityStats.Attacks[rand];
        return attack;
    }

    protected async Task<List<Vector2I>> HighlightPattern(Vector2I gridPosition, List<List<int>> pattern, Vector2I markerAtlasCoords)
    {

        var attackOrigin = FindValueInPattern(pattern, OR).First();

        _highlightLayer.Clear();

        List<Vector2I> possibleAttackOrigins = new List<Vector2I>();

        foreach (var coordinate in (Vector2I[])[
                     ..FindValueInPattern(pattern, NO),
                     ..FindValueInPattern(pattern, UO),
                     ..FindValueInPattern(pattern, RO),
                     ..FindValueInPattern(pattern, DO),
                     ..FindValueInPattern(pattern, LO)])
        {
            var atkPos = coordinate - attackOrigin + gridPosition;
                
            if (_groundLayer.GetCellTileData(atkPos) == null) continue;
                
            possibleAttackOrigins.Add(atkPos);
            _highlightLayer.SetCell(atkPos, 1, markerAtlasCoords);
            await Task.Delay(10/TurnSkipper.SpeedUpTurn);
        }

        return possibleAttackOrigins;
    }

    protected List<Vector2I> FindValueInPattern(List<List<int>> pattern, int value)
    {
        List<Vector2I> foundCoordinates = [];
        for (int x = 0; x < pattern.Count; x++)
        {
            for (int y = 0; y < pattern[0].Count; y++)
            {
                if (pattern[x][y] != value) continue;
                var foundCoordinate = new Vector2I(x, y);
                foundCoordinates.Add(foundCoordinate);
            }
        } 
        
        return foundCoordinates;
    }

    protected HashSet<Vector2I> GetPossibleMovePositions(int distance, Vector2I start, bool isKnockback)
    {
        var results = new HashSet<Vector2I>();
        var q = new Queue<(Vector2I pos, int remaining)>();
        q.Enqueue((start, distance));

        while (q.Count > 0)
        {
            var (pos, remaining) = q.Dequeue();
            if (remaining == 0) continue;
            
            foreach (var n in isKnockback ? Get8Neighbors(pos) : Get4Neighbors(pos))
            {
                if (results.Contains(n)) continue;                                      // already discovered at same or better level
                if (!isKnockback && _groundLayer.GetCellTileData(n) == null) continue;   // blocked

                results.Add(n);
                q.Enqueue((n, remaining - 1));
            }
        }

        return results;
    }
    
    protected IEnumerable<Vector2I> Get4Neighbors(Vector2I p)
    {
        yield return p + Vector2I.Up;
        yield return p + Vector2I.Down;
        yield return p + Vector2I.Left;
        yield return p + Vector2I.Right;
    }
    protected IEnumerable<Vector2I> Get8Neighbors(Vector2I p)
    {
        yield return p + Vector2I.Up;
        yield return p + Vector2I.Down;
        yield return p + Vector2I.Left;
        yield return p + Vector2I.Right;
        
        yield return p + Vector2I.Up + Vector2I.Left;
        yield return p + Vector2I.Up + Vector2I.Right;
        yield return p + Vector2I.Down + Vector2I.Left;
        yield return p + Vector2I.Down + Vector2I.Right;
    }


    public async Task Move(EntityStats ownEntityStats, List<EntityStats> allEntityStats)
    {
        for (int i = 0; i < ownEntityStats.Speed; i++)
        {
            await Task.Delay(250/TurnSkipper.SpeedUpTurn);
            MoveStep(ownEntityStats, allEntityStats);
        }

        
    }

    protected void MoveStep(EntityStats ownEntityStats,  List<EntityStats> allEntityStats)
    {
        List<Vector2I> moveOptions = [];
		
        for (int x = ownEntityStats.GridPosition.X - 1; x <= ownEntityStats.GridPosition.X + 1; x++)
        {
            for (int y = ownEntityStats.GridPosition.Y - 1; y <= ownEntityStats.GridPosition.Y + 1; y++)
            {
                var pos = new Vector2I(x, y);
				
                if(pos == ownEntityStats.GridPosition) continue; // Starting Tile
                if(_groundLayer.GetCellTileData(pos) == null) continue; // Empty Tile
                if(allEntityStats.Any(eS => eS.GridPosition == pos)) continue; // Used Tile
				
                moveOptions.Add(pos);
				
                // _highlightLayer.SetCell(pos, 1, new Vector2I(2,0));
                // GD.Print($"X:{x};Y:{y}");
            }
        }
		
        GD.Print(ownEntityStats.GridPosition);
        
        
        ownEntityStats.GridPosition = FindClosestToPlayer(allEntityStats, moveOptions) ?? ownEntityStats.GridPosition;
    }

    protected static Vector2I? FindClosestToPlayer(List<EntityStats> allEntityStats, List<Vector2I> tiles)
    {
         return tiles.MinBy(
            pos => 
                pos.DistanceTo(
                    allEntityStats
                        .Where(eS => 
                            eS.EntityType == EntityStats.Type.Player && eS.Health > 0)
                        .MinBy(eS => pos.DistanceTo(eS.GridPosition))
                        .GridPosition)
                );
    }
    protected static Vector2I FindFurthestFromPosition(Vector2I target, List<Vector2I> tiles)
    {
         return tiles.MaxBy(
            pos => 
                pos.DistanceTo(target)
                );
    }


    

    public void Die(EntityStats entityStats)
    {
        if (GetParent().GetChildren().FirstOrDefault(x => x is Sprite2D) is Sprite2D sprite)
        {
            sprite.RotationDegrees = 90;

        }

    }
}