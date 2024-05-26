using Microsoft.Xna.Framework;

namespace HelloWorld.Events;

public class TileEvent : System.EventArgs
{
    private readonly Point _tilePos; // actual value, hidden from other classes
    private readonly Entity _entity;
    private readonly string _id;

    public Point TilePos => _tilePos; // publically available property (which only provides the value, and cannot set the value)
    public Entity Entity => _entity;
    public string ID => _id;

    public TileEvent(Point tilePos, Entity entity, string tileId)
    {
        _tilePos = tilePos;
        _entity = entity;
        _id = tileId;
    }
}
