using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelloWorld.Graphics;

public class PlayerRenderer
{
    private Texture2D texture;
    private readonly SpriteBatch _spriteBatch;

    public PlayerRenderer(GraphicsDevice graphicsDevice)
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);
    }

    public void LoadContent()
    {
        texture = Main.GetAsset<Texture2D>("Images/Characters/player");
    }

    public void DrawPlayer(Player player)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        DrawPlayer(player, player.position, player.angle, new Vector2(8, 8));

        _spriteBatch.End();
    }

    private void DrawPlayer(Player player, Vector2 position, float rotation, Vector2 pivotPoint)
    {
        _spriteBatch.Draw(
            Main.OnePixel,
            new Rectangle(player.Hitbox.X, player.Hitbox.Y, player.Hitbox.Width, player.Hitbox.Height),
            null,
            Color.Yellow * 0.5f,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            0
        );

        _spriteBatch.Draw(
            texture,
            Vector2.Round(position) + new Vector2(4, 6),
            null,
            Color.White,
            rotation,
            pivotPoint,
            1,
            SpriteEffects.FlipHorizontally & (SpriteEffects)Math.Max(0, -player.facing),
            0.5f
        );

        _spriteBatch.Draw(
            Main.OnePixel,
            new Rectangle((player.Center - Vector2.One).ToPoint(), (Vector2.One * 2).ToPoint()),
            null,
            Color.Red,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            0
        );
    }
}
