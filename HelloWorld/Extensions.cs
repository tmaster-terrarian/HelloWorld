using Microsoft.Xna.Framework;

namespace HelloWorld;

public static class Extensions
{
    public static int ToInt32(this bool value)
    {
        return value ? 1 : 0;
    }

    public static Vector2Int ToVector2Int(this Vector2 value)
    {
        return value;
    }
}
