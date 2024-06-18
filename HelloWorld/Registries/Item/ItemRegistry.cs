using Microsoft.Xna.Framework;

using HelloWorld.Core;
using HelloWorld.Registries.Tile;

namespace HelloWorld.Registries.Item;

[System.Serializable]
public class ItemOptions
{
    public int maxStack = 999;

    public ItemRarity rarity = ItemRarity.Common;

    public bool placeable = false;
    public bool tool = false;

    public UseStyle useStyle = UseStyle.None;
    public int useTime = 40;
    public int toolSpeed = 15;
    public int placementSpeed = 15;
    public int rangeBonus = 0;

    public int pickaxePower = 0;

    public DamageType damageType = DamageType.Harmless;
    public int damage = 0;

    public static ItemOptions Default => new ItemOptions();
    public static ItemOptions DefaultTile => new ItemOptions { placeable = true, useTime = 15, useStyle = UseStyle.Swing };
    public static ItemOptions DefaultTool => new ItemOptions { tool = true, maxStack = 1, useStyle = UseStyle.Swing };
}

public class ItemOptionsBuilder
{
    private readonly ItemOptions options = ItemOptions.Default;

    public ItemOptionsBuilder() {}

    public ItemOptionsBuilder(ItemOptions options)
    {
        this.options = options;
    }

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

    public ItemOptionsBuilder Tile(bool value = true)
    {
        options.placeable = value;
        return this;
    }

    public ItemOptionsBuilder Tool(bool value = true)
    {
        options.tool = value;
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

    public TileItemDef() : this(ItemOptions.DefaultTile) {}

    public virtual void CreateTile(Level level, Point position)
    {
        level.SetTile(id, position);
    }

    public TileDef GetTileDef()
    {
        return Registry.GetTile(id);
    }
}

public class ToolItemDef : ItemDef
{
    private ToolType type;

    public ToolType ToolType => type;

    public ToolItemDef(ItemOptions settings, ToolType type) : base(settings)
    {
        this.type = type;
    }

    public ToolItemDef(ToolType type) : this(ItemOptions.DefaultTool, type) {}
}

public class ItemRegistry : GenericRegistry<ItemDef>
{
    public static readonly ItemDef MISSING = new ItemDef(ItemOptions.Default);

    public override void Register()
    {
        Registry.RegisterItem("missing", MISSING);
        Registry.RegisterItem("iron_pickaxe", CreatePickaxe(40, 5, 20, 13));
        Registry.RegisterItem("dirt", new TileItemDef());
        Registry.RegisterItem("stone", new TileItemDef());
        Registry.RegisterItem("brick", new TileItemDef());
        Registry.RegisterItem("copper_pickaxe", CreatePickaxe(35, 4, 23, 15));
    }

    static ToolItemDef CreatePickaxe(int pickaxePower, int damage, int useTime = 30, int toolSpeed = 15, DamageType damageType = DamageType.Melee, UseStyle useStyle = UseStyle.Swing, int rangeBonus = 0)
    {
        var options = ItemOptions.DefaultTool;

        options.pickaxePower = pickaxePower;
        options.useTime = useTime;
        options.useStyle = useStyle;
        options.toolSpeed = toolSpeed;
        options.rangeBonus = rangeBonus;

        if(damage > 0)
        {
            options.damageType = damageType;
            options.damage = damage;
        }

        return new ToolItemDef(options, ToolType.Pickaxe);
    }
}
