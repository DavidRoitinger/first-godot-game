using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using static FirstGodotGame.Attack;

namespace FirstGodotGame;

public partial class EntityAction : Node, IEntityAttack, IEntityMove
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
        var attacks = GetAttacks();

        var attack = attacks.First();
        
        Vector2I attackOrigin = new Vector2I(
            attack.OriginPattern
                .FindIndex(x =>
                    x.Contains(AOR)),

            attack.OriginPattern
                .First(x =>
                    x.Contains(AOR))
                .FindIndex(x => x == AOR)
        );

        HighlightLayer.Clear();

        List<Vector2I> possibleAttackOrigins = new List<Vector2I>();
        
        for (int x = 0; x < attack.OriginPattern.Count; x++)
        {
            for (int y = 0; y < attack.OriginPattern[1].Count; y++)
            {
                if (attack.OriginPattern[x][y] == AOE)
                {
                    var atkPos = new Vector2I(x,y) - attackOrigin + ownEntityStats.GridPosition;
                    
                    if (GroundLayer.GetCellTileData(atkPos) == null) continue;
                    
                    possibleAttackOrigins.Add(atkPos);
                    
                    HighlightLayer.SetCell(atkPos, 1, new Vector2I(1,0));
                    
                }
            }
        }

        await Task.Delay(500);

        attackOrigin = new Vector2I(
            attack.AttackPattern
                .FindIndex(x =>
                    x.Contains(AOR)),

            attack.AttackPattern
                .First(x =>
                    x.Contains(AOR))
                .FindIndex(x => x == AOR)
        );
        
        
        Vector2I closestAttackPosition = FindClosestToPlayer(allEntityStats, possibleAttackOrigins);

        List<Vector2I> attackedTiles = new List<Vector2I>();
        
        HighlightLayer.SetCell(closestAttackPosition, 1, new Vector2I(3,0));
         
        
        for (int x = 0; x < attack.AttackPattern.Count; x++)
        {
            for (int y = 0; y < attack.AttackPattern[0].Count; y++)
            {
                if (attack.AttackPattern[x][y] != NOA)
                {
                    var atkPos = new Vector2I(x, y) - attackOrigin + closestAttackPosition;
                    attackedTiles.Add(atkPos);
                    
                    HighlightLayer.SetCell(atkPos, 1, new Vector2I(2,0));
        
                }
            }
        } 
        
    }

    private List<Attack> GetAttacks()
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

    private void MoveStep(EntityStats ownEntityStats,  List<EntityStats> allEntityStats)
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

    private Vector2I FindClosestToPlayer(List<EntityStats> allEntityStats, List<Vector2I> tiles)
    {
         return tiles.OrderBy(
            pos => 
                pos.DistanceTo(
                    allEntityStats.First(eS => 
                            eS.EntityType == EntityStats.Type.Player)
                        .GridPosition)
                ).First();
    }
}