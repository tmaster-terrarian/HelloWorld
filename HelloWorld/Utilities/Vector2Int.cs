using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.Xna.Framework;

[DataContract]
[DebuggerDisplay("{DebugDisplayString,nq}")]
public struct Vector2Int : IEquatable<Vector2>
{
    private static readonly Vector2Int zeroVector = new Vector2Int(0, 0);

    private static readonly Vector2Int unitVector = new Vector2Int(1, 1);

    private static readonly Vector2Int unitXVector = new Vector2Int(1, 0);

    private static readonly Vector2Int unitYVector = new Vector2Int(0, 1);

    public static Vector2Int Zero => zeroVector;

    public static Vector2Int One => unitVector;

    public static Vector2Int UnitX => unitXVector;

    public static Vector2Int UnitY => unitYVector;

    internal string DebugDisplayString => X + "  " + Y;

    [DataMember]
    public int X;

    [DataMember]
    public int Y;

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2Int(float x, float y)
    {
        X = (int)x;
        Y = (int)y;
    }

    public Vector2Int(int value)
    {
        X = value;
        Y = value;
    }

    public Vector2Int(float value)
    {
        X = (int)value;
        Y = (int)value;
    }

    public static implicit operator Vector2Int(System.Numerics.Vector2 value)
    {
        return new Vector2Int(value.X, value.Y);
    }

    public static implicit operator Vector2Int(Vector2 value)
    {
        return new Vector2Int(value.X, value.Y);
    }

    public static implicit operator Vector2(Vector2Int value)
    {
        return new Vector2(value.X, value.Y);
    }

    public static implicit operator Vector2Int(Point value)
    {
        return new Vector2Int(value.X, value.Y);
    }

    public static implicit operator Point(Vector2Int value)
    {
        return new Point(value.X, value.Y);
    }

    public static Vector2Int operator -(Vector2Int value)
    {
        value.X = 0 - value.X;
        value.Y = 0 - value.Y;
        return value;
    }

    public static Vector2Int operator +(Vector2Int value1, Vector2Int value2)
    {
        value1.X += value2.X;
        value1.Y += value2.Y;
        return value1;
    }

    public static Vector2Int operator -(Vector2Int value1, Vector2Int value2)
    {
        value1.X -= value2.X;
        value1.Y -= value2.Y;
        return value1;
    }

    public static Vector2Int operator *(Vector2Int value1, Vector2Int value2)
    {
        value1.X *= value2.X;
        value1.Y *= value2.Y;
        return value1;
    }

    public static Vector2Int operator *(Vector2Int value1, Vector2 value2)
    {
        value1.X = (int)(value1.X * value2.X);
        value1.Y = (int)(value1.X * value2.X);
        return value1;
    }

    public static Vector2Int operator *(Vector2 value1, Vector2Int value2)
    {
        value2.X = (int)(value1.X * value2.X);
        value2.Y = (int)(value1.X * value2.X);
        return value2;
    }

    public static Vector2Int operator *(Vector2Int value, float scaleFactor)
    {
        value.X = (int)(value.X * scaleFactor);
        value.Y = (int)(value.Y * scaleFactor);
        return value;
    }

    public static Vector2Int operator *(float scaleFactor, Vector2Int value)
    {
        value.X = (int)(value.X * scaleFactor);
        value.Y = (int)(value.Y * scaleFactor);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int operator /(Vector2Int value1, Vector2Int value2)
    {
        value1.X /= value2.X;
        value1.Y /= value2.Y;
        return value1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int operator /(Vector2Int value1, float divider)
    {
        float num = 1f / divider;
        value1.X = (int)(value1.X * num);
        value1.Y = (int)(value1.Y * num);
        return value1;
    }

    public static bool operator ==(Vector2Int value1, Vector2Int value2)
    {
        if (value1.X == value2.X)
        {
            return value1.Y == value2.Y;
        }

        return false;
    }

    public static bool operator !=(Vector2Int value1, Vector2Int value2)
    {
        if (value1.X == value2.X)
        {
            return value1.Y != value2.Y;
        }

        return true;
    }

    public static Vector2Int Add(Vector2Int value1, Vector2Int value2)
    {
        value1.X += value2.X;
        value1.Y += value2.Y;
        return value1;
    }

    public static Vector2Int Subtract(Vector2Int value1, Vector2Int value2)
    {
        value1.X -= value2.X;
        value1.Y -= value2.Y;
        return value1;
    }

    public static Vector2Int Clamp(Vector2Int value1, Vector2Int min, Vector2Int max)
    {
        return new Vector2Int(MathHelper.Clamp(value1.X, min.X, max.X), MathHelper.Clamp(value1.Y, min.Y, max.Y));
    }

    public static int TaxicabDistance(Vector2Int value1, Vector2Int value2)
    {
        int x = Math.Abs(value2.X - value1.X);
        int y = Math.Abs(value2.Y - value1.Y);
        return x + y;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector2Int)
        {
            return Equals((Vector2Int)obj);
        }

        return false;
    }

    public bool Equals(Vector2Int other)
    {
        if (X == other.X)
        {
            return Y == other.Y;
        }

        return false;
    }

    public float Length()
    {
        return MathF.Sqrt(X * X + Y * Y);
    }

    public float LengthSquared()
    {
        return X * X + Y * Y;
    }

    public float TaxicabLength()
    {
        return X + Y;
    }

    public static Vector2Int Max(Vector2Int value1, Vector2Int value2)
    {
        return new Vector2Int((value1.X > value2.X) ? value1.X : value2.X, (value1.Y > value2.Y) ? value1.Y : value2.Y);
    }

    public static Vector2Int Min(Vector2Int value1, Vector2Int value2)
    {
        return new Vector2((value1.X < value2.X) ? value1.X : value2.X, (value1.Y < value2.Y) ? value1.Y : value2.Y);
    }

    public static Vector2Int Negate(Vector2Int value)
    {
        value.X = 0 - value.X;
        value.Y = 0 - value.Y;
        return value;
    }

    public override string ToString()
    {
        return "{X:" + X + " Y:" + Y + "}";
    }

    public Point ToPoint()
    {
        return new Point(X, Y);
    }

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    public bool Equals(Vector2 other)
    {
        if(X == other.X)
        {
            return Y == other.Y;
        }

        return false;
    }
}
