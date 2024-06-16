using Microsoft.Xna.Framework;

namespace HelloWorld;

public enum ItemRarity
{
    Common
}

public static class ItemRarityColor
{
    public static readonly Color Common = Color.White;

    public static Color get(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => Common,
            _ => Common
        };
    }
}

public enum ToolType
{
    Pickaxe
}

public enum DamageType
{
    Harmless,
    Universal,
    Melee
}

public enum UseStyle
{
    None,
    Swing
}
