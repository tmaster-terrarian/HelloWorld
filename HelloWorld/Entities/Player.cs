using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using HelloWorld.Core;
using HelloWorld.Registries;
using HelloWorld.Events;

namespace HelloWorld;

public class Player : Entity
{
    protected readonly List<Registries.Sound.SoundDef> sounds = new();

    public class StyleColors
    {
        public Color Skin { get; set; }
        public Color Pants { get; set; }
        public Color Shoes { get; set; }
        public Color Clothes1 { get; set; }
        public Color Clothes2 { get; set; }
    }

    private readonly int _baseReach = 5;
    private readonly PlayerInventory _inventory = new();
    private PlayerState state;
    private bool swinging = false;
    private int armSpeed = 30;
    private int toolSpeed = 30;
    private float toolDelay = 0;

    public float moveSpeed = 2f;
    public float jumpSpeed = -4.35f;
    public float gravity = 0.2f;
    public bool onGround = false;
    public int inputDir = 0;

    public StyleColors styleColors = new StyleColors {
        Skin = Extensions.Hex2Color(0xfddac2),
        Pants = new Color(255, 230, 175),
        Shoes = new Color(160, 105, 60),
        Clothes1 = new Color(175, 165, 140),
        Clothes2 = new Color(160, 180, 215),
    };

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

    public bool Swinging => swinging;

    public ItemStack HeldItem => _inventory[HeldItemSlot];

    public Player()
    {
        width = 12;
        height = 22;
        Layer = 3;

        _inventory.TryInsert(new("iron_pickaxe"));
        _inventory.TryInsert(new("copper_pickaxe"));
        _inventory.TryInsert(new("stone", 10));
        _inventory.TryInsert(new("brick", 10));
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
        if(!level.TileMeeting(Hitbox.Shift(0, amount)) && level.TileMeeting(Hitbox.Shift(0, amount + 1)))
        {
            for(int i = 0; i < amount; i++)
            {
                Move(new(0, 1), false, false);
                if(level.TileMeeting(Hitbox.Shift(0, 1)))
                {
                    onGround = true;
                    return true;
                }
            }
        }
        return false;
    }

    public void Update()
    {
        inputDir = Input.Get(Keys.D).ToInt32() - Input.Get(Keys.A).ToInt32();

        bool wasOnGround = onGround;
        onGround = level.TileMeeting(Hitbox.Shift(0, 1));

        if(Hitbox.Bottom >= level.height * Level.tileSize)
        {
            onGround = true;
        }

        if(!wasOnGround && onGround)
        {
            jumpCancelled = false;
        }
        if(wasOnGround && !onGround && velocity.Y >= 0)
        {
            // if(!TryNudgeDown(4)) TryNudgeDown(8);
            TryNudgeDown(4);
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

        if(!swinging)
        {
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
        }

        if(Input.Get(Keys.LeftControl))
        {
            Center = Main.MouseWorldPos;
            velocity.Y = 0;
        }

        Move(velocity);

        if(Hitbox.Bottom > level.height * Level.tileSize)
        {
            velocity.Y = 0;
            position.Y += level.height * Level.tileSize - Hitbox.Bottom;
        }

        if(swinging)
        {
            if(armFrame == 2)
            {
                armSpeed = HeldItem?.GetDef().settings.useTime ?? armSpeed;

                var snd = Registry.GetSound("swing")?.Play();
                snd.Pitch += (Random.Shared.NextSingle() * 0.1f) - 0.05f;
            }
            armFrame = MathUtil.Approach(armFrame, 6, 4f / armSpeed);
            if(armFrame >= 6) armFrame = 2;
        }

        if(toolDelay > 0)
            toolDelay = MathUtil.Approach(toolDelay, 0, 1);

        if(Input.Get(MouseButtons.LeftButton) && HeldItem is not null)
        {
            var def = HeldItem.GetDef();
            if(def.settings.useStyle == UseStyle.Swing && !swinging)
            {
                swinging = true;
                armSpeed = def.settings.useTime;
                armFrame = 2;
            }

            if(def.settings.tool && toolDelay == 0)
            {
                toolSpeed = def.settings.toolSpeed;
                toolDelay = toolSpeed;
                TryUseTool();
            }

            if(def.settings.placeable && toolDelay == 0)
            {
                toolSpeed = def.settings.placementSpeed;
                toolDelay = toolSpeed;
                TryPlaceItem();
            }
        }
        else
        {
            if(swinging)
            {
                if(armFrame == 2) swinging = false;
            }
        }

        if(onGround)
        {
            if(MathF.Sign(velocity.X) != 0 && !level.TileMeeting(Hitbox.Shift(MathF.Sign(velocity.X), 0)))
            {
                if(legFrame < 6) legFrame = 6;
                legFrame = MathUtil.Approach(legFrame, 20, MathF.Abs(velocity.X) / 4f);
                if(legFrame >= 20) legFrame = 6;
            }
            else
            {
                legFrame = swinging ? armFrame : 0;
            }
            if(!swinging) armFrame = legFrame;
        }
        else
        {
            legFrame = 1;
            if(!swinging) armFrame = 1;
        }

        if(swinging)
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
            if(!onGround && velocity.Y > -0.75f && velocity.Y < 0.3f)
            {
                bodyFrame = 0;
            }
        }
    }

