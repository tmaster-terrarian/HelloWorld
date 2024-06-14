using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelloWorld.Graphics;

public class PlayerRenderer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly List<Texture2D> _textures = new();
    private Texture2D empty;

    public PlayerRenderer(GraphicsDevice graphicsDevice)
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);
    }

    public void LoadContent()
    {
        // head
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_0"));  // head
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_1"));  // eye whites
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_2"));  // eye pupils
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_15")); // eyelids

        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/Hair_148")); // hair

        // legs
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_10")); // legs
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_11")); // pants
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_12")); // shoes

        // body/arms
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_3"));  // chest
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_16")); // arms
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_8a")); // clothing layer 1
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_8b")); // clothing layer 2
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_9a")); // clothing sleeve 1
        _textures.Add(Main.GetAsset<Texture2D>("Images/Player/0_9b")); // clothing sleeve 2

        empty = Main.GetAsset<Texture2D>("Images/Player/empty");
    }

    enum TextureIndex
    {
        Head,
        EyeWhites,
        EyePupils,
        Eyelids,
        Hair,
        Legs,
        Pants,
        Shoes,
        Chest,
        Arms,
        Clothes1,
        Clothes2,
        Sleeve1,
        Sleeve2
    }

    public void DrawPlayer(Player player)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        DrawPlayer(player, player.position, player.angle, new Vector2(26 / 2, 36 / 2 - 2));

        _spriteBatch.End();
    }

    private void DrawPlayer(Player player, Vector2 position, float rotation, Vector2 pivotPoint)
    {
        Rectangle bodyFrame = new(0, 0, 26, 36);
        Rectangle legFrame = new(0, 0, 26, 36);
        Rectangle armFrame = new(0, 0, 26, 36);
        Rectangle eyelidFrame = new(26, 0, 26, 36);

        position += new Vector2(6, 4);

        bodyFrame.Location = new((int)player.bodyFrame % 10 * 26, ((int)(player.bodyFrame / 10f) + ((player.female ? 1 : 0) * 2)) * 36);
        legFrame.Location = new((int)player.legFrame % 10 * 26, ((int)(player.legFrame / 10f) + ((player.female ? 1 : 0) * 2)) * 36);
        armFrame.Location = new((int)player.armFrame % 10 * 26, (int)(player.armFrame / 10f) * 36);

        eyelidFrame.Y = (int)player.bodyFrame switch {
            7 or 8 or 9 or
            14 or 15 or 16 => 1,
            _ => 0,
        };

        var spriteEffects = SpriteEffects.FlipHorizontally & (SpriteEffects)Math.Max(0, -player.Facing);

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

        for(int i = 0; i < 3; i++)
        {
            _spriteBatch.Draw(
                _textures[i] ?? empty,
                Vector2.Round(position),
                bodyFrame,
                Color.White,
                rotation,
                pivotPoint,
                1,
                spriteEffects,
                0
            );
        }

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Eyelids] ?? empty,
            Vector2.Round(position),
            eyelidFrame,
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Hair] ?? empty,
            Vector2.Round(position),
            bodyFrame,
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        for(int i = 5; i < 8; i++)
        {
            _spriteBatch.Draw(
                _textures[i] ?? empty,
                Vector2.Round(position),
                legFrame,
                Color.White,
                rotation,
                pivotPoint,
                1,
                spriteEffects,
                0
            );
        }

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Arms] ?? empty,
            Vector2.Round(position),
            new(armFrame.Location + new Point(0, 36 * 2), armFrame.Size),
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Sleeve1] ?? empty,
            Vector2.Round(position),
            new(armFrame.Location + new Point(0, 36 * 2), armFrame.Size),
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Sleeve2] ?? empty,
            Vector2.Round(position),
            new(armFrame.Location + new Point(0, 36 * 2), armFrame.Size),
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Chest] ?? empty,
            Vector2.Round(position),
            bodyFrame,
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Clothes1] ?? empty,
            Vector2.Round(position),
            armFrame,
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Clothes2] ?? empty,
            Vector2.Round(position),
            armFrame,
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Arms] ?? empty,
            Vector2.Round(position),
            armFrame,
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Sleeve1] ?? empty,
            Vector2.Round(position),
            armFrame,
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Sleeve2] ?? empty,
            Vector2.Round(position),
            armFrame,
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
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
