using Godot;
using System;

public class CellTransform
{
    public const int CELL_SIZE = 16;

    public Vector2 Position => _entity.Position / CELL_SIZE;

    private readonly Node2D _entity;

    public CellTransform(Node2D entity)
    {
        _entity = entity;
    }

    public void MoveTo(Vector2 cellPosition)
    {
        _entity.Position = CellToWorld(cellPosition);
    }

    public static Vector2 CellToWorld(Vector2 cellPosition)
    {
        return cellPosition * CELL_SIZE;
    }
}
