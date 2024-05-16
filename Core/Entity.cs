using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace GAY.Core;

public class Entity
{
    public static EntityObject Create<T>(Vector2 position) where T : Entity, new()
    {
        EntityObject entity = new EntityObject {
            transform = new Transform(new TransformData {
                position = position
            })
        };

        return entity;
    }
}

public struct EntityObject
{
    public Transform transform;
}
