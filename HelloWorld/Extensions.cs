using System;

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

    public static string MemberwiseToString<T>(this T[] array, bool pretty = false)
    {
        string str = typeof(T).FullName;
        if(pretty)
        {
            str += " [\n  ";
        }
        else str += "[";

        for(int i = 0; i < array.Length; i++)
        {
            T item = array[i];

            str += item?.ToString() ?? "null";

            if(i < array.Length - 1)
            {
                str += ",";
                if(!pretty)
                {
                    str += " ";
                }
                else str += "\n  ";
            }
            else if(pretty) str += "\n";
        }

        str += "]";

        return str;
    }

    public static Color Hex2Color(uint value)
    {
        return new Color((int)((value & 0b00000000111111110000000000000000) >> 16), (int)((value & 0b00000000000000001111111100000000) >> 8), (int)(value & 0b00000000000000000000000011111111));
    }

    public static int NextWithinRange(this Random random, int minInclusive, int max)
    {
        if(minInclusive >= max) return minInclusive;

        return (int)(random.NextSingle() * (max - minInclusive));
    }

    public static bool NextBool(this Random random)
    {
        return random.NextSingle() < 0.5f;
    }
}
