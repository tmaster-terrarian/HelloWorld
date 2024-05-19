using System;
using System.Collections.Generic;
using HelloWorld.Registries;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace HelloWorld;

public class Player : Entity
{
    public float moveSpeed = 2f;
    public float gravity = 0.2f;
    public bool onGround = false;

    public int inputDir = 0;

    public override Vector2 RenderPosition => base.RenderPosition;

    public Player()
    {
        width = 8;
        height = 14;
        Layer = 3;
    }

    public static event EventHandler<PlayerLandedEvent>? PlayerLanded;

    protected List<SoundRegistryEntry> sounds = new();

    public void LoadContent()
    {
        sounds.Add(Main.SoundRegistry.Get("playerLand"));
        sounds.Add(Main.SoundRegistry.Get("playerJump"));
    }

    public override void Update(float delta)
    {
        inputDir = Convert.ToInt32(Input.Get(Keys.D)) - Convert.ToInt32(Input.Get(Keys.A));

        if(inputDir == 1)
        {
            facing = 1;
            if(velocity.X < 0)
            {
                MathUtil.Approach(ref velocity.X, 0, 0.18f * delta);
            }
            else if(velocity.X < moveSpeed)
            {
                MathUtil.Approach(ref velocity.X, moveSpeed, 0.12f * delta);
            }
        }
        else if(inputDir == -1)
        {
            facing = -1;
            if(-velocity.X < 0)
            {
                MathUtil.Approach(ref velocity.X, 0, 0.18f * delta);
            }
            else if(-velocity.X < moveSpeed)
            {
                MathUtil.Approach(ref velocity.X, -moveSpeed, 0.12f * delta);
            }
        }
        else
        {
            MathUtil.Approach(ref velocity.X, 0, 0.18f * 2 * delta);
        }

        if(!onGround)
        {
            velocity.Y = MathUtil.Approach(velocity.Y, 20, gravity * delta);
        }

        if(onGround && Input.GetPressed(Keys.Space))
        {
            sounds[1].Play();

            velocity.Y = -3.7f;
        }

        if(Input.Get(Keys.LeftControl))
        {
            position = Main.MouseWorldPos;
            velocity = Vector2.Zero;
        }

        Move(velocity * delta, Main.Level);

        bool wasOnGround = onGround;
        onGround = Main.Level.TileMeeting(RectangleHelper.Shift(Hitbox, new Point(0, 1)));

        if(!wasOnGround && onGround)
        {
            sounds[0].Play();

            PlayerLanded?.Invoke(this, new PlayerLandedEvent(this));
        }
    }

    public class PlayerLandedEvent : EventArgs
    {
        public PlayerLandedEvent(Player player)
        {
            Player = player;
        }

        public Player Player { get; set; }
    }
}
