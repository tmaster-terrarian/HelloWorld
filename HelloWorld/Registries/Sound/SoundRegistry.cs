using System.Collections.Generic;

using Microsoft.Xna.Framework.Audio;

namespace HelloWorld.Registries.Sound;

[System.Serializable]
public class SoundSettings
{
    public float volume = 1f;

    public static SoundSettings Default => new SoundSettings();
}

public class SoundDef : AbstractRegistryDef
{
    public readonly SoundSettings settings;
    public SoundEffect sound = null;

    public SoundDef(SoundSettings? settings)
    {
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
    public static readonly SoundDef PLAYER_JUMP = new SoundDef(new SoundSettings{volume = 0.25f});
    public static readonly SoundDef PLAYER_LAND = new SoundDef(new SoundSettings{volume = 0.5f});

    public override void Register()
    {
        Registry.RegisterSound("playerLand", PLAYER_JUMP);
        Registry.RegisterSound("playerJump", PLAYER_LAND);
    }
}
