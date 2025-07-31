using Godot;
using System.Collections.Generic;

public class Player : Entity
{
    [Export] private PackedScene _thornScene;

    private Vector2 _spawnPosition;
    private SceneTreeTween _movementTween;

    private HashSet<Vector2> _visitedCells = new HashSet<Vector2>();

    public override void _Ready()
    {
        _spawnPosition = Position;
        _movementTween = null;
    }

    public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

        Vector2 direction = Vector2.Zero;

        if (@event.IsAction("move_left"))
            direction = Vector2.Left;
        else if (@event.IsAction("move_right"))
            direction = Vector2.Right;
        else if (@event.IsAction("move_up"))
            direction = Vector2.Up;
        else if (@event.IsAction("move_down"))
            direction = Vector2.Down;

        if (direction == Vector2.Zero) return;

        Move(direction);
	}

    private void Move(Vector2 direction)
    {
        if (_movementTween != null) return;

        var targetCellPosition = CellTransform.Position + direction;

        if (!Level.Current.IsCellValid(targetCellPosition, out var entityInCell))
        {
            StartBonkTween(Position + direction * CellTransform.CELL_SIZE * 0.5f);
            return;
        }

        if (entityInCell is Flower flower)
        {
            NextLoop();
            flower.RemovePetal();
            return;
        }

        StartMoveTween(CellTransform.CellToWorld(targetCellPosition));
        _visitedCells.Add(targetCellPosition);
    }

    private void NextLoop()
    {
        SpawnThorns();

        // TODO: cooler reset animation
        StartMoveTween(_spawnPosition);
    }

    private void StartMoveTween(Vector2 targetPosition)
    {
        _movementTween = CreateTween().SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _movementTween.TweenProperty(this, "position", targetPosition, 0.2f);
        _movementTween.TweenCallback(this, "FinishTween");
    }
    private void StartBonkTween(Vector2 targetPosition)
    {
        var initialPosition = Position;

        _movementTween = CreateTween();
        _movementTween.TweenProperty(this, "position", targetPosition, 0.1f);
        _movementTween.TweenProperty(this, "position", initialPosition, 0.15f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _movementTween.TweenCallback(this, "FinishTween");
    }
    private void FinishTween() => _movementTween = null;

    private void SpawnThorns()
    {
        foreach (var cell in _visitedCells)
        {
            var newThorn = _thornScene.Instance<Thorn>();
            newThorn.CellTransform.MoveTo(cell);
            Level.Current.AddEntity(newThorn);
        }
        _visitedCells.Clear();
    }
}