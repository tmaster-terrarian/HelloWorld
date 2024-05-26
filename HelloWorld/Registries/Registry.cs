using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using HelloWorld.Registries.Sound;
using HelloWorld.Registries.Tile;
using HelloWorld.Registries.Item;

namespace HelloWorld.Registries;

public class IRegistryEntry
{
    public string ID { get; set; }
}

public static class Registry
{
    public static readonly Dictionary<string, TileDef> TileRegistry = new();
    public static readonly Dictionary<string, SoundDef> SoundRegistry = new();
    public static readonly Dictionary<string, ItemDef> ItemRegistry = new();

    public static void RegisterTile(TileDef entry)
    {
        if(!TileRegistry.ContainsKey(entry.ID))
            TileRegistry.Add(entry.ID, entry);
    }

    public static void RegisterSound(SoundDef entry, SoundEffect soundEffect)
    {
        entry.sound = soundEffect;

        if(!SoundRegistry.ContainsKey(entry.ID))
            SoundRegistry.Add(entry.ID, entry);
    }

    public static void RegisterItem(ItemDef entry)
    {
        if(!ItemRegistry.ContainsKey(entry.ID))
            ItemRegistry.Add(entry.ID, entry);
    }

    public static TileDef GetTile(string id)
    {
        return TileRegistry.GetValueOrDefault(id);
    }

    public static SoundDef GetSound(string id)
    {
        return SoundRegistry.GetValueOrDefault(id);
    }

    public static ItemDef GetItem(string id)
    {
        return ItemRegistry.GetValueOrDefault(id) ?? ItemRegistry["Missing"];
    }
}

public abstract class GenericRegistry<T> where T : IRegistryEntry
{
    public abstract void Register();
}
