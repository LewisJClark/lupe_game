using Godot;
using System;
using System.Collections.Generic;

public class Level : Node2D
{
    public event Action Completed;
    
    public static Level Current = null;
    
    public readonly int CellSize = 16;
    
    [Export] public int Width { get; private set; }
    [Export] public int Height { get; private set; }
    [Export] private PackedScene _thornScene;

    private Vector2 _playerSpawnCell;

    private Dictionary<Vector2, Node2D> _entities = new Dictionary<Vector2, Node2D>();
    private HashSet<Vector2> _visitedCells =  new HashSet<Vector2>();
    
    public override void _Ready()
    {
        Current = this;
        
        // Center the level on the screen.
        var centeredX = GetViewport().Size.x / 2 - (Width * CellSize / 2);
        var centeredY = GetViewport().Size.y / 2 - (Height * CellSize / 2);
        Position =  new Vector2(centeredX, centeredY);
        
        // Add all the entities placed in the editor to our 'grid'.
        foreach (Node2D child in GetChildren())
        {
            if (child is Player p)
                _playerSpawnCell = p.Position / CellSize;

            if (child is Flower f)
            {
                f.AllPetalsRemoved += () =>
                {
                    Completed?.Invoke();
                    // TODO: Remove this - we just restart the level when it's completed at the moment.
                    GetTree().ReloadCurrentScene();
                };
            }

            _entities.Add(child.Position / CellSize, child);
        }
    }

    public void Move(Node2D entity, Vector2 direction)
    {
        var currentCell = entity.Position / CellSize;
        var targetCell = currentCell + direction;

        if (!CellInBounds(targetCell))
            return;
        
        if (!_entities.ContainsKey(targetCell))
        {
            if (entity is Player)
                _visitedCells.Add(targetCell);
            entity.Position = targetCell * CellSize;

            _entities.Remove(currentCell);
            _entities.Add(targetCell, entity);
        }

        var entityInCell =  _entities[targetCell];
        if (entityInCell is Obstacle)
            return;

        if (entity is Player && entityInCell is Flower flower)
        {
            flower.RemovePetal();
            entity.Position = _playerSpawnCell * CellSize;
            _entities.Remove(currentCell);
            _entities.Add(_playerSpawnCell, entity);
            SpawnThorns();
        }
        else if (entity is Player && entityInCell is Thorn)
            GetTree().ReloadCurrentScene();
    }

    private bool CellInBounds(Vector2 cell) => cell.x >= 0 && cell.x < Width && cell.y >= 0 && cell.y < Height;
    
    private void SpawnThorns()
    {
        foreach (var cell in _visitedCells)
        {
            var newThorn = _thornScene.Instance<Thorn>();
            newThorn.Position = cell * CellSize;
            _entities.Add(cell, newThorn);
            AddChild(newThorn);
        }
        _visitedCells.Clear();
    }
    
}
