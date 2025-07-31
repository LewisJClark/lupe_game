using Godot;
using System;
using System.Collections.Generic;

public class Level : Node2D
{
	public event Action Completed;
	
	public static Level Current = null;
	
	[Export] public int Width { get; private set; }
	[Export] public int Height { get; private set; }

	private Vector2 _playerSpawnCell;

	private Dictionary<Vector2, Entity> _entities = new Dictionary<Vector2, Entity>();
	
	public override void _Ready()
	{
		Current = this;
		
		// Center the level on the screen.
		var centeredX = GetViewport().Size.x / 2 - (Width * CellTransform.CELL_SIZE / 2);
		var centeredY = GetViewport().Size.y / 2 - (Height * CellTransform.CELL_SIZE / 2);
		Position =  new Vector2(centeredX, centeredY);
		
		// Add all the entities placed in the editor to our 'grid'.
		foreach (Node2D child in GetChildren())
		{
            if(!(child is Entity entity)) continue;

			if (entity is Player player)
            {
                _playerSpawnCell = player.CellTransform.Position;
                continue;
            }
			
			if (entity is Flower flower)
			{
				flower.AllPetalsRemoved += () =>
				{
					Completed?.Invoke();
					// TODO: Remove this - we just restart the level when it's completed at the moment.
					GetTree().ReloadCurrentScene();
				};
			}

			_entities.Add(entity.CellTransform.Position, entity);
		}
	}

    public bool IsCellValid(Vector2 targetCell, out Entity entityInCell)
    {
        entityInCell = null;

        if(!CellInBounds(targetCell)) return false;

        if (_entities.TryGetValue(targetCell, out entityInCell)
            && entityInCell is Obstacle)
            return false;

        return true;
    }

    public void AddEntity(Entity entity)
    {
        _entities.Add(entity.CellTransform.Position, entity);
        AddChild(entity);
    }

	private bool CellInBounds(Vector2 cell) => cell.x >= 0 && cell.x < Width && cell.y >= 0 && cell.y < Height;
}
