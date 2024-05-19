using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace HelloWorld.Registries;

[System.Serializable]
public class TileOptions
{
    public bool isAir;

    public static TileOptions Default => new TileOptions
    {
        isAir = false,
    };
}

public class TileRegistryEntry : IRegistryEntry
{
    public readonly TileOptions settings;

    public TileRegistryEntry(string id, TileOptions settings)
    {
        this.ID = id;
        this.settings = settings!;
    }

    public TileRegistryEntry(string id) : this(id, TileOptions.Default) {}
}

public class TileRegistry : GenericRegistry<TileRegistryEntry>
{
    public override Dictionary<string, TileRegistryEntry> registry => Registry.TileRegistry;

    readonly TileRegistryEntry AIR = new TileRegistryEntry("air", new TileOptions{isAir = true});
    readonly TileRegistryEntry STONE = new TileRegistryEntry("stone");

    public override void Register()
    {
        Registry.RegisterTile(AIR);
        Registry.RegisterTile(STONE);
    }
}
