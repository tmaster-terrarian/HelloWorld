using System.Collections.Generic;

using Microsoft.Xna.Framework;

using HelloWorld.Events;
using System;

namespace HelloWorld.Registries.Tile;

[System.Serializable]
public class TileOptions
{
    public bool isAir = false;

    public static TileOptions Default => new TileOptions();
}

public class TileDef : IRegistryEntry
{
    public readonly TileOptions settings;

    public TileDef(string id, TileOptions? settings)
    {
        this.ID = id;
        this.settings = settings ?? TileOptions.Default;

        Main.GlobalEvents.onTilePlace += GlobalEvents_onPlace;
        Main.GlobalEvents.onTileUpdated += GlobalEvents_onUpdated;
    }

    public virtual void GlobalEvents_onPlace(TileEvent e) {}

    public virtual void GlobalEvents_onUpdated(TileEvent e) {}
}

public class TileRegistry : GenericRegistry<TileDef>
{
    public static readonly TileDef AIR = new TileDef("air", new TileOptions{isAir = true});
    public static readonly TileDef STONE = new TileDef("stone", null);

    public override void Register()
    {
        Registry.RegisterTile(AIR);
        Registry.RegisterTile(STONE);
    }
}
