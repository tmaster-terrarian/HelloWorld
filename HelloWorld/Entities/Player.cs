using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using HelloWorld.Core;
using HelloWorld.Registries;

namespace HelloWorld;

public class Player : Entity
{
    protected readonly List<Registries.Sound.SoundDef> sounds = new();

    private readonly int _baseReach = 6;
    private readonly PlayerInventory _inventory = new();
    private PlayerState state;
    private bool attacking = false;

    public float moveSpeed = 2f;
    public float jumpSpeed = -4.35f;
    public float gravity = 0.2f;
    public bool onGround = false;
    public int inputDir = 0;

    public float bodyFrame = 0;
    public float legFrame = 0;
    public float armFrame = 0;
    public float eyelidFrame = 0;

    public bool female = true;

    bool jumpCancelled = false;

    public override Vector2 RenderPosition => base.RenderPosition;

    public PlayerInventory Inventory => _inventory;
    public PlayerState State { get => state; set => state = value; }

    public int HeldItemSlot { get; set; } = 0;

    public ItemStack HeldItem => _inventory[HeldItemSlot];

    public Player()
    {
        width = 12;
        height = 22;
        Layer = 3;

        _inventory.TryInsert(new("iron_pickaxe"), 0);
        _inventory.TryInsert(new("stone", 10), 2);
        _inventory.TryInsert(new("brick", 10), 3);
    }

    public void LoadContent()
    {
        sounds.Add(Registry.GetSound("playerLand"));
        sounds.Add(Registry.GetSound("playerJump"));
    }

    public enum PlayerState
    {
        Normal,
    }

    bool TryNudgeDown(int amount)
    {
        if(!level.TileMeeting(RectangleHelper.Shift(Hitbox, new Point(0, amount))) && level.TileMeeting(RectangleHelper.Shift(Hitbox, new Point(0, amount + 1))))
        {
            for(int i = 0; i < amount; i++)
            {
                Move(new(0, 1), false, false);
                if(level.TileMeeting(RectangleHelper.Shift(Hitbox, new Point(0, 1))))
                {
                    onGround = true;
                    return true;
                }
            }
        }
        return false;
    }

    public override void Update()
    {
        inputDir = Input.Get(Keys.D).ToInt32() - Input.Get(Keys.A).ToInt32();

        bool wasOnGround = onGround;
        onGround = level.TileMeeting(RectangleHelper.Shift(Hitbox, new Point(0, 1)));

        if(!wasOnGround && onGround)
        {
            jumpCancelled = false;
        }
        if(wasOnGround && !onGround && velocity.Y >= 0)
        {
            if(!TryNudgeDown(4)) TryNudgeDown(8);
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
            Facing = 1;
            if(velocity.X < 0)
            {
                MathUtil.Approach(ref velocity.X, 0, fric);
            }
            if(velocity.X < moveSpeed)
            {
                MathUtil.Approach(ref velocity.X, moveSpeed, accel);
            }
            if(velocity.X > moveSpeed && onGround)
            {
                MathUtil.Approach(ref velocity.X, moveSpeed, fric/2);
            }
        }
        else if(inputDir == -1)
        {
            Facing = -1;
            if(-velocity.X < 0)
            {
                MathUtil.Approach(ref velocity.X, 0, fric);
            }
            if(-velocity.X < moveSpeed)
            {
                MathUtil.Approach(ref velocity.X, -moveSpeed, accel);
            }
            if(-velocity.X > moveSpeed && onGround)
            {
                MathUtil.Approach(ref velocity.X, -moveSpeed, fric/2);
            }
        }
        else
        {
            MathUtil.Approach(ref velocity.X, 0, fric * 2);
        }

        if(!onGround)
        {
            velocity.Y = MathUtil.Approach(velocity.Y, 20, gravity);

            if(Input.GetReleased(Keys.Space) && velocity.Y < 0 && !jumpCancelled)
            {
                jumpCancelled = true;
                velocity.Y /= 2;
            }
        }

        if(onGround && Input.GetPressed(Keys.Space))
        {
            velocity.Y = jumpSpeed;
        }

        int slotSelectDir = -Input.GetScrollDirection();

        HeldItemSlot += slotSelectDir;

        if(HeldItemSlot < 0) HeldItemSlot = 9;
        if(HeldItemSlot > 9) HeldItemSlot = 0;

        for(int i = 0; i < 10; i++)
        {
            if(Input.GetPressed(Keys.D0 + i))
            {
                HeldItemSlot = (i - 1) % 10;
                break;
            }
        }

        if(Input.Get(Keys.LeftControl))
        {
            Center = Main.MouseWorldPos;
            velocity = Vector2.Zero;
        }

        Move(velocity);

        if(attacking)
        {
            if(armFrame < 2) armFrame = 2;
            armFrame = MathUtil.Approach(armFrame, 6, 0.2f);
            if(armFrame >= 6) armFrame = 2;
        }

        if(Input.Get(MouseButtons.LeftButton))
        {
            attacking = true;
        }
        else if(attacking)
        {
            if(armFrame == 2) attacking = false;
        }

        if(onGround)
        {
            if(MathF.Sign(velocity.X) != 0 && MathF.Abs(velocity.X) > accel * 2.1f)
            {
                if(legFrame < 6) legFrame = 6;
                legFrame = MathUtil.Approach(legFrame, 20, MathF.Abs(velocity.X) / 4f);
                if(legFrame >= 20) legFrame = 6;
            }
            else
            {
                legFrame = attacking ? armFrame : 0;
            }
            if(!attacking) armFrame = legFrame;
        }
        else
        {
            legFrame = 1;
            if(!attacking) armFrame = 1;
        }

        if(attacking)
        {
            bodyFrame = armFrame;
        }
        else
        {
            bodyFrame = legFrame;
            if(!onGround && velocity.Y < 0)
            {
                bodyFrame = 5;
            }
        }
    }

    public override void OnCollisionX()
    {
        if(inputDir != 0 && (!level.TileMeeting(RectangleHelper.Shift(Hitbox, new Point(inputDir, -4))) || !level.TileMeeting(RectangleHelper.Shift(Hitbox, new Point(inputDir, -8)))))
        {
            for(int i = 0; i < 8; i++)
            {
                Move(new(0, -1), false, true);
                if(!level.TileMeeting(RectangleHelper.Shift(Hitbox, new Point(inputDir, 0))))
                {
                    Move(new(inputDir * 2, 0), false, false);
                    break;
                }
            }
        }
        else
        {
            base.OnCollisionX();
        }
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
            Center - Vector2.UnitY * 2 * Level.tileSize,
            MathUtil.Snap(position, Level.tileSize) + Vector2.One * (Level.tileSize / 2f)
        );

        return distance <= _baseReach * Level.tileSize;
    }
}
