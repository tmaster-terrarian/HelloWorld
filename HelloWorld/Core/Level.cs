using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using HelloWorld.Graphics;

namespace HelloWorld.Core;

public class Level : IDisposable
{
    public ContentManager Content { get; protected set; }

    public Tile[,] Tiles { get; protected set; }
    public int width;
    public int height;

    public Rectangle Bounds {
        get {
            return new Rectangle(0, 0, width, height);
        }
    }
    public readonly int tileSize = 8;

    private Rectangle[,] _collisions = null;

    public Rectangle[,] Collisions {
        get {
            if(_collisions != null) return _collisions;

            Rectangle[,] rectangles = new Rectangle[width, height];

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    var tile = Tiles[x, y];
                    Rectangle rect = new Rectangle(-10000, -10000, 1, 1);

                    if(tile.id != "air")
                        rect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

                    rectangles[x, y] = rect;
                }
            }

            _collisions = rectangles;
            return rectangles;
        }
    }

    public bool TileMeeting(Rectangle rect)
    {
        Rectangle[,] cols = Collisions;
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var r = cols[x, y];

                if(rect.Intersects(r)) return true;
            }
        }
        return false;
    }

    public Tile TilePlace(Rectangle rect)
    {
        Rectangle[,] cols = Collisions;
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var r = cols[x, y];

                if(rect.Intersects(r)) return Tiles[x, y];
            }
        }
        return null;
    }

    private Dictionary<string, Texture2D> _textureCache;

    private SpriteBatch _spriteBatch;

    public Level(GraphicsDevice graphicsDevice)
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);

        width = 40;
        height = 23;
        Tiles = new Tile[width, height];

        Content = new ContentManager(Main.ContentManager.ServiceProvider, "Content");

        _textureCache = new Dictionary<string, Texture2D>
        {
            { "air", null }
        };

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Tiles[x, y] = new Tile();
            }
        }

        Tiles[0, 0] = new Tile("stone");
        Tiles[1, 0] = new Tile("stone");
        Tiles[2, 0] = new Tile("stone");
        Tiles[0, 1] = new Tile("stone");

        for(var x = 0; x < width; x++)
        {
            for(int y = 1; y < 3; y++)
            {
                int Y = Tiles.GetLength(1) - y;
                Tiles[x, Y] = new Tile("stone");
            }
        }

        Tiles[16, 16] = new Tile("stone");

        _collisions = new Rectangle[width, height];
        RefreshTileShapes(Bounds);
    }

    public void RefreshTileShapes(Rectangle area)
    {
        var _area = ValidateArea(area);

        for(int x = _area.X; x < _area.Width; x++)
        {
            for(int y = _area.Y; y < _area.Height; y++)
            {
                Tile tile = Tiles[x, y];

                Rectangle rect = new Rectangle(-10000, -10000, 1, 1);
                if(tile.id != "air")
                    rect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                _collisions[x, y] = rect;

                if(tile.id == "air") continue;

                byte matches = 0b0000;

                if((y > 0 && Tiles[x, y - 1].id == tile.id) || y <= 0)
                    matches |= 0b0001;

                if((x < width - 1 && Tiles[x + 1, y].id == tile.id) || x >= width - 1)
                    matches |= 0b0010;

                if((y < height - 1 && Tiles[x, y + 1].id == tile.id) || y >= height - 1)
                    matches |= 0b0100;

                if((x > 0 && Tiles[x - 1, y].id == tile.id) || x <= 0)
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
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Tile tile = Tiles[x, y];
                if(!_textureCache.ContainsKey(tile.id))
                {
                    _textureCache.Add(tile.id, Content.Load<Texture2D>("Images/Tiles/" + tile.id));
                }
            }
        }
    }

    public void Draw(DrawContext context)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Tile tile = Tiles[x, y];
                if(tile.id == "air") continue;

                Texture2D texture;
                _textureCache.TryGetValue(tile.id, out texture);

                if(texture == null) continue;

                Rectangle UV = Tile.GetShapeUV(tile.shape);
                UV.X *= tileSize;
                UV.Y *= tileSize;
                UV.Width *= tileSize;
                UV.Height *= tileSize;

                _spriteBatch.Draw(
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

        _spriteBatch.End();
    }

    public void Dispose()
    {
        _textureCache.Clear();
        Content.Unload();
    }

    public string GetTileIdAtPosition(Vector2 position)
    {
        int x = (int)(position.X / tileSize);
        int y = (int)(position.Y / tileSize);

        if(!InWorld(x, y)) return "air";

        return Tiles[x, y].id;
    }

    public string GetTileIdAtTilePosition(Point position)
    {
        int x = position.X;
        int y = position.Y;

        if(!InWorld(x, y)) return "air";

        return Tiles[x, y].id;
    }

    public Tile GetTileAtPosition(Vector2 position)
    {
        int x = (int)(position.X / tileSize);
        int y = (int)(position.Y / tileSize);

        if(!InWorld(x, y)) return null;

        return Tiles[x, y];
    }

    public Tile GetTileAtTilePosition(Point position)
    {
        int x = position.X;
        int y = position.Y;

        if(!InWorld(x, y)) return null;

        return Tiles[x, y];
    }

    public static bool InWorld(Level level, int x, int y)
    {
        return x >= 0 && x < level.width && y >= 0 && y < level.height;
    }

    public bool InWorld(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
