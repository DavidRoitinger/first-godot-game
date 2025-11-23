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
            _groundLayer ??= GetTree().GetNodesInGroup("Tilemap").First(node => node.Name == "GroundLayer") as TileMapLayer;
            return _groundLayer;
        }
    }
    private TileMapLayer _highlightLayer;
    public TileMapLayer HighlightLayer
    {
        get
        {
            _highlightLayer ??= GetTree().GetNodesInGroup("Tilemap").First(node => node.Name == "HighlightLayer") as TileMapLayer;
            return _highlightLayer;
        }
    }

    

    public async Task Attack(EntityStats ownEntityStats, List<EntityStats> allEntityStats)
    {
        var attack = GetRandomAttack();

        var possibleAttackOrigins = await HighlightPossibleOrigins(ownEntityStats, attack);
        
        await Task.Delay(500);
        

        Vector2I closestAttackPosition = FindClosestToPlayer(allEntityStats, possibleAttackOrigins);

        List<Vector2I> attackedTiles = await FireAttack(closestAttackPosition, attack);
        
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
        //Todo:Make Random?
        
        var attacks = GetAttacks();

        var attack = attacks.First();
        return attack;
    }

    protected async Task<List<Vector2I>> HighlightPossibleOrigins(EntityStats ownEntityStats, Attack attack)
    {

        var attackOrigin = FindValueInPattern(attack.OriginPattern, AOR).First();

        HighlightLayer.Clear();

        List<Vector2I> possibleAttackOrigins = new List<Vector2I>();

        foreach (var coordinate in FindValueInPattern(attack.OriginPattern, AOE))
        {
            var atkPos = coordinate - attackOrigin + ownEntityStats.GridPosition;
                
            if (GroundLayer.GetCellTileData(atkPos) == null) continue;
                
            possibleAttackOrigins.Add(atkPos);
            HighlightLayer.SetCell(atkPos, 1, new Vector2I(1,0));
            await Task.Delay(10);
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

    protected List<Attack> GetAttacks()
    {
        List<Attack> attacks = GetChildren()
            .Where(x => x is IAttack)
            .Select(x => ((IAttack)x).GetAttack())
            .ToList();
        attacks.AddRange(GetChildren()
            .Where(x => x is IAttacks)
            .Select(x => ((IAttacks)x).GetAttacks())
            .Aggregate(new List<Attack>(),(x1, x2) => [..x1,..x2]));
        return attacks;
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
        List<Vector2I> moveOptions = new List<Vector2I>();
		
        for (int x = ownEntityStats.GridPosition.X - 1; x <= ownEntityStats.GridPosition.X + 1; x++)
        {
            for (int y = ownEntityStats.GridPosition.Y - 1; y <= ownEntityStats.GridPosition.Y + 1; y++)
            {
                var pos = new Vector2I(x, y);
				
                if(pos == ownEntityStats.GridPosition) continue; // Starting Tile
                if(GroundLayer.GetCellTileData(pos) == null) continue; // Empty Tile
                if(allEntityStats.Count(eS => eS.GridPosition == pos) > 0) continue; // Used Tile
				
                moveOptions.Add(pos);
				
                // _highlightLayer.SetCell(pos, 1, new Vector2I(2,0));
                // GD.Print($"X:{x};Y:{y}");
            }
        }
		
        ownEntityStats.GridPosition = FindClosestToPlayer(allEntityStats, moveOptions);
    }

    protected Vector2I FindClosestToPlayer(List<EntityStats> allEntityStats, List<Vector2I> tiles)
    {
         return tiles.OrderBy(
            pos => 
                pos.DistanceTo(
                    allEntityStats.First(eS => 
                            eS.EntityType == EntityStats.Type.Player)
                        .GridPosition)
                ).First();
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