using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace HelloWorld.Registries;

public class IRegistryEntry
{
    public string ID { get; set; }
}

public static class Registry
{
    public static readonly Dictionary<string, TileRegistryEntry> TileRegistry = new();
    public static readonly Dictionary<string, SoundRegistryEntry> SoundRegistry = new();
    public static readonly Dictionary<string, ItemRegistryEntry> ItemRegistry = new();

    public static void RegisterTile(TileRegistryEntry entry)
    {
        if(!TileRegistry.ContainsKey(entry.ID))
            TileRegistry.Add(entry.ID, entry);
    }

    public static void RegisterSound(SoundRegistryEntry entry, SoundEffect soundEffect)
    {
        entry.sound = soundEffect;

        if(!SoundRegistry.ContainsKey(entry.ID))
            SoundRegistry.Add(entry.ID, entry);
    }

    public static void RegisterItem(ItemRegistryEntry entry)
    {
        if(!ItemRegistry.ContainsKey(entry.ID))
            ItemRegistry.Add(entry.ID, entry);
    }

    public static TileRegistryEntry GetTile(string id)
    {
        return TileRegistry.GetValueOrDefault(id);
    }

    public static SoundRegistryEntry GetSound(string id)
    {
        return SoundRegistry.GetValueOrDefault(id);
    }

    public static ItemRegistryEntry GetItem(string id)
    {
        return ItemRegistry.GetValueOrDefault(id) ?? ItemRegistry["Missing"];
    }
}

public abstract class GenericRegistry<T> where T : IRegistryEntry
{
    public abstract Dictionary<string, T> registry { get; }

    public abstract void Register();

    public T Get(string id)
    {
        try
        {
            return registry[id];
        }
        catch(KeyNotFoundException exception)
        {
            System.Console.WriteLine(exception.ToString());
        }
        return default;
    }
}
