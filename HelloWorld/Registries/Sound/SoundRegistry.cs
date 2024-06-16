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
    public override void Register()
    {
        Registry.RegisterSound("playerLand", new SoundDef(new SoundSettings { volume = 0.25f }));
        Registry.RegisterSound("playerJump", new SoundDef(new SoundSettings { volume = 0.5f }));

        Registry.RegisterSound("swing", new SoundDef(new SoundSettings { volume = 0.5f }));
        Registry.RegisterSound("grab", new SoundDef(new SoundSettings { volume = 1 }));

        Registry.RegisterSound("dig_0", new SoundDef(new SoundSettings { volume = 1 }));
        Registry.RegisterSound("dig_1", new SoundDef(new SoundSettings { volume = 1 }));
        Registry.RegisterSound("dig_2", new SoundDef(new SoundSettings { volume = 1 }));
        Registry.RegisterSound("tink_0", new SoundDef(new SoundSettings { volume = 1 }));
        Registry.RegisterSound("tink_1", new SoundDef(new SoundSettings { volume = 1 }));
        Registry.RegisterSound("tink_2", new SoundDef(new SoundSettings { volume = 1 }));
    }
}
