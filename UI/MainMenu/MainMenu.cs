using Godot;
using System;

public class MainMenu : Control
{
   [Export] private PackedScene _levelSelectScene;
   
   public override void _UnhandledInput(InputEvent @event)
   {
      base._UnhandledInput(@event);
      if (@event.IsActionPressed("restart"))
      {
         GetParent().AddChild(_levelSelectScene.Instance());
         QueueFree();
      }
   }
}
