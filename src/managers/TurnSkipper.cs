using Godot;

namespace FirstGodotGame;

public partial class TurnSkipper : Node
{
    public static int SpeedUpTurn = 1;
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Turn_Speed_Up"))
        {
            SpeedUpTurn = 10;
        }
        else if(@event.IsActionReleased("Turn_Speed_Up"))
        {
            SpeedUpTurn = 1;
        }
    }
}