    void UseItem()
    {
        
    }

    void TryPlaceItem()
    {
        if(!TileWithinReach(Main.MouseWorldPos.ToVector2())) return;

        var mousePos = MathUtil.Snap(Main.MouseWorldPos.ToVector2(), Level.tileSize);
        var mouseTilePos = (mousePos / Level.tileSize).ToPoint();

        if(!level.InWorld(mouseTilePos)) return;

        var mouseTile = level.GetTileAtPosition(mousePos);

        var itemDef = HeldItem.GetDef();
        var tileDef = mouseTile.GetDef();

        bool placeable = false;

        if(mouseTile is not null)
        {
            if(mouseTile.id == "air")
            {
                if(level.TileMeeting(new((mousePos - Vector2.UnitX * 5).ToPoint(), new(Level.tileSize + 10, Level.tileSize)))
                || level.TileMeeting(new((mousePos - Vector2.UnitY * 5).ToPoint(), new(Level.tileSize, Level.tileSize + 10))))
                {
                    placeable = !Hitbox.Intersects(new(mousePos.ToPoint(), new(Level.tileSize)));
                }
            }
        }

        if(placeable)
        {
            var tileItem = HeldItem.GetDef<Registries.Item.TileItemDef>();
            var tileItemDef = tileItem.GetTileDef();

            HeldItem.Stacks--;

            tileItem.CreateTile(level, mouseTilePos);

            var e = new TileEvent(mouseTilePos, this, tileItem.id);

            int ind = Random.Shared.NextWithinRange(0, 3);
            Registry.GetSound("dig_" + ind)?.Play();

            tileItemDef.OnPlace(e);
            Main.GlobalEvents.DoTilePlace(e);

            level.UpdateNeighbors(e);
        }
    }

    void TryUseTool()
    {
        if(!TileWithinReach(Main.MouseWorldPos.ToVector2())) return;

        var mousePos = MathUtil.Snap(Main.MouseWorldPos.ToVector2(), Level.tileSize);
        var mouseTilePos = (mousePos / Level.tileSize).ToPoint();

        if(!level.InWorld(mouseTilePos)) return;

        var mouseTile = level.GetTileAtPosition(mousePos);

        var itemDef = HeldItem.GetDef();
        var tileDef = mouseTile.GetDef();

        bool mineable = false;

        if(mouseTile is not null)
        {
            if(mouseTile.id != "air")
            {
                mineable = tileDef.settings.mineable;
            }
        }

        if(mineable)
        {
            if(itemDef.settings.pickaxePower >= tileDef.settings.minimumPickaxePower)
            {
                mouseTile.breakingProgress += (itemDef.settings.pickaxePower / 100f) / tileDef.settings.hardness;
                if(mouseTile.breakingProgress > 1) mouseTile.breakingProgress = 1;

                int ind = Random.Shared.NextWithinRange(0, 3);
                switch(tileDef.settings.soundType)
                {
                    case TileSoundType.Default:
                        Registry.GetSound("dig_" + ind)?.Play();
                        break;
                    case TileSoundType.Stone:
                        Registry.GetSound("tink_" + ind)?.Play();
                        break;
                }
            }

            if(mouseTile.breakingProgress >= 1)
            {
                var e = new TileEvent(mouseTilePos, this, mouseTile.id);

                tileDef.Destroy(e);
                Main.GlobalEvents.DoTileDestroy(e);

                level.SetTile("air", mouseTilePos);

                level.UpdateNeighbors(e);
            }
        }
    }

    public override void OnCollisionX()
    {
        if(inputDir != 0 && inputDir == MathF.Sign(velocity.X) && !Input.Get(Keys.S) && (!level.TileMeeting(Hitbox.Shift(inputDir, -4)) || !level.TileMeeting(Hitbox.Shift(inputDir, -8))))
        {
            for(int i = 0; i < 8; i++)
            {
                Move(new(0, -1), false, true);
                if(!level.TileMeeting(Hitbox.Shift(inputDir, 0)))
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

        var settings = HeldItem.GetDef().settings;

        if(settings.tool || settings.placeable) return true;

        return false;
    }

    public bool IsHoldingPlaceable()
    {
        if(HeldItem is null) return false;

        var settings = HeldItem.GetDef().settings;

        if(settings.placeable) return true;

        return false;
    }

    public bool TileWithinReach(Vector2 position)
    {
        var distance = Vector2.Distance(
            MathUtil.Snap(Center.ToVector2(), Level.tileSize),
            MathUtil.Snap(position, Level.tileSize)
        );

        int reach = _baseReach + (HeldItem?.GetDef().settings.rangeBonus ?? 0);

        return distance <= _baseReach * Level.tileSize;
    }
}
