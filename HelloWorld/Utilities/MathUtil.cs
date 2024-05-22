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
}
