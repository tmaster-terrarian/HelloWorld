using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelloWorld.Graphics;

public class InventoryRenderer : IDrawable
{
    private SpriteBatch _spriteBatch;
    private PlayerInventory _inventory;

    private Texture2D slotTexture;

    public InventoryRenderer(GraphicsDevice graphicsDevice, PlayerInventory inventory)
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _inventory = inventory;
    }

    public void LoadContent()
    {
        slotTexture = Main.ContentManager.Load<Texture2D>("Images/UI/slot");
    }

    public void Draw(DrawContext drawContext)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        for(var i = 0; i < 10; i++)
        {
            _spriteBatch.Draw(slotTexture, new Vector2(2 + i * 20, 2), Color.White * (0.5f + (Main.Player.HeldItemSlot == i ? 0.2f : 0)));
        }
        for(var i = 0; i < 10; i++)
        {
            var item = _inventory[i];
            if(item is null) continue;

            var def = item.GetRegistryEntry();

            var tex = Main.ContentManager.Load<Texture2D>(def.GetTexturePath());
            if(tex is null) continue;

            Rectangle? rect = null;
            var size = 16f / MathHelper.Max(16, tex.Width);
            var pivot = new Vector2(tex.Width / 2f, tex.Height / 2f);

            _spriteBatch.Draw(tex, new Vector2(11 + i * 20, 11), rect, Color.White, 0, pivot, size, SpriteEffects.None, 0);
        }

        _spriteBatch.End();
    }
}
