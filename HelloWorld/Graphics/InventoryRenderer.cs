using System;
using HelloWorld.Registries.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelloWorld.Graphics;

public class InventoryRenderer
{
    private PlayerInventory _inventory;

    private Texture2D slotTexture;

    public InventoryRenderer(PlayerInventory inventory)
    {
        _inventory = inventory;
    }

    public void LoadContent()
    {
        slotTexture = Main.GetAsset<Texture2D>("Images/UI/slot");
    }

    public void Draw(SpriteBatch _spriteBatch)
    {
        for(var i = 0; i < 10; i++)
        {
            _spriteBatch.Draw(slotTexture, new Vector2(2 + i * 20, 2), null, Color.White * (0.5f + (Main.Player.HeldItemSlot == i ? 0.2f : 0)), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
        for(var i = 0; i < 10; i++)
        {
            var item = _inventory[i];
            if(item is null) continue;

            var def = item.GetDef();

            var tex = Main.GetAsset<Texture2D>(def.GetTexturePath());
            if(tex is null) continue;

            DrawItem(def, item, tex, i, _spriteBatch);
        }
    }

    void DrawItem(ItemDef def, ItemStack item, Texture2D tex, int index, SpriteBatch _spriteBatch)
    {
        Rectangle? rect = null;
        var size = 14f / MathHelper.Max(14, tex.Width);
        var pivot = new Vector2(tex.Width / 2f, tex.Height / 2f);

        _spriteBatch.Draw(tex, new Vector2(11 + index * 20, 11), rect, Color.White, 0, pivot, size, SpriteEffects.None, 0);

        if(item.Stacks > 1)
        {
            _spriteBatch.DrawString(Main.Font, item.Stacks.ToString(), new Vector2(2 + index * 20, 12), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
    }
}
