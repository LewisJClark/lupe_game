using Godot;

public class Entity : Node2D
{
    public CellTransform CellTransform { get; private set; }
    public Entity()
    {
        CellTransform = new CellTransform(this);
    }
}