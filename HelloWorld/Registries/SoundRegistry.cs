using System.Collections.Generic;

using Microsoft.Xna.Framework.Audio;

namespace HelloWorld.Registries.Sound;

[System.Serializable]
public class SoundSettings
{
    public float volume = 1f;

    public static SoundSettings Default => new SoundSettings();
}

public class SoundDef : IRegistryEntry
{
    public readonly SoundSettings settings;
    public SoundEffect sound = null;

    public SoundDef(string id, SoundSettings? settings)
    {
        this.ID = id;
        this.settings = settings ?? SoundSettings.Default;
    }

    public SoundEffectInstance Play()
    {
        if(sound == null) return null;

        var inst = sound.CreateInstance();
        inst.Volume = settings.volume;
        inst.Play();

        return inst;
    }
}

public class SoundRegistry : GenericRegistry<SoundDef>
{
    public static readonly SoundDef PLAYER_JUMP = new SoundDef("playerJump", new SoundSettings{volume = 0.25f});
    public static readonly SoundDef PLAYER_LAND = new SoundDef("playerLand", new SoundSettings{volume = 0.5f});

    static SoundEffect LoadSfx(string id) => Main.ContentManager.Load<SoundEffect>("Audio/Sfx/" + id);

    public override void Register()
    {
        Registry.RegisterSound(PLAYER_JUMP, LoadSfx(PLAYER_JUMP.ID));
        Registry.RegisterSound(PLAYER_LAND, LoadSfx(PLAYER_LAND.ID));
    }
}
