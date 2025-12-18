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

    private EntityStats _playerStats;
    private Attack _currentAttack;
    private int _attackIndex;

    public new async Task Attack(EntityStats ownEntityStats, List<EntityStats> allEntityStats)
    {
        _attackIndex = 0;
        _playerStats = ownEntityStats;

        Attack attack;
        Vector2I currentMousePosition;
        
        while (true)
        {
            attack = GetAttackByIndex(_attackIndex, ownEntityStats);

            var possibleAttackOrigins =
                await HighlightPattern(
                    ownEntityStats.GridPosition,
                    attack.OriginPattern,
                    new Vector2I(1, 0));

            _currentAttack = attack;

            
            
            string input = await WaitForAction();
            if (input == "Left")
            {
                currentMousePosition = 
                    _groundLayer.LocalToMap(_groundLayer.ToLocal(GetViewport().GetCamera2D().GetGlobalMousePosition())); 
                if (possibleAttackOrigins.Contains(currentMousePosition)) break;
            }
            else if (input == "Right")
            {
                _attackIndex++;
                if (_attackIndex >= ownEntityStats.Attacks.Count)
                {
                    _attackIndex = 0;
                }
            }
        }

        _currentAttack = null;
        CursorLayer.Clear();
        
        List<Vector2I> attackedTiles = await FireAttack(currentMousePosition, attack, ownEntityStats.GridPosition);

        if (DistributeDamage(ownEntityStats, allEntityStats, attackedTiles, attack) >= 1)
        {
            ApplyKnockback(currentMousePosition, attack.UserKnockback, ownEntityStats);
            SoundManager.Instance.PlaySfx(SoundManager.Shot);
        }
        else
        {
            SoundManager.Instance.PlaySfx(SoundManager.Miss);
        }
        
        ownEntityStats.PlayAnimation("Action");

        await Task.Delay(2000/TurnSkipper.SpeedUpTurn);
    }
    
    protected Attack GetAttackByIndex(int index, EntityStats ownEntityStats)
    {
        var attacks = ownEntityStats.Attacks;
        var attack = attacks[index];
        return attack;
    }


    public new async Task Move(EntityStats ownEntityStats, List<EntityStats> allEntityStats)
    {
        var possibleMovePositions =
            GetPossibleMovePositions(ownEntityStats.Speed, ownEntityStats.GridPosition, false)
                .Where(x => allEntityStats.Count(eS => eS.GridPosition == x) == 0)
                .ToList();

        foreach (var possibleMovePosition in possibleMovePositions)
        {
            await Task.Delay(10/TurnSkipper.SpeedUpTurn);
            _highlightLayer.SetCell(possibleMovePosition, 1, new Vector2I(1, 0));
        }

        while (true)
        {
            if(await WaitForAction() != "Left") continue;
            
            var currentMousePosition =
                _groundLayer.LocalToMap(_groundLayer.ToLocal(GetViewport().GetCamera2D().GetGlobalMousePosition()));
            if (possibleMovePositions.Contains(currentMousePosition))
            {
                ownEntityStats.GridPosition = currentMousePosition;
                break;
            }
        }
        
        await Task.Delay(2000/TurnSkipper.SpeedUpTurn);
    }

    protected void PreviewPattern(Vector2I gridPosition, Vector2I markerAtlasCoords)
    {

        List<List<int>> pattern = FindAttackPatternDirection(gridPosition, _currentAttack, _playerStats.GridPosition);
        
        var patternOrigin = FindValueInPattern(_currentAttack.OriginPattern, OR).First();
        
        var originPattern = ((Vector2I[])[
                ..FindValueInPattern(_currentAttack.OriginPattern, NO),
                ..FindValueInPattern(_currentAttack.OriginPattern, UO),
                ..FindValueInPattern(_currentAttack.OriginPattern, RO),
                ..FindValueInPattern(_currentAttack.OriginPattern, DO),
                ..FindValueInPattern(_currentAttack.OriginPattern, LO),
            ])
            .Select(x => x - patternOrigin + _playerStats.GridPosition);

        var attackOrigin = FindValueInPattern(pattern, OR).First();

        CursorLayer.Clear();
        
        if (_groundLayer.GetCellTileData(gridPosition) == null) return;
        if (!originPattern
            .Contains(gridPosition)) return;

        foreach (var coordinate in (List<Vector2I>)
                 [..FindValueInPattern(pattern, OR),
                     ..FindValueInPattern(pattern, NO)])
        {
            var atkPos = coordinate - attackOrigin + gridPosition;
            if (_groundLayer.GetCellTileData(atkPos) == null) continue;
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
        else if (@event is InputEventMouseMotion && _currentAttack != null)
        {
            PreviewPattern(
                CursorLayer.LocalToMap(_groundLayer.ToLocal(GetViewport().GetCamera2D().GetGlobalMousePosition())),
                new Vector2I(0, 0));
        }
    }
}