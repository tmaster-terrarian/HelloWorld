using Microsoft.Xna.Framework;

using HelloWorld.Core;
using HelloWorld.Registries.Tile;

namespace HelloWorld.Registries.Item;

[System.Serializable]
public class ItemOptions
{
    public int maxStack = 999;
    public ItemRarity rarity = ItemRarity.Common;
    public bool pickaxe = false;
    public bool tileItem = false;

    public static ItemOptions Default => new ItemOptions();
    public static ItemOptions DefaultTile => new ItemOptions { tileItem = true };
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

public class ItemDef : AbstractRegistryDef
{
    public readonly ItemOptions settings;

    public ItemDef(ItemOptions settings)
    {
        this.settings = settings;
    }

    public virtual string GetTexturePath()
    {
        return "Images/Item/" + id;
    }
}

public class TileItemDef : ItemDef
{
    public TileItemDef(ItemOptions settings) : base(settings) {}

    public virtual void CreateTile(Level level, Point position)
    {
        level.SetTile(id, position);
    }

    public TileDef GetTileDef()
    {
        return Registry.GetTile(id);
    }
}

public class ItemRegistry : GenericRegistry<ItemDef>
{
    public static readonly ItemDef MISSING = new ItemDef(ItemOptions.Default);

    public static readonly ItemDef IRON_PICKAXE = new ItemDef(new ItemOptions{ maxStack = 1, pickaxe = true });

    public static readonly TileItemDef STONE = new TileItemDef(ItemOptions.DefaultTile);

    public override void Register()
    {
        Registry.RegisterItem("missing", MISSING);
        Registry.RegisterItem("iron_pickaxe", IRON_PICKAXE);
        Registry.RegisterItem("stone", STONE);
    }
}
