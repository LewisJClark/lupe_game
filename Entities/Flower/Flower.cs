using Godot;
using System;

public class Flower : Entity
{
    public event Action AllPetalsRemoved;
    
    [Export] private PackedScene _petalScene;
    [Export] private int _numberOfPetals = 3;

    private Node2D _petals;
    
    public override void _Ready()
    {
        _petals = GetNode<Node2D>("Petals");
        
        float petalSeparation = 360 /  (float)_numberOfPetals;
        for (int i = 0; i < _numberOfPetals; i++)
        {
            var newPetal = _petalScene.Instance<Node2D>();
            newPetal.RotationDegrees = i * petalSeparation;
            _petals.AddChild(newPetal);           
        }
    }

    public void RemovePetal()
    {
        _numberOfPetals--;
        _petals.GetChild(0).QueueFree();
        
        if (_numberOfPetals == 0)
            AllPetalsRemoved?.Invoke();
    }

}
