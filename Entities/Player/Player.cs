using Godot;
using System.Collections.Generic;

public class Player : Entity
{
    [Export] private PackedScene _thornScene;

    private Vector2 _spawnPosition;
    private SceneTreeTween _movementTween;

    private Stack<Vector2> _directionInput = new Stack<Vector2>();

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
        StoreDirectionInput(direction);
    }

    private void StoreDirectionInput(Vector2 direction)
    {
        if (_directionInput.Count > 0 && _directionInput.Peek() == -direction)
            _directionInput.Pop();
        else
            _directionInput.Push(direction);
    }

    private void NextLoop()
    {
        SpawnThorns();

        // TODO: cooler reset animation
        StartMoveTween(_spawnPosition);
    }

    private void SpawnThorns()
    {
        var cellPosition = CellTransform.WorldToCell(_spawnPosition);
        var path = _directionInput.ToArray();

        for (int i = path.Length - 1; i >= 0; i--)
        {
            var direction = path[i];

            cellPosition += direction;

            var newThorn = _thornScene.Instance<Thorn>();
            newThorn.CellTransform.MoveTo(cellPosition);
            Level.Current.AddEntity(newThorn);
        }

        _directionInput.Clear();
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
}