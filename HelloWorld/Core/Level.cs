using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using HelloWorld.Events;

namespace HelloWorld.Core;

public class Level : IDisposable
{
    protected Tile[,] _tiles;
    public int width;
    public int height;

    private Texture2D breakingTexture;

    public Rectangle Bounds {
        get {
            return new Rectangle(0, 0, width, height);
        }
    }
    public const int tileSize = 8;

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

                    if(tile.half)
                    {
                        rect.Y += tileSize / 2;
                        rect.Height = tileSize / 2;
                    }

                    rectangles[x, y] = rect;
                }
            }

            _collisions = rectangles;
            return rectangles;
        }
    }

    public bool drawCollisionsOnDebug = false;

    public Tile TilePlace(Rectangle rect)
    {
        if(Main.DebugModeEnabled && drawCollisionsOnDebug && !_collisionChecks.Contains(rect)) _collisionChecks.Add(rect);

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var r = Collisions[x, y];

                if(rect.Intersects(r)) return _tiles[x, y];
            }
        }
        return null;
    }

    public bool TileMeeting(Rectangle rect) => TilePlace(rect) != null;

    private Dictionary<string, Texture2D> _textureCache;
    private readonly List<Rectangle> _collisionChecks;

    public Level()
    {
        _collisionChecks = new();

        width = 200;
        height = 100;
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

        Main.GlobalEvents.onTilePlaced += GlobalEvents_onTilePlaced;
        Main.GlobalEvents.onTileUpdated += GlobalEvents_onTileUpdated;
    }

    protected virtual void GlobalEvents_onTilePlaced(TileEvent e)
    {
        
    }

    protected virtual void GlobalEvents_onTileUpdated(TileEvent e)
    {
        
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

    public void RefreshTileShapes(Rectangle area)
    {
        area = ValidateArea(area);

        for(int x = area.X; x < area.X + area.Width; x++)
        {
            for(int y = area.Y; y < area.Y + area.Height; y++)
            {
                Tile tile = _tiles[x, y];

                Rectangle rect = new Rectangle(-10000, -10000, 1, 1);
                if(tile.id != "air")
                    rect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

                if(tile.half)
                {
                    rect.Y += tileSize / 2;
                    rect.Height = tileSize / 2;
                }

                _collisions[x, y] = rect;

                if(tile.id == "air") continue;

                RefreshTileShape(tile, x, y);
            }
        }
    }

    private void RefreshTileShape(Tile tile, int x, int y)
    {
        byte matches = 0b0000;

        if((y > 0 && _tiles[x, y - 1].id == tile.id) || y <= 0)
            matches |= 0b0001;

        if((x < width - 1 && _tiles[x + 1, y].id == tile.id) || x >= width - 1)
            matches |= 0b0010;

        if((y < height - 1 && _tiles[x, y + 1].id == tile.id) || y >= height - 1)
            matches |= 0b0100;

        if((x > 0 && _tiles[x - 1, y].id == tile.id) || x <= 0)
            matches |= 0b1000;

        if((y > 0 && _tiles[x, y - 1].id == "air") || (x < width - 1 && _tiles[x + 1, y].id == "air") || (y < height - 1 && _tiles[x, y + 1].id == "air") || (x > 0 && _tiles[x - 1, y].id == "air"))
        {
            tile.lightLevel = 5;
        }

        tile.shape = matches;
    }

    public void UpdateNeighbors(TileEvent e)
    {
        Point tilePosition = e.TilePos;

        for(int x = Math.Max(tilePosition.X - 1, 0); x <= Math.Min(tilePosition.X + 1, width - 1); x++)
        {
            for(int y = Math.Max(tilePosition.Y - 1, 0); y <= Math.Min(tilePosition.Y + 1, height - 1); y++)
            {
                if(x == tilePosition.X && y == tilePosition.Y) continue;

                var tile = _tiles[x, y];

                var def = tile.GetDef();

                if((x == tilePosition.X && y == 0) || (x == 0 && y == tilePosition.Y) || (x == tilePosition.X + 1 && y == tilePosition.Y) || (x == tilePosition.X && y == tilePosition.Y + 1))
                {
                    if(x == tilePosition.X && y == tilePosition.Y + 1 && _tiles[tilePosition.X, tilePosition.Y].id != "air" && tile.half)
                    {
                        tile.half = false;
                    }

                    def.OnUpdate(e);
                    Main.GlobalEvents.DoTileUpdate(e);
                }
            }
        }
    }

    public void LoadContent()
    {
        breakingTexture = Main.GetAsset<Texture2D>("Images/Other/breaking_progress");
    }

    public void Draw(SpriteBatch _spriteBatch)
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(x * tileSize + tileSize < Main.CameraPosition.X) continue;
                if(y * tileSize + tileSize < Main.CameraPosition.Y) continue;
                if(x * tileSize > Main.CameraPosition.X + Main.NativeScreenSize.X) continue;
                if(y * tileSize > Main.CameraPosition.Y + Main.NativeScreenSize.Y) continue;

                Tile tile = _tiles[x, y];
                if(tile.id == "air") continue;

                Texture2D texture = Main.GetAsset<Texture2D>("Images/Tiles/" + tile.id);
                if(texture == null) continue;

                Rectangle UV = Tile.GetShapeUV(tile.shape);
                UV.X *= tileSize;
                UV.Y *= tileSize;
                UV.Width *= tileSize;
                UV.Height *= tileSize;

                if(tile.half) UV.Height -= tileSize / 2;

                _spriteBatch.Draw(
                    texture,
                    new Vector2(x * tileSize, y * tileSize + (tile.half ? tileSize / 2 : 0)),
                    UV,
                    Color.White,
                    0,
                    Vector2.Zero,
                    new Vector2(1, 1),
                    SpriteEffects.None,
                    0f
                );

                if(tile.breakingProgress > 0)
                {
                    _spriteBatch.Draw(
                        breakingTexture,
                        new Vector2(x * tileSize, y * tileSize + (tile.half ? tileSize / 2 : 0)),
                        new Rectangle(8 * (int)MathHelper.Max(tile.breakingProgress * 3, 1), tile.half ? tileSize / 2 : 0, tileSize, tile.half ? tileSize / 2 : tileSize),
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

    public Tile? GetTileAtPosition(Vector2 position) => GetTileAtTilePosition((position / tileSize).ToPoint());

    public Tile? GetTileAtTilePosition(Point position)
    {
        int x = position.X;
        int y = position.Y;

        if(!InWorld(x, y)) return null;

        return _tiles[x, y];
    }

    public void SetTile(string id, Point position)
    {
        if(!InWorld(position.X, position.Y)) return;

        var tile = new Tile(id) { breakingProgress = 0 };
        _tiles[position.X, position.Y] = tile;

        RefreshTileShapes(new Rectangle(new Point(position.X - 1, position.Y - 1), new Point(3, 3)));
    }

    public static bool InWorld(Level level, int x, int y)
    {
        return x >= 0 && x < level.width && y >= 0 && y < level.height;
    }

    public static bool InWorld(Level level, Point pos)
    {
        return InWorld(level, pos.X, pos.Y);
    }

    public bool InWorld(int x, int y)
    {
        return InWorld(this, x, y);
    }

    public bool InWorld(Point pos)
    {
        return InWorld(pos.X, pos.Y);
    }
}
