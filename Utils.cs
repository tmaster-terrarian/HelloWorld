using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HelloWorld;

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
}
