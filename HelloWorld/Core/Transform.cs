using System.ComponentModel;
using Math = System.Math;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelloWorld.Core;

public struct TransformData
{
    public Vector2? position;
    public Vector2? scale;
    public float? angle;
}

public class Transform
{
    public Vector2 position { get; private set; } = Vector2.Zero;
    public Vector2 scale { get; private set; } = Vector2.One;
    public float angle { get; private set; } = 0;

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
    }

    public Transform(Vector2 position, float uniformScale)
    {
        this.position = position;
        this.scale = Vector2.One * uniformScale;
    }

    public Transform(Vector2 position)
    {
        this.position = position;
    }

    public Transform() {}

    #endregion

    #region modification functions

    public Transform Translate(Vector2 position, bool relative = true)
    {
        return new Transform(relative ? position + this.position : position, this.scale, this.angle);
    }

    public Transform Rotate(float angle, bool relative = true)
    {
        return new Transform(this.position, this.scale, relative ? angle + this.angle : angle);
    }

    public Transform Scale(Vector2 scale, bool multiply = true)
    {
        return new Transform(this.position, multiply ? scale * this.scale : scale, this.angle);
    }

    public Transform Scale(float uniformScale, bool multiply = true)
    {
        return this.Scale(Vector2.One * uniformScale, multiply);
    }

    #endregion

    #region conversion

    public override string ToString()
    {
        return $"{{position: {position}, scale: {scale}, angle: {angle}}}";
    }

    /// <summary>
    /// <para>Creates an Affine Transformation Matrix.</para>
    /// <para>The Matrix multiplication order is angle, then scale, then translation.</para>
    /// </summary>
    /// <returns>Matrix</returns>
    public Matrix ToMatrix()
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

    public static bool operator ==(Transform a, Transform b)
    {
        return a.Equals(b);
    }
    public static bool operator !=(Transform a, Transform b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if(ReferenceEquals(this, obj)) return true;
        if(obj == null) return false;

        if(obj.GetType() == typeof(Transform)) return Equals(this, obj);

        return false;
    }

    public override int GetHashCode()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
