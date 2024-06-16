using System.Collections.Generic;

using Microsoft.Xna.Framework;

using HelloWorld.Events;
using HelloWorld.Core;

namespace HelloWorld.Registries.Tile;

[System.Serializable]
public class TileOptions
{
    public bool isAir = false;

    public bool dropsLoot = false;

    public bool mineable = true;

    public int minimumPickaxePower = 0;
    public float hardness = 1;

    public TileSoundType soundType = TileSoundType.Default;

    public static TileOptions Default => new TileOptions();
    public static TileOptions DefaultDropsLoot => new TileOptions { dropsLoot = true };

    public static TileOptions Air => new TileOptions { dropsLoot = false, mineable = false, isAir = true };
}

public class TileDef : AbstractRegistryDef
{
    public readonly TileOptions settings;

    public TileDef(TileOptions settings)
    {
        this.settings = settings;
    }

    public virtual void OnPlace(TileEvent e) {}

    public virtual void OnUpdate(TileEvent e) {}

    protected virtual void OnDestroy(TileEvent e) {}

    public void Destroy(TileEvent e)
    {
        if(settings.dropsLoot) DropLoot(e);
        OnDestroy(e);
    }

    protected virtual void DropLoot(TileEvent e)
    {
        HelloWorld.Item.Drop(id, new Point(e.TilePos.X * Level.tileSize + Level.tileSize / 2, e.TilePos.Y * Level.tileSize + Level.tileSize / 2), 1);
    }
}

public class TileRegistry : GenericRegistry<TileDef>
{
    public static readonly TileDef AIR = new TileDef(TileOptions.Air);

    public override void Register()
    {
        Registry.RegisterTile("air", AIR);
        Registry.RegisterTile("stone", CreateStoney());
        Registry.RegisterTile("brick", CreateStoney());
        Registry.RegisterTile("dirt", CreateDirty());
    }

    static TileDef CreateStoney()
    {
        return new TileDef(new TileOptions { dropsLoot = true, hardness = 2, soundType = TileSoundType.Stone });
    }

    static TileDef CreateDirty()
    {
        return new TileDef(new TileOptions { dropsLoot = true, hardness = 1 });
    }
}
