using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using HelloWorld.Events;
using HelloWorld.Core;

namespace HelloWorld.Registries.Item;

[Serializable]
public class ItemOptions
{
    public int maxStack = 999;
    public ItemRarity rarity = ItemRarity.Common;
    public bool pickaxe = false;
    public bool tileItem = false;

    public static ItemOptions Default => new ItemOptions();
}

public class ItemOptionsBuilder
{
    private readonly ItemOptions options = ItemOptions.Default;

    public ItemOptions build()
    {
        return options;
    }

    public ItemOptionsBuilder MaxStacks(int stacks)
    {
        options.maxStack = stacks;
        return this;
    }

    public ItemOptionsBuilder Rarity(ItemRarity rarity)
    {
        options.rarity = rarity;
        return this;
    }

    public ItemOptionsBuilder Pickaxe(bool value = true)
    {
        options.pickaxe = value;
        return this;
    }

    public ItemOptionsBuilder Tile(bool value = true)
    {
        options.tileItem = value;
        return this;
    }
}

public class ItemDef : IRegistryEntry
{
    public readonly ItemOptions settings;

    public ItemDef(string id, ItemOptions settings = null)
    {
        this.ID = id;
        this.settings = settings ?? ItemOptions.Default;
    }

    public string GetTexturePath()
    {
        return "Images/Item/" + ID;
    }
}

public class TileItemDef : ItemDef
{
    public TileItemDef(string id, ItemOptions settings) : base(id, settings) {}

    public virtual void CreateTile(Level level, Point position)
    {
        level.SetTile(ID, position);
    }
}

public class ItemRegistry : GenericRegistry<ItemDef>
{
    public static readonly ItemDef MISSING = new("Missing");

    public static readonly ItemDef IRON_PICKAXE = new("IronPickaxe",
        new ItemOptionsBuilder().MaxStacks(1).Pickaxe().build()
    );

    public static readonly TileItemDef STONE = new("Stone",
        new ItemOptionsBuilder().Tile().build()
    );

    public override void Register()
    {
        Registry.RegisterItem(MISSING);
        Registry.RegisterItem(IRON_PICKAXE);
        Registry.RegisterItem(STONE);
    }
}
