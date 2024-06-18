using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelloWorld.Graphics;

public class PlayerRenderer
{
    private readonly List<Texture2D> _textures = new();
    private Texture2D empty;

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

    public void DrawPlayer(Player player, SpriteBatch _spriteBatch)
    {
        DrawPlayer(player, _spriteBatch, player.position.ToVector2(), player.angle, new Vector2(26 / 2, 36 / 2 - 2));
    }

    private void DrawPlayer(Player player, SpriteBatch _spriteBatch, Vector2 position, float rotation, Vector2 pivotPoint)
    {
        Rectangle bodyFrame = new(0, 0, 26, 36);
        Rectangle legFrame = new(0, 0, 26, 36);
        Rectangle armFrame = new(0, 0, 26, 36);
        Rectangle eyelidFrame = new(0, 0, 26, 36);

        // 0xf3dcbc

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

        // _spriteBatch.Draw(
        //     Main.OnePixel,
        //     new Rectangle(player.Hitbox.X, player.Hitbox.Y, player.Hitbox.Width, player.Hitbox.Height),
        //     null,
        //     Color.Yellow * 0.5f,
        //     0f,
        //     Vector2.Zero,
        //     SpriteEffects.None,
        //     0
        // );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Arms] ?? empty,
            position,
            new(armFrame.Location + new Point(0, 36 * 2), armFrame.Size),
            player.styleColors.Skin,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Sleeve1] ?? empty,
            position,
            new(armFrame.Location + new Point(0, 36 * 2), armFrame.Size),
            player.styleColors.Clothes1,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Sleeve2] ?? empty,
            position,
            new(armFrame.Location + new Point(0, 36 * 2), armFrame.Size),
            player.styleColors.Clothes2,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Head] ?? empty,
            position,
            bodyFrame,
            player.styleColors.Skin,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.EyeWhites] ?? empty,
            position,
            eyelidFrame,
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.EyePupils] ?? empty,
            position,
            eyelidFrame,
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Eyelids] ?? empty,
            position,
            eyelidFrame,
            player.styleColors.Skin,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Hair] ?? empty,
            position,
            bodyFrame,
            Color.White,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Legs] ?? empty,
            position,
            legFrame,
            player.styleColors.Skin,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Pants] ?? empty,
            position,
            legFrame,
            player.styleColors.Pants,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Shoes] ?? empty,
            position,
            legFrame,
            player.styleColors.Shoes,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Chest] ?? empty,
            position,
            bodyFrame,
            player.styleColors.Skin,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Clothes1] ?? empty,
            position,
            bodyFrame,
            player.styleColors.Clothes1,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Clothes2] ?? empty,
            position,
            bodyFrame,
            player.styleColors.Clothes2,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        if(player.Swinging && player.HeldItem is not null)
        {
            var def = player.HeldItem.GetDef();
            var tex = Main.GetAsset<Texture2D>(def.GetTexturePath());

            if(tex is not null)
            {
                _spriteBatch.Draw(
                    tex,
                    position + new Vector2(player.Facing * -3, 3),
                    null,
                    Color.White,
                    MathHelper.ToRadians((((player.armFrame - 2) * 160 / 4) - 80) * player.Facing),
                    new Vector2(player.Facing == 1 ? -4 : tex.Width + 4, tex.Height + 4),
                    1,
                    spriteEffects,
                    0
                );
            }
        }

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Arms] ?? empty,
            position,
            armFrame,
            player.styleColors.Skin,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Sleeve1] ?? empty,
            position,
            armFrame,
            player.styleColors.Clothes1,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        _spriteBatch.Draw(
            _textures[(int)TextureIndex.Sleeve2] ?? empty,
            position,
            armFrame,
            player.styleColors.Clothes2,
            rotation,
            pivotPoint,
            1,
            spriteEffects,
            0
        );

        // _spriteBatch.Draw(
        //     Main.OnePixel,
        //     new Rectangle((player.Center - Vector2.One).ToPoint(), (Vector2.One * 2).ToPoint()),
        //     null,
        //     Color.Red,
        //     0f,
        //     Vector2.Zero,
        //     SpriteEffects.None,
        //     0
        // );
    }
}
