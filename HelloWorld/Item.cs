using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HelloWorld.Registries;
using HelloWorld.Registries.Item;
using HelloWorld.Core;
using System;

namespace HelloWorld;

public class Item : Entity
{
    static readonly List<Item> items = new();
    static readonly List<Item> toRemove = new();

    public string id;
    public int stacks = 1;

    public int pickupRange = 32;

    private int animFrames = 1;
    private int frameIndex = 0;
    private int lifetime = 0;

    private float pickupCooldown = 0;

    private bool _sizeDirty = true;

    ItemStack _stack;

    Item(string id, int stacks)
    {
        this.id = id;
        this.stacks = stacks;
        this._stack = new(id, stacks);

        width = 8;
        height = 8;
    }

    public static void UpdateAll()
    {
        for(int i = 0; i < items.Count; i++)
        {
            Item item = items[i];

            if(item._stack.Stacks <= 0)
            {
                items.Remove(item);
                i--;
                continue;
            }

            if(item.pickupCooldown > 0) item.pickupCooldown--;

            var player = Main.NearestPlayerTo(item.position);

            float dx = player.Center.X - item.Center.X;
            float dy = player.Center.Y - item.Center.Y;

            float distance = MathF.Sqrt(dx*dx + dy*dy);
            if(dx == 0 && dy == 0) distance = 1;

            if(item.pickupCooldown <= 0 && distance <= item.pickupRange && player.Inventory.CanInsert(item._stack))
            {
                item._noTileCollision = true;
                item.pickupRange = 64;

                item.velocity.X = MathUtil.Approach(item.velocity.X, dx / distance * 3f, 0.2f);
                item.velocity.Y = MathUtil.Approach(item.velocity.Y, dy / distance * 3f, 0.2f);
            }
            else
            {
                item._noTileCollision = false;
                item.pickupRange = 32;

                item.velocity.X = MathUtil.Approach(item.velocity.X, 0f, 0.05f);
                if(!item.level.TileMeeting(item.Hitbox.Shift(0, 1)))
                {
                    item.velocity.Y = MathUtil.Approach(item.velocity.Y, 20f, 0.1f);

                    if(item.position.Y > (item.level.Bounds.Height + 1) * Level.tileSize)
                    {
                        item.velocity.Y -= 0.4f;
                        if(item.position.Y > (item.level.Bounds.Height + 2) * Level.tileSize)
                            item.position.Y = (item.level.Bounds.Height + 2) * Level.tileSize;
                    }
                }

                if(item.lifetime % 2 == 0)
                if(AttemptMergeWithOthers(item, out Item remove))
                {
                    toRemove.Add(remove);
                }
            }

            if(toRemove.Contains(item))
                continue;

            item.Move(item.velocity);

            item.angle = MathHelper.Clamp(item.velocity.X * 0.375f, -1.24f, 1.24f);

            if(item.pickupCooldown <= 0 && item.Hitbox.Intersects(player.Hitbox))
            {
                int stacks = item._stack.Stacks;
                if(player.Inventory.TryInsert(ref item._stack))
                {
                    Registry.GetSound("grab")?.Play();
                    items.Remove(item);
                    i--;
                    continue;
                }
                if(stacks != item._stack.Stacks) Registry.GetSound("grab")?.Play();
            }

            item.lifetime++;
        }

        if(toRemove.Count > 0)
        {
            foreach(var item in toRemove)
            {
                items.Remove(item);
            }
            toRemove.Clear();
        }
    }

    static bool AttemptMergeWithOthers(Item item, out Item remove)
    {
        bool ret = false;
        remove = null;

        ItemDef def = Registry.GetItem(item.id);
        for(int i = 0; i < items.Count; i++)
        {
            Item other = items[i];
            if(ReferenceEquals(other, item)) continue;

            if(
                (other.Hitbox.Intersects(item.Hitbox) || Vector2.Distance(item.Center.ToVector2(), other.Center.ToVector2()) < 10)
                && other.id == item.id
                && other._stack.Stacks < def.settings.maxStack
                && other._stack.Stacks >= item._stack.Stacks
            )
            {
                var diffCount = MathHelper.Min(item._stack.Stacks, def.settings.maxStack - other._stack.Stacks);

                if(diffCount <= 0)
                {
                    ret = true;
                    remove = item;
                }

                var newItem = items[i] = _Drop(item.id, other.Center, other._stack.Stacks + diffCount);

                newItem.velocity = (newItem.velocity * 0.5f) + ((item.velocity + other.velocity) / 4f) + Vector2.UnitY * 0.12f;

                item._stack.Stacks -= diffCount;

                if(item.pickupCooldown > 0 || other.pickupCooldown > 0)
                    newItem.pickupCooldown = (item.pickupCooldown + other.pickupCooldown) / 2f;

                break;
            }
        }
        return ret;
    }

    public static void DrawAll(SpriteBatch _spriteBatch)
    {
        foreach(Item item in items)
        {
            var texture = Main.GetAsset<Texture2D>(Registry.GetItem(item.id).GetTexturePath());
            int frameSize = texture.Width / item.animFrames;
            if(item._sizeDirty)
            {
                item._sizeDirty = false;
                item.width = frameSize;
                item.height = texture.Height;
            }
            _spriteBatch.Draw(
                texture,
                item.Center.ToVector2(), new Rectangle(item.frameIndex * item.width, 0, item.width, item.height),
                Color.White,
                item.angle, new(item.width / 2, item.height / 2),
                1, SpriteEffects.None, 0
            );
        }
    }

    static Item _Drop(string id, Point position, int stacks, float pickupCooldown = 0, Vector2? velocity = null)
    {
        return new(id, stacks)
        {
            velocity = velocity ?? new((System.Random.Shared.NextSingle() * 4f) - 2, (System.Random.Shared.NextSingle() * -1) - 0.7f),
            Center = position,
            pickupCooldown = pickupCooldown,
        };
    }

    public static void Drop(string id, Point position, int stacks, float pickupCooldown = 0, Vector2? velocity = null)
    {
        items.Add(_Drop(id, position, stacks, pickupCooldown, velocity));
    }
}

public class ItemStack
{
    private int _stacks;
    bool invalid = false;

    public string id;
    public int Stacks {
        get => _stacks;
        set {
            if(value <= 0) invalid = true;
            _stacks = value;
        }
    }

    public ItemStack(string id, int stacks)
    {
        this.id = id;
        this._stacks = stacks;
    }

    public ItemStack(string id) : this(id, 1) {}

    public ItemDef GetDef()
    {
        return Registry.GetItem(id);
    }

    public T GetDef<T>() where T : ItemDef
    {
        return (T)Registry.GetItem(id);
    }

    public ItemStack? Verify()
    {
        if(invalid) return null;
        return this;
    }

    public static bool operator ==(ItemStack a, ItemStack b)
    {
        if(ReferenceEquals(a, b)) return true;
        if(a is null || b is null) return false; // null == null will also return false!
        return a.id == b.id;
    }

    public static bool operator !=(ItemStack a, ItemStack b)
    {
        if(ReferenceEquals(a, b)) return false;
        if(a is null && b is null) return false;
        if(a is null || b is null) return true;
        return a.id != b.id;
    }

    public override bool Equals(object obj)
    {
        if(obj is null)
        {
            return false;
        }

        if(ReferenceEquals(this, obj))
        {
            return true;
        }

        if(obj is ItemStack stack)
        {
            return stack.id == id;
        }

        return false;
    }

    public override string ToString()
    {
        return $"{id} x{Stacks}";
    }
}
