using System.Collections.Generic;

using Microsoft.Xna.Framework;

using HelloWorld.Events;

namespace HelloWorld.Registries.Tile;

[System.Serializable]
public class TileOptions
{
    public bool isAir = false;

    public static TileOptions Default => new TileOptions();
}

public class TileDef : AbstractRegistryDef
{
    public readonly TileOptions settings;

    public TileDef(TileOptions? settings)
    {
        this.settings = settings ?? TileOptions.Default;
    }

    public virtual void OnPlace(TileEvent e) {}

    public virtual void OnUpdate(TileEvent e) {}

    public virtual void OnDestroy(TileEvent e) {}
}

public class TileRegistry : GenericRegistry<TileDef>
{
    public static readonly TileDef AIR = new TileDef(new TileOptions{isAir = true});
    public static readonly TileDef STONE = new TileDef(null);
    public static readonly TileDef BRICK = new TileDef(null);

    public override void Register()
    {
        Registry.RegisterTile("air", AIR);
        Registry.RegisterTile("stone", STONE);
        Registry.RegisterTile("brick", BRICK);
    }
}
