using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace HelloWorld.Registries;

[System.Serializable]
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
    private ItemOptions options = ItemOptions.Default;

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

    public ItemOptionsBuilder TileItem(bool value = true)
    {
        options.tileItem = value;
        return this;
    }
}

public class ItemRegistryEntry : IRegistryEntry
{
    public readonly ItemOptions settings;

    public ItemRegistryEntry(string id, ItemOptions settings)
    {
        this.ID = id;
        this.settings = settings!;
    }

    public ItemRegistryEntry(string id) : this(id, ItemOptions.Default) {}

    public string GetTexturePath()
    {
        if(settings.tileItem) return "Images/Tiles/" + ID;

        return "Images/Item/" + ID;
    }
}

public class ItemRegistry : GenericRegistry<ItemRegistryEntry>
{
    public override Dictionary<string, ItemRegistryEntry> registry => Registry.ItemRegistry;

    readonly ItemRegistryEntry IRON_PICKAXE = new ItemRegistryEntry("IronPickaxe",
        new ItemOptionsBuilder().MaxStacks(1).Pickaxe().build()
    );

    readonly ItemRegistryEntry STONE = new ItemRegistryEntry("stone",
        new ItemOptionsBuilder().TileItem().build()
    );

    public override void Register()
    {
        Registry.RegisterItem(IRON_PICKAXE);
        Registry.RegisterItem(STONE);
    }
}
