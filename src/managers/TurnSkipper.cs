using Godot;

namespace FirstGodotGame;

public partial class TurnSkipper : Node
{
    public static int SpeedUpTurn = 1;
    
    [Export] public Texture2D OnIcon { get; set; }
    [Export] public Texture2D OffIcon { get; set; }
    
    private int _skipSpeed = 10;

    private bool _isInRushMode;
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Turn_Speed_Up"))
        {
            SpeedUpTurn = _isInRushMode ? 1 : _skipSpeed;

            if (GetTree().GetFirstNodeInGroup("RushModeIcon") is TextureRect textureRect)
                textureRect.Texture = !_isInRushMode ? OnIcon : OffIcon;

            _isInRushMode = !_isInRushMode;
        }
    }
}