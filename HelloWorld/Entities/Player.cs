using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

using HelloWorld.Registries;

namespace HelloWorld;

public class Player : Entity
{
    protected List<Registries.Sound.SoundDef> sounds = new();

    public float moveSpeed = 2f;
    public float jumpSpeed = -4.1f;
    public float gravity = 0.2f;
    public bool onGround = false;

    public int inputDir = 0;

    public override Vector2 RenderPosition => base.RenderPosition;

    public static event EventHandler<PlayerLandedEvent>? PlayerLanded;

    private readonly int _baseReach = 6;
    private readonly PlayerInventory _inventory = new();

    public PlayerInventory Inventory => _inventory;

    public int HeldItemSlot { get; set; } = 0;

    public ItemStack HeldItem => _inventory[HeldItemSlot];

    public Player()
    {
        width = 8;
        height = 14;
        Layer = 3;

        _inventory.TryInsert(new("iron_pickaxe"), 0);
        _inventory.TryInsert(new("stone", 10), 2);
    }

    public void LoadContent()
    {
        sounds.Add(Registry.GetSound("playerLand"));
        sounds.Add(Registry.GetSound("playerJump"));
    }

    public override void Update(float delta)
    {
        inputDir = Input.Get(Keys.D).ToInt32() - Input.Get(Keys.A).ToInt32();

        bool wasOnGround = onGround;
        onGround = level.TileMeeting(RectangleHelper.Shift(Hitbox, new Point(0, 1)));

        if(!wasOnGround && onGround)
        {
            sounds[0].Play();

            PlayerLanded?.Invoke(this, new PlayerLandedEvent(this));
        }

        float accel = 0.12f;
        float fric = 0.08f;
        if(!onGround)
        {
            accel = 0.07f;
            fric = 0.02f;
        }

        if(inputDir == 1)
        {
            facing = 1;
            if(velocity.X < 0)
            {
                MathUtil.Approach(ref velocity.X, 0, fric * delta);
            }
            if(velocity.X < moveSpeed)
            {
                MathUtil.Approach(ref velocity.X, moveSpeed, accel * delta);
            }
            if(velocity.X > moveSpeed && onGround)
            {
                MathUtil.Approach(ref velocity.X, moveSpeed, fric/2 * delta);
            }
        }
        else if(inputDir == -1)
        {
            facing = -1;
            if(-velocity.X < 0)
            {
                MathUtil.Approach(ref velocity.X, 0, fric * delta);
            }
            if(-velocity.X < moveSpeed)
            {
                MathUtil.Approach(ref velocity.X, -moveSpeed, accel * delta);
            }
            if(-velocity.X > moveSpeed && onGround)
            {
                MathUtil.Approach(ref velocity.X, -moveSpeed, fric/2 * delta);
            }
        }
        else
        {
            MathUtil.Approach(ref velocity.X, 0, fric * 2 * delta);
        }

        if(!onGround)
        {
            velocity.Y = MathUtil.Approach(velocity.Y, 20, gravity * delta);
        }

        if(onGround && Input.GetPressed(Keys.Space))
        {
            sounds[1].Play();

            velocity.Y = jumpSpeed;
        }

        int slotSelectDir = -Input.GetScrollDirection();

        HeldItemSlot += slotSelectDir;

        if(HeldItemSlot < 0) HeldItemSlot = 9;
        if(HeldItemSlot > 9) HeldItemSlot = 0;

        if(Input.GetPressed(Keys.D1)) HeldItemSlot = 0;
        if(Input.GetPressed(Keys.D2)) HeldItemSlot = 1;
        if(Input.GetPressed(Keys.D3)) HeldItemSlot = 2;
        if(Input.GetPressed(Keys.D4)) HeldItemSlot = 3;
        if(Input.GetPressed(Keys.D5)) HeldItemSlot = 4;
        if(Input.GetPressed(Keys.D6)) HeldItemSlot = 5;
        if(Input.GetPressed(Keys.D7)) HeldItemSlot = 6;
        if(Input.GetPressed(Keys.D8)) HeldItemSlot = 7;
        if(Input.GetPressed(Keys.D9)) HeldItemSlot = 8;
        if(Input.GetPressed(Keys.D0)) HeldItemSlot = 9;

        if(Input.Get(Keys.LeftControl))
        {
            Center = Main.MouseWorldPos;
            velocity = Vector2.Zero;
        }

        Move(velocity * delta);
    }

    public class PlayerLandedEvent : EventArgs
    {
        public PlayerLandedEvent(Player player)
        {
            Player = player;
        }

        public Player Player { get; private set; }
    }

    public bool IsHoldingTileRelatedItem()
    {
        if(HeldItem is null) return false;

        var def = HeldItem.GetDef();
        var settings = def.settings;

        if(settings.pickaxe || settings.tileItem) return true;

        return false;
    }

    public bool IsHoldingPlaceable()
    {
        if(HeldItem is null) return false;

        var settings = HeldItem.GetDef().settings;

        if(settings.tileItem) return true;

        return false;
    }

    public bool TileWithinReach(Vector2 position)
    {
        var distance = Vector2.Distance(
            Center - Vector2.UnitY * 2 * Main.Level.tileSize,
            MathUtil.Snap(position, Main.Level.tileSize) + Vector2.One * (Main.Level.tileSize / 2f)
        );

        return distance <= _baseReach * Main.Level.tileSize;
    }
}
