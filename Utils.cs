using Math = System.Math;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace GAY;

public static class Utils
{
    public static float UnBool(bool value) => value == true ? 1f : 0f;

    public static float Approach(float value, float target, float rate)
    {
        if(value < target)
            return Math.Min(value + rate, target);
        else
            return Math.Max(value - rate, target);
    }

    public static void Approach(ref float value, float target, float rate)
    {
        if(value < target)
            value = Math.Min(value + rate, target);
        else
            value = Math.Max(value - rate, target);
    }

    public static float DegToRad(float degrees)
    {
        return degrees * (float)(Math.PI / 180);
    }
    public static double DegToRad(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

    public static float RadToDeg(float radians)
    {
        return radians * (float)(180 / Math.PI);
    }
    public static double RadToDeg(double radians)
    {
        return radians * (180 / Math.PI);
    }
}
