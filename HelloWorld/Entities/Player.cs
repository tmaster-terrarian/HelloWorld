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

    private readonly PlayerInventory _inventory = new();
    private int _selectedSlot = 0;

    public PlayerInventory Inventory => _inventory;

    public int HeldItemSlot => _selectedSlot;
    public ItemStack HeldItem => _inventory[_selectedSlot];

    public Player()
    {
        width = 8;
        height = 14;
        Layer = 3;

        _inventory.TryInsert(new("IronPickaxe"), 0);
        _inventory.TryInsert(new("Stone", 10), 2);
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

        int slotSelectDir = -Input.GetScrollDelta();

        _selectedSlot += slotSelectDir;

        if(_selectedSlot < 0) _selectedSlot = 9;
        if(_selectedSlot > 9) _selectedSlot = 0;

        if(Input.GetPressed(Keys.D1)) _selectedSlot = 0;
        if(Input.GetPressed(Keys.D2)) _selectedSlot = 1;
        if(Input.GetPressed(Keys.D3)) _selectedSlot = 2;
        if(Input.GetPressed(Keys.D4)) _selectedSlot = 3;
        if(Input.GetPressed(Keys.D5)) _selectedSlot = 4;
        if(Input.GetPressed(Keys.D6)) _selectedSlot = 5;
        if(Input.GetPressed(Keys.D7)) _selectedSlot = 6;
        if(Input.GetPressed(Keys.D8)) _selectedSlot = 7;
        if(Input.GetPressed(Keys.D9)) _selectedSlot = 8;
        if(Input.GetPressed(Keys.D0)) _selectedSlot = 9;

        if(Input.Get(Keys.LeftControl))
        {
            position = Main.MouseWorldPos;
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

    public bool IsHoldingTileItem()
    {
        if(HeldItem is null) return false;

        var settings = HeldItem.GetDef().settings;

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
}
