using Godot;
using System;

public class Thorn : Obstacle
{
    public override void _Ready()
    {
        var sprite = GetNode<Node2D>("Sprite");

        sprite.Scale = new Vector2(0.5f, 1.5f);

        var spawnTween = CreateTween().SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.Out);
        spawnTween.TweenProperty(sprite, "scale", Vector2.One, 0.4f);
    }

}
