using Godot;

namespace FirstGodotGame;

public partial class TurnSkipper : Node
{
    public static int SpeedUpTurn = 1;

    private static int _skipSpeed = 10;
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Turn_Speed_Up"))
        {
            SpeedUpTurn = SpeedUpTurn == _skipSpeed ? 1 : _skipSpeed;
        }
    }
}