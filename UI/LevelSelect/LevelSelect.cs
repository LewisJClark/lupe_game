using Godot;
using System;

using Array = Godot.Collections.Array;

public class LevelSelect : Control
{
    private VBoxContainer _buttonRoot;
    
    public override void _Ready()
    {
        _buttonRoot = GetNode<VBoxContainer>("ButtonRoot");

        for (int i = 0; i < Main.Instance.LevelData.Count; i++)
        {
            var button = new Button();
            button.Text = $"Level {i + 1}";
            button.Disabled = !Main.Instance.LevelData[i].Unlocked;
            button.Connect("pressed", this, nameof(OnLevelButtonPressed), new Array() { i });
            _buttonRoot.AddChild(button);
        }
    }

    private void OnLevelButtonPressed(int index)
    {
        Main.Instance.StartLevel(index);
        QueueFree();
    }

}
