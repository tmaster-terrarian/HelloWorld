using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace HelloWorld.Registries;

[System.Serializable]
public class SoundSettings
{
    public float volume = 1f;

    public static SoundSettings Default => new SoundSettings();
}

public class SoundRegistryEntry : IRegistryEntry
{
    public readonly SoundSettings settings;
    public SoundEffect sound = null;

    public SoundRegistryEntry(string id, SoundSettings settings)
    {
        this.ID = id;
        this.settings = settings!;
    }

    public SoundRegistryEntry(string id) : this(id, SoundSettings.Default) {}

    public SoundEffectInstance Play()
    {
        if(sound == null) return null;

        var inst = sound.CreateInstance();
        inst.Volume = settings.volume;
        inst.Play();

        return inst;
    }
}

public class SoundRegistry : GenericRegistry<SoundRegistryEntry>
{
    public override Dictionary<string, SoundRegistryEntry> registry => Registry.SoundRegistry;

    readonly SoundRegistryEntry PLAYER_JUMP = new SoundRegistryEntry("playerJump", new SoundSettings{volume = 0.25f});
    readonly SoundRegistryEntry PLAYER_LAND = new SoundRegistryEntry("playerLand", new SoundSettings{volume = 0.5f});

    private SoundEffect LoadSfx(SoundRegistryEntry entry) => Main.ContentManager.Load<SoundEffect>("Audio/Sfx/" + entry.ID);

    public override void Register()
    {
        Registry.RegisterSound(PLAYER_JUMP, LoadSfx(PLAYER_JUMP));
        Registry.RegisterSound(PLAYER_LAND, LoadSfx(PLAYER_LAND));
    }
}
