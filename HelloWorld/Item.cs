using Microsoft.Xna.Framework;

using HelloWorld.Registries;
using HelloWorld.Registries.Item;

namespace HelloWorld;

public class Item : Entity
{
    public string id;
    public int stacks;

    public int pickupRange = 48;

    public Item(string id)
    {
        this.id = id;
    }

    public static void Update(Item[] items)
    {
        foreach(var item in items)
        {
            var player = Main.NearestPlayerTo(item.position);
        }
    }
}

public class ItemStack
{
    public string id;
    public int stacks;

    public ItemStack(string id, int stacks)
    {
        this.id = id;
        this.stacks = stacks;
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

    public static ItemStack operator +(ItemStack a, ItemStack b)
    {
        if(a.id != b.id) throw new System.Exception(nameof(ItemStack) + " IDs must match");

        return new ItemStack(a.id, a.stacks + b.stacks);
    }

    public override bool Equals(object obj)
    {
        if(ReferenceEquals(this, obj))
        {
            return true;
        }

        if(obj is null)
        {
            return false;
        }

        if(obj is ItemStack stack)
        {
            return stack.id == id;
        }

        return false;
    }
}
