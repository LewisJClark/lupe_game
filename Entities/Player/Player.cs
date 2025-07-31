using Godot;
using System;

public class Player : Node2D
{
    public override void _Ready()
    {
        
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event.IsActionPressed("move_left"))
            Level.Current.Move(this, Vector2.Left);
        else if (@event.IsActionPressed("move_right"))
            Level.Current.Move(this, Vector2.Right);
        else if (@event.IsActionPressed("move_up"))
            Level.Current.Move(this, Vector2.Up);
        else if (@event.IsActionPressed("move_down"))
            Level.Current.Move(this, Vector2.Down);
    }
    
}
