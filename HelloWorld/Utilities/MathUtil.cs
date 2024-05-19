using System;

namespace HelloWorld;

public static class MathUtil
{
    public static float Approach(float value, float target, float rate)
    {
        if(value < target)
            return MathF.Min(value + rate, target);
        else
            return MathF.Max(value - rate, target);
    }

    public static void Approach(ref float value, float target, float rate)
    {
        if(value < target)
            value = MathF.Min(value + rate, target);
        else
            value = MathF.Max(value - rate, target);
    }
}
