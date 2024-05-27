using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using HelloWorld.Registries.Item;
using HelloWorld.Registries.Sound;
using HelloWorld.Registries.Tile;
using System;

namespace HelloWorld.Registries;

public abstract class AbstractRegistryDef
{
    public string id { get; set; }

    public override string ToString()
    {
        return $"{GetType()} {id}";
    }
}

public static class Registry
{
    public static readonly Dictionary<string, TileDef> TileRegistry = new();
    public static readonly Dictionary<string, SoundDef> SoundRegistry = new();
    public static readonly Dictionary<string, ItemDef> ItemRegistry = new();

    public static void RegisterTile(string id, TileDef entry)
    {
        entry.id = id;
        if(!TileRegistry.ContainsKey(id))
            TileRegistry.Add(id, entry);
    }

    public static void RegisterSound(string id, SoundDef entry)
    {
        entry.id = id;
        entry.sound = Main.GetAsset<SoundEffect>("Audio/Sfx/" + id);

        if(!SoundRegistry.ContainsKey(id))
            SoundRegistry.Add(id, entry);
    }

    public static void RegisterItem(string id, ItemDef entry)
    {
        entry.id = id;
        if(!ItemRegistry.ContainsKey(id))
            ItemRegistry.Add(id, entry);
    }

    public static TileDef GetTile(string id)
    {
        return TileRegistry.GetValueOrDefault(id) ?? TileRegistry["air"];
    }

    public static SoundDef? GetSound(string id)
    {
        return SoundRegistry.GetValueOrDefault(id);
    }

    public static ItemDef GetItem(string id)
    {
        return ItemRegistry.GetValueOrDefault(id) ?? ItemRegistry["missing"];
    }
}

public abstract class GenericRegistry<T> where T : AbstractRegistryDef
{
    public abstract void Register();

    public Type DefType => typeof(T);
}
