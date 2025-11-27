using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using static FirstGodotGame.Attack;

namespace FirstGodotGame;

public partial class EntityAction : Node, IEntityAttack, IEntityMove, IEntityDie
{
    private TileMapLayer _groundLayer;
    public TileMapLayer GroundLayer
    {
        get
        {
            _groundLayer ??= GetTree()
                .GetNodesInGroup("Tilemap")
                .First(node => node.Name == "GroundLayer") as TileMapLayer;
            return _groundLayer;
        }
    }
    private TileMapLayer _highlightLayer;
    public TileMapLayer HighlightLayer
    {
        get
        {
            _highlightLayer ??= GetTree()
                .GetNodesInGroup("Tilemap")
                .First(node => node.Name == "HighlightLayer") as TileMapLayer;
            return _highlightLayer;
        }
    }
    
    private List<Attack> _attacks;
    public List<Attack> Attacks
    {
        get
        {
            if (_attacks != null) return _attacks;
            
            _attacks = GetChildren()
                .Where(x => x is IAttack)
                .Select(x => ((IAttack)x).GetAttack())
                .ToList();
            _attacks.AddRange(GetChildren()
                .Where(x => x is IAttacks)
                .Select(x => ((IAttacks)x).GetAttacks())
                .Aggregate(new List<Attack>(),(x1, x2) => [..x1,..x2]));
            _attacks = _attacks.OrderBy(x => x.Name).ToList();
            return _attacks;
        }
    }

    

    public async Task Attack(EntityStats ownEntityStats, List<EntityStats> allEntityStats)
    {
        var attack = GetRandomAttack();

        var possibleAttackOrigins = await HighlightPattern(
            ownEntityStats.GridPosition,
            attack.OriginPattern,
            new Vector2I(1, 0));
        
        await Task.Delay(500);
        

        Vector2I? closestAttackPosition = FindClosestToPlayer(allEntityStats, possibleAttackOrigins);

        if(!closestAttackPosition.HasValue) return;
        
        List<Vector2I> attackedTiles = await FireAttack(closestAttackPosition.Value, attack);
        
        DistributeDamage(ownEntityStats, allEntityStats, attackedTiles, attack);
        
    }

    protected static void DistributeDamage(EntityStats ownEntityStats, List<EntityStats> allEntityStats, List<Vector2I> attackedTiles, Attack attack)
    {
        allEntityStats
            .Where(stats => 
                stats.EntityType != ownEntityStats.EntityType && 
                attackedTiles.Contains(stats.GridPosition))
            .ToList()
            .ForEach(hitEntityStats => hitEntityStats.TakeDamage(attack.Damage));
    }

    protected async Task<List<Vector2I>> FireAttack(Vector2I attackPosition, Attack attack)
    {
        List<Vector2I> attackedTiles = [];
        var attackOrigin = FindValueInPattern(attack.AttackPattern, AOR).First();
        
        foreach (var coordinate in (List<Vector2I>)[..FindValueInPattern(attack.AttackPattern, AOR), ..FindValueInPattern(attack.AttackPattern, AOE)])
        {
            var atkPos = coordinate - attackOrigin + attackPosition;
            attackedTiles.Add(atkPos);
                    
            HighlightLayer.SetCell(atkPos, 1, new Vector2I(2,0));
            await Task.Delay(10);
        }
        return attackedTiles;
    }

    protected Attack GetRandomAttack()
    {
        var rand = GD.RandRange(0, Attacks.Count-1);
        var attack = Attacks[rand];
        return attack;
    }

    protected async Task<List<Vector2I>> HighlightPattern(Vector2I gridPosition, List<List<int>> pattern, Vector2I markerAtlasCoords)
    {

        var attackOrigin = FindValueInPattern(pattern, AOR).First();

        HighlightLayer.Clear();

        List<Vector2I> possibleAttackOrigins = new List<Vector2I>();

        foreach (var coordinate in FindValueInPattern(pattern, AOE))
        {
            var atkPos = coordinate - attackOrigin + gridPosition;
                
            if (GroundLayer.GetCellTileData(atkPos) == null) continue;
                
            possibleAttackOrigins.Add(atkPos);
            HighlightLayer.SetCell(atkPos, 1, markerAtlasCoords);
            await Task.Delay(10);
        }

        return possibleAttackOrigins;
    }

    protected static List<Vector2I> FindValueInPattern(List<List<int>> pattern, int value)
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




    public async Task Move(EntityStats ownEntityStats, List<EntityStats> allEntityStats)
    {
        for (int i = 0; i < ownEntityStats.Speed; i++)
        {
            await Task.Delay(250);
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
                if(GroundLayer.GetCellTileData(pos) == null) continue; // Empty Tile
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

    public void Die(EntityStats entityStats)
    {
        GD.Print($"Peter the {entityStats.EntityName} is here!");
        if (GetParent().GetChildren().FirstOrDefault(x => x is Sprite2D) is Sprite2D sprite)
        {
            sprite.RotationDegrees = 90;

        }
        

    }
}