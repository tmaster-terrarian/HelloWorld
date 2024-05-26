using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HelloWorld.Core;

public class Level : IDisposable, IDrawable
{
    protected Tile[,] _tiles;
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
                    var tile = _tiles[x, y];
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

    public int DrawOrder => 0;

    public bool Visible => true;

    public bool drawCollisionsOnDebug = false;

    public Tile TilePlace(Rectangle rect)
    {
        if(Main.DebugModeEnabled && drawCollisionsOnDebug && !_collisionChecks.Contains(rect)) _collisionChecks.Add(rect);

        Rectangle[,] cols = Collisions;
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var r = cols[x, y];

                if(rect.Intersects(r)) return _tiles[x, y];
            }
        }
        return null;
    }

    public bool TileMeeting(Rectangle rect) => TilePlace(rect) != null;

    private Dictionary<string, Texture2D> _textureCache;
    private readonly List<Rectangle> _collisionChecks;

    private SpriteBatch _spriteBatch;

    public event EventHandler<EventArgs> DrawOrderChanged;
    public event EventHandler<EventArgs> VisibleChanged;

    public Level(GraphicsDevice graphicsDevice)
    {
        _spriteBatch = new(graphicsDevice);
        _collisionChecks = new();

        width = 40;
        height = 23;
        _tiles = new Tile[width, height];

        _textureCache = new Dictionary<string, Texture2D>
        {
            { "air", null }
        };

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile();
            }
        }

        _collisions = new Rectangle[width, height];
        // RefreshTileShapes(Bounds);
    }

    public void RefreshTileShapes(Rectangle area)
    {
        var _area = ValidateArea(area);

        for(int x = _area.X; x < _area.X + _area.Width; x++)
        {
            for(int y = _area.Y; y < _area.Y + _area.Height; y++)
            {
                Tile tile = _tiles[x, y];

                Rectangle rect = new Rectangle(-10000, -10000, 1, 1);
                if(tile.id != "air")
                    rect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                _collisions[x, y] = rect;

                if(tile.id == "air") continue;

                byte matches = 0b0000;

                if((y > 0 && _tiles[x, y - 1].id == tile.id) || y <= 0)
                    matches |= 0b0001;

                if((x < width - 1 && _tiles[x + 1, y].id == tile.id) || x >= width - 1)
                    matches |= 0b0010;

                if((y < height - 1 && _tiles[x, y + 1].id == tile.id) || y >= height - 1)
                    matches |= 0b0100;

                if((x > 0 && _tiles[x - 1, y].id == tile.id) || x <= 0)
                    matches |= 0b1000;

                tile.shape = matches;
            }
        }
    }

    public Rectangle ValidateArea(Rectangle rectangle)
    {
        var area = new Rectangle(rectangle.Location, rectangle.Size);

        return new(
            Math.Clamp(rectangle.X, 0, width - 1),
            Math.Clamp(rectangle.Y, 0, height - 1),
            Math.Clamp(rectangle.X + rectangle.Width, rectangle.X + 1, width) - rectangle.X,
            Math.Clamp(rectangle.Y + rectangle.Height, rectangle.Y + 1, height) - rectangle.Y
        );
    }

    public void LoadContent()
    {
        // for(int x = 0; x < width; x++)
        // {
        //     for(int y = 0; y < height; y++)
        //     {
        //         Tile tile = _tiles[x, y];
        //         if(!_textureCache.ContainsKey(tile.id))
        //         {
        //             _textureCache.Add(tile.id, Main.ContentManager.Load<Texture2D>("Images/Tiles/" + tile.id));
        //         }
        //     }
        // }
    }

    public void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Tile tile = _tiles[x, y];
                if(tile.id == "air") continue;

                Texture2D texture = Main.ContentManager.Load<Texture2D>("Images/Tiles/" + tile.id);
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

        if(Main.DebugModeEnabled && drawCollisionsOnDebug)
        {
            foreach(var rect in _collisionChecks)
            {
                _spriteBatch.Draw(
                    Main.OnePixel,
                    rect,
                    Color.Red * 0.25f
                );
            }
        }

        if(_collisionChecks.Count > 0) _collisionChecks.Clear();

        _spriteBatch.End();
    }

    public void Dispose()
    {
        _textureCache.Clear();
        _collisionChecks.Clear();
    }

    public string GetTileIdAtPosition(Vector2 position) => GetTileIdAtTilePosition((position / tileSize).ToPoint());

    public string GetTileIdAtTilePosition(Point position)
    {
        int x = position.X;
        int y = position.Y;

        if(!InWorld(x, y)) return "air";

        return _tiles[x, y].id;
    }

    public Tile GetTileAtPosition(Vector2 position) => GetTileAtTilePosition((position / tileSize).ToPoint());

    public Tile GetTileAtTilePosition(Point position)
    {
        int x = position.X;
        int y = position.Y;

        if(!InWorld(x, y)) return null;

        return _tiles[x, y];
    }

    public void SetTile(string id, Point position)
    {
        if(!InWorld(position.X, position.Y)) return;

        _tiles[position.X, position.Y] = new Tile(id);

        RefreshTileShapes(new Rectangle(new Point(position.X - 1, position.Y - 1), new Point(3, 3)));
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
