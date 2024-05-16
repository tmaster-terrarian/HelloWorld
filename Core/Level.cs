using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GAY.Core;

public class Level : IDisposable
{
    public ContentManager Content { get; protected set; }

    public Tile[,] tiles;
    public int width;
    public int height;

    public Rectangle Bounds { 
        get {
            return new Rectangle(0, 0, width, height);
        }
    }

    private Dictionary<string, Texture2D> _textureCache;
    private readonly int tileSize = 8;

    public Level(IServiceProvider serviceProvider)
    {
        width = 10;
        height = 10;
        tiles = new Tile[width, height];

        Content = new ContentManager(serviceProvider, "Content");

        _textureCache = new Dictionary<string, Texture2D>
        {
            { "air", null }
        };

        for(int x = 0; x < tiles.GetLength(0); x++)
        {
            for(int y = 0; y < tiles.GetLength(1); y++)
            {
                tiles[x, y] = new Tile();
            }
        }

        tiles[0, 0] = new Tile("stone");
        tiles[1, 0] = new Tile("stone");
        tiles[2, 0] = new Tile("stone");
        tiles[0, 1] = new Tile("stone");

        RefreshTileShapes(Bounds);
    }

    public void RefreshTileShapes(Rectangle area)
    {
        var _area = ValidateArea(area);

        for(int x = _area.X; x < _area.Width; x++)
        {
            for(int y = _area.Y; y < _area.Height; y++)
            {
                Tile tile = tiles[x, y];
                if(tile.id == "air") continue;

                byte matches = 0b0000;

                if((y > 0 && tiles[x, y - 1].id == tile.id) || y <= 0)
                    matches |= 0b0001;

                if((x < width - 1 && tiles[x + 1, y].id == tile.id) || x >= width - 1)
                    matches |= 0b0010;

                if((y < height - 1 && tiles[x, y + 1].id == tile.id) || y >= height - 1)
                    matches |= 0b0100;

                if((x > 0 && tiles[x - 1, y].id == tile.id) || x <= 0)
                    matches |= 0b1000;

                tile.shape = matches;
            }
        }
    }

    public Rectangle ValidateArea(Rectangle rectangle)
    {
        var area = new Rectangle(rectangle.Location, rectangle.Size);

        if(area == Rectangle.Empty) area = new Rectangle(0, 0, 1, 1);

        area.X = Math.Clamp(area.X, 0, width - 1);
        area.Y = Math.Clamp(area.Y, 0, width - 1);

        if(area.X + area.Width > width) area.Width -= area.X + area.Width - width;
        if(area.Y + area.Height > height) area.Height -= area.Y + area.Height - height;

        return area;
    }

    public void LoadContent()
    {
        for(int x = 0; x < tiles.GetLength(0); x++)
        {
            for(int y = 0; y < tiles.GetLength(1); y++)
            {
                Tile tile = tiles[x, y];
                if(!_textureCache.ContainsKey(tile.id))
                {
                    _textureCache.Add(tile.id, Content.Load<Texture2D>("Images/Tiles/" + tile.id));
                }
            }
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        for(int x = 0; x < tiles.GetLength(0); x++)
        {
            for(int y = 0; y < tiles.GetLength(1); y++)
            {
                Tile tile = tiles[x, y];
                if(tile.id == "air") continue;

                Texture2D texture;
                _textureCache.TryGetValue(tile.id, out texture);

                if(texture == null) continue;

                Rectangle UV = Tile.GetShapeUV(tile.shape);
                UV.X *= tileSize;
                UV.Y *= tileSize;
                UV.Width *= tileSize;
                UV.Height *= tileSize;

                spriteBatch.Draw(
                    texture,
                    new Vector2(x * tileSize, y * tileSize),
                    UV,
                    Color.White,
                    0,
                    Vector2.Zero,
                    new Vector2(1, 1),
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }

    public void Dispose()
    {
        _textureCache.Clear();
        Content.Unload();
    }
}

public class Tile
{
    public readonly string id;
    public byte shape;

    // 0000 : LBRT

    private static readonly Rectangle[] UVshapes = {
        new Rectangle(0, 0, 1, 1),   // 00 0000
        new Rectangle(0, 3, 1, 1),   // 01 0001
        new Rectangle(1, 0, 1, 1),   // 02 0010
        new Rectangle(1, 3, 1, 1),   // 03 0011
        new Rectangle(0, 1, 1, 1),   // 04 0100
        new Rectangle(0, 2, 1, 1),   // 05 0101
        new Rectangle(1, 1, 1, 1),   // 06 0110
        new Rectangle(1, 2, 1, 1),   // 07 0111
        new Rectangle(3, 0, 1, 1),   // 08 1000
        new Rectangle(3, 3, 1, 1),   // 09 1001
        new Rectangle(2, 0, 1, 1),   // 10 1010
        new Rectangle(2, 3, 1, 1),   // 11 1011
        new Rectangle(3, 1, 1, 1),   // 12 1100
        new Rectangle(3, 2, 1, 1),   // 13 1101
        new Rectangle(2, 1, 1, 1),   // 14 1110
        new Rectangle(2, 2, 1, 1),   // 15 1111
    };

    public Tile()
    {
        id = "air";
        shape = 0;
    }

    public Tile(string id, byte shape = 0)
    {
        this.id = id;
        this.shape = shape;
    }

    public override string ToString()
    {
        return $"Tile({id})";
    }

    public static Rectangle GetShapeUV(byte shape)
    {
        if(shape < 16 && shape >= 0) return UVshapes[shape];
        else return UVshapes[0];
    }
}
