using Microsoft.Xna.Framework;

namespace HelloWorld;

public static class MathUtil
{
    public static float Approach(float value, float target, float rate)
    {
        if(value < target)
            return MathHelper.Min(value + rate, target);
        else
            return MathHelper.Max(value - rate, target);
    }

    public static void Approach(ref float value, float target, float rate)
    {
        if(value < target)
            value = MathHelper.Min(value + rate, target);
        else
            value = MathHelper.Max(value - rate, target);
    }

    public static float Snap(float value, float interval)
    {
        return System.MathF.Floor(value / interval) * interval;
    }

    public static void Snap(ref float value, float interval)
    {
        value = System.MathF.Floor(value / interval) * interval;
    }

    public static Vector2 Snap(Vector2 value, Vector2 interval)
    {
        value.X = Snap(value.X, interval.X);
        value.Y = Snap(value.Y, interval.Y);
        return value;
    }

    public static void Snap(ref Vector2 value, Vector2 interval)
    {
        value.X = Snap(value.X, interval.X);
        value.Y = Snap(value.Y, interval.Y);
    }

    public static Vector2 Snap(Vector2 value, float interval)
    {
        value.X = Snap(value.X, interval);
        value.Y = Snap(value.Y, interval);
        return value;
    }

    public static void Snap(ref Vector2 value, float interval)
    {
        value.X = Snap(value.X, interval);
        value.Y = Snap(value.Y, interval);
    }

    public static float SnapCeiling(float value, float interval)
    {
        return System.MathF.Ceiling(value / interval) * interval;
    }

    public static void SnapCeiling(ref float value, float interval)
    {
        value = System.MathF.Ceiling(value / interval) * interval;
    }

    public static Vector2 SnapCeiling(Vector2 value, Vector2 interval)
    {
        value.X = SnapCeiling(value.X, interval.X);
        value.Y = SnapCeiling(value.Y, interval.Y);
        return value;
    }

    public static void SnapCeiling(ref Vector2 value, Vector2 interval)
    {
        value.X = SnapCeiling(value.X, interval.X);
        value.Y = SnapCeiling(value.Y, interval.Y);
    }

    public static Vector2 SnapCeiling(Vector2 value, float interval)
    {
        value.X = SnapCeiling(value.X, interval);
        value.Y = SnapCeiling(value.Y, interval);
        return value;
    }

    public static void SnapCeiling(ref Vector2 value, float interval)
    {
        value.X = SnapCeiling(value.X, interval);
        value.Y = SnapCeiling(value.Y, interval);
    }
}
