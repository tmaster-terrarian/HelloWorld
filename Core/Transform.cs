using System.ComponentModel;
using Math = System.Math;

using Microsoft.Xna.Framework;
using System;

namespace GAY.Core;

public struct TransformData
{
    public Vector2? position;
    public Vector2? scale;
    public float? angle;
}

#pragma warning disable CS0659
#pragma warning disable CS0661
[ImmutableObject(true)]
public struct Transform
{
    public Vector2 position { get; private set; }
    public Vector2 scale { get; private set; }
    public float angle { get; private set; }

    public Transform(Vector2 position, Vector2 scale, float angle)
    {
        this.position = position;
        this.scale = scale;
        this.angle = angle % 360;
    }

    public Transform(TransformData data)
    {
        this.position = data.position != null ? (Vector2)data.position : Vector2.Zero;
        this.scale = data.scale != null ? (Vector2)data.scale : Vector2.One;
        this.angle = data.angle != null ? (float)data.angle % 360 : 0;
    }

    #region constructor overloads

    public Transform(Vector2 position, Vector2 scale)
    {
        this.position = position;
        this.scale = scale;
        this.angle = 0;
    }

    public Transform(Vector2 position, float uniformScale)
    {
        this.position = position;
        this.scale = Vector2.One * uniformScale;
        this.angle = 0;
    }

    public Transform(Vector2 position)
    {
        this.position = position;
        this.scale = Vector2.One;
        this.angle = 0;
    }

    public Transform()
    {
        this.position = Vector2.Zero;
        this.scale = Vector2.One;
        this.angle = 0;
    }

    #endregion

    #region modification functions

    public readonly Transform Translate(Vector2 position, bool relative = true)
    {
        return new Transform(relative ? position + this.position : position, this.scale, this.angle);
    }

    public readonly Transform Scale(Vector2 scale, bool multiply = true)
    {
        return new Transform(this.position, multiply ? scale * this.scale : scale, this.angle);
    }

    public readonly Transform Scale(float scale, bool multiply = true)
    {
        return this.Scale(Vector2.One * scale, multiply);
    }

    public readonly Transform Rotate(float angle, bool relative = true)
    {
        return new Transform(this.position, this.scale, relative ? angle + this.angle : angle);
    }

    public readonly Transform RotateRadians(float angle, bool relative = true)
    {
        return new Transform(this.position, this.scale, relative ? Utils.RadToDeg(angle) + this.angle : Utils.RadToDeg(angle));
    }

    #endregion

    #region conversion

    public override readonly string ToString()
    {
        return $"{{position: {position}, scale: {scale}, angle: {angle}}}";
    }

    /// <summary>
    /// <para>Creates an Affine Transformation Matrix.</para>
    /// <para>The Matrix multiplication order is angle, then scale, then translation.</para>
    /// </summary>
    /// <returns>Matrix</returns>
    public readonly Matrix ToMatrix()
    {
        Matrix translation = Matrix.Identity;
        translation[0, 2] = position.X;
        translation[1, 2] = position.Y;

        Matrix dilation = Matrix.Identity;
        dilation[0, 0] = scale.X;
        dilation[1, 1] = scale.Y;

        Matrix rotation = Matrix.Identity;
        rotation[0, 0] =  (float)Math.Acos(angle % 360); rotation[0, 1] = (float)Math.Asin(angle % 360);
        rotation[1, 0] = -(float)Math.Asin(angle % 360); rotation[1, 1] = (float)Math.Acos(angle % 360);

        return translation * dilation * rotation;
    }

    #endregion

    #region equation / comparison

    public readonly bool Equals(Transform transform)
    {
        return transform.position == position && transform.scale == scale && transform.angle == angle;
    }

    public static bool operator ==(Transform a, Transform b)
    {
        return a.Equals(b);
    }
    public static bool operator !=(Transform a, Transform b)
    {
        return !(a == b);
    }

    public override readonly bool Equals(object obj)
    {
        return Equals(this, obj);
    }

    #endregion
}
#pragma warning restore CS0661
#pragma warning restore CS0659
