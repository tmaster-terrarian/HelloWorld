using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HelloWorld.Core;
using HelloWorld.Graphics;

namespace HelloWorld;

public abstract class Entity
{
    public Vector2 position = Vector2.Zero;
    public Vector2 scale = Vector2.One;
    public float angle = 0;

    public int width = 1;
    public int height = 1;

    protected float _layer { get; private set; }

    protected bool _noTileCollision = false;

    public Level level { get; set; } = Main.Level;

    public bool NoCollision => _noTileCollision;

    public int Layer
    {
        get => (int)(_layer * 10000);
        set => _layer = value / 10000f;
    }

    public Vector2 velocity = Vector2.Zero;

    private Vector2 _remainderPosition = Vector2.Zero;

    public int facing = 1;

    public Vector2 Size
    {
        get => new Vector2(width, height);
        set {
            width = (int)value.X;
            height = (int)value.Y;
        }
    }

    public Rectangle Hitbox
    {
        get => new Rectangle((int)position.X, (int)position.Y, width, height);
        set {
            position = new Vector2(value.X, value.Y);
            width = value.Width;
            height = value.Height;
        }
    }

    public Vector2 Center
    {
        get => new Vector2(position.X + width / 2, position.Y + height / 2);
        set => position = new Vector2(value.X - width / 2, value.Y - height / 2);
    }

    public int Left
    {
        get => Hitbox.X;
        set {
            position.X = value;
        }
    }
    public int Right
    {
        get => Hitbox.X + width;
        set {
            position.X = value - width;
        }
    }
    public int Top
    {
        get => Hitbox.Y;
        set {
            position.Y = value;
        }
    }
    public int Bottom
    {
        get => Hitbox.Y + height;
        set {
            position.Y = value - height;
        }
    }

    public virtual Vector2 RenderPosition => position;

    public virtual void Update(float delta) {}

    public virtual void OnCollisionX()
    {
        velocity.X = 0;
    }

    public virtual void OnCollisionY()
    {
        velocity.Y = 0;
    }

    public void Move(Vector2 vel, bool onCollisionX = true, bool onCollisionY = true)
    {
        _remainderPosition += vel;

        int moveX = (int)MathF.Round(_remainderPosition.X);
        _remainderPosition.X -= moveX;
        int signX = MathF.Sign(moveX);

        if(_noTileCollision)
            position.X += moveX;
        else for(var i = 0; i < MathF.Abs(moveX); i++)
        {
            Rectangle hitbox = Hitbox;

            if(level.TileMeeting(RectangleHelper.Shift(hitbox, new Vector2(signX, 0))))
            {
                if(onCollisionX) OnCollisionX();
                break;
            }
            else
            {
                position.X += signX;
            }
        }

        int moveY = (int)MathF.Round(_remainderPosition.Y);
        _remainderPosition.Y -= moveY;
        int signY = MathF.Sign(moveY);

        if(_noTileCollision)
            position.Y += moveY;
        else for(var i = 0; i < MathF.Abs(moveY); i++)
        {
            Rectangle hitbox = Hitbox;

            if(level.TileMeeting(RectangleHelper.Shift(hitbox, new Vector2(0, signY))))
            {
                if(onCollisionY) OnCollisionY();
                break;
            }
            else
            {
                position.Y += signY;
            }
        }
    }
}
