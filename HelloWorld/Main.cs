using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using HelloWorld.Core;
using HelloWorld.Events;
using HelloWorld.Graphics;
using HelloWorld.Registries;
using System.Collections.Generic;

namespace HelloWorld;

public class Main : Game
{
    public static int GamepadDeadZone { get; private set; }

    public static bool GamepadEnabled { get; private set; }

    public static Point ScreenSize {
        get {
            return new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }
        set {
            _graphics.PreferredBackBufferWidth = value.X;
            _graphics.PreferredBackBufferHeight = value.Y;
        }
    }

    static ContentManager ContentManager { get; set; }

    public static Point NativeScreenSize { get; private set; } = new(640, 360);

    public static Level Level { get; private set; }
    public static Texture2D OnePixel { get; private set; }

    public static Player Player;

    RenderTarget2D renderTarget;

    Matrix viewMatrix = Matrix.Identity;
    static Point cameraPos = Point.Zero;

    public static SpriteFont Font { get; private set; }

    Vector2 inputVector = Vector2.Zero;

    private static GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static Point MousePos => new Point(Mouse.GetState().X / (_graphics.PreferredBackBufferWidth / NativeScreenSize.X), Mouse.GetState().Y / (_graphics.PreferredBackBufferHeight / NativeScreenSize.Y));

    public static Point MouseWorldPos => MousePos + cameraPos;

    public static Point CameraPosition => cameraPos;

    private PlayerRenderer clientPlayerRenderer;
    private CursorRenderer cursorRenderer;
    private InventoryRenderer inventoryRenderer;

    private readonly Registries.Tile.TileRegistry TileRegistry = new();
    private readonly Registries.Sound.SoundRegistry SoundRegistry = new();
    private readonly Registries.Item.ItemRegistry ItemRegistry = new();

    private static KeyboardState _keyboardState;
    private static GamePadState _gamePadState;
    private static JoystickState _joystickState;
    private static MouseState _mouseState;

    public static KeyboardState KeyboardState => _keyboardState;
    public static GamePadState GamePadState => _gamePadState;
    public static JoystickState JoystickState => _joystickState;
    public static MouseState MouseState => _mouseState;

    private static bool _debugMode = false;
    public static bool DebugModeEnabled => _debugMode;

    public static Random WorldGenRandom { get; private set; } = new(0);

    public static class GlobalEvents
    {
        public delegate void GlobalEventHandler<TEventArgs>(TEventArgs e);

        public static event GlobalEventHandler<TileEvent> onTilePlaced;
        public static event GlobalEventHandler<TileEvent> onTileUpdated;
        public static event GlobalEventHandler<TileEvent> onTileDestroyed;

        public static void DoTilePlace(TileEvent e) => onTilePlaced?.Invoke(e);
        public static void DoTileUpdate(TileEvent e) => onTileUpdated?.Invoke(e);
        public static void DoTileDestroy(TileEvent e) => onTileDestroyed?.Invoke(e);
    }

    public Main()
    {
        _graphics = new GraphicsDeviceManager(this);

        IsMouseVisible = true;
        IsFixedTimeStep = true;
        GamepadEnabled = false;

        Content.RootDirectory = "Content";
        ContentManager = Content;

        _debugMode = true;
    }

    protected override void Initialize()
    {
        Window.Title = "MONOGAYME";

        int screenScale = GraphicsDevice.Adapter.CurrentDisplayMode.Height / 360;

        ScreenSize = new Point(NativeScreenSize.X * screenScale, NativeScreenSize.Y * screenScale);
        if(ScreenSize.Y == GraphicsDevice.Adapter.CurrentDisplayMode.Height)
        {
            Window.Position = Point.Zero;
            Window.IsBorderless = true;
        }

        _graphics.ApplyChanges();

        SoundEffect.SpeedOfSound = 1000000f;
        SoundEffect.MasterVolume = 0.5f;

        Input.Init();

        TileRegistry.Register();
        ItemRegistry.Register();

        Level = new Level();

        for(var x = 0; x < Level.width; x++)
        {
            for(int y = 1; y < 30; y++)
            {
                int Y = Level.height - y;

                if(WorldGenRandom.NextSingle() < 0.99f)
                {
                    if(y <= 24)
                        Level.SetTile("stone", new(x, Y));
                    else
                        Level.SetTile("dirt", new(x, Y));
                }
                else
                {
                    var nx = x;
                    var ny = Y;

                    for(int i = 0; i < 20; i++)
                    {
                        if(WorldGenRandom.NextSingle() < 0.05f) break;

                        for(int j = 0; j < 100; j++)
                        {
                            if(Level.GetTileIdAtTilePosition(new(nx, ny)) != "air") break;
                            var _x = WorldGenRandom.NextBool();
                            nx += _x ? (WorldGenRandom.NextBool() ? -1 : 1) : 0;
                            ny += !_x ? (WorldGenRandom.NextBool() ? -1 : 1) : 0;
                            if(nx < 0) nx++;
                            if(nx > Level.width - 1) nx--;
                            if(ny < 0) ny++;
                            if(ny > Level.height - 1) ny--;
                        }

                        Level.SetTile("air", new(nx, ny));
                    }
                }
            }
        }

        Level.SetTile("dirt", new(16, 16));

        Level.SetTile("brick", new(13, Level.height - 40));
        Level.GetTileAtTilePosition(new(13, Level.height - 40)).half = true;

        Level.SetTile("brick", new(12, Level.height - 40));

        Player = new Player()
        {
            position = new(320 / 2, 180 / 2)
        };

        GamepadDeadZone = 4096;

        this.renderTarget = new RenderTarget2D(
            GraphicsDevice,
            NativeScreenSize.X,
            NativeScreenSize.Y,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24
        );

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        clientPlayerRenderer = new PlayerRenderer();
        cursorRenderer = new CursorRenderer();
        inventoryRenderer = new InventoryRenderer(Player.Inventory);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        OnePixel = Content.Load<Texture2D>("Images/Other/onepixel");

        SoundRegistry.Register();

        Level.LoadContent();

        clientPlayerRenderer.LoadContent();
        Player.LoadContent();

        cursorRenderer.LoadContent();

        inventoryRenderer.LoadContent();

        Font = SpriteFontBuilder.BuildDefaultFont(Content.Load<Texture2D>("Fonts/default"));
    }

    public static event EventHandler LostFocus;
    public static event EventHandler RegainedFocus;

    private bool wasActive = true;

    protected override void Update(GameTime gameTime)
    {
        _keyboardState = Input.GetKeyboardState();
        _gamePadState  = Input.GetGamePadState();
        _joystickState = Input.GetJoystickState();
        _mouseState = Input.GetMouseState();

        if(IsActive != wasActive)
        {
            if(wasActive)
            {
                LostFocus?.Invoke(this, new());
            }
            else
            {
                RegainedFocus?.Invoke(this, new());
            }
        }

        if(GamePadState.IsButtonDown(Buttons.Back) || KeyboardState.IsKeyDown(Keys.Escape))
            Exit();

        if(Joystick.LastConnectedIndex == 0 && GamepadEnabled)
        {
            inputVector = new Vector2(
                Math.Abs(JoystickState.Axes[0]) >= GamepadDeadZone ? Math.Sign(JoystickState.Axes[0]) : 0,
                Math.Abs(JoystickState.Axes[1]) >= GamepadDeadZone ? Math.Sign(JoystickState.Axes[1]) : 0
            );
        }
        else
        {
            inputVector = new Vector2(
                (KeyboardState.IsKeyDown(Keys.D) ? 1f : 0f) - (KeyboardState.IsKeyDown(Keys.A) ? 1f : 0f),
                (KeyboardState.IsKeyDown(Keys.S) ? 1f : 0f) - (KeyboardState.IsKeyDown(Keys.W) ? 1f : 0f)
            );
        }

        if(inputVector != Vector2.Zero) inputVector.Normalize();

        Player.Update();

        cameraPos.X = MathHelper.Clamp(Player.Center.X - (NativeScreenSize.X / 2), 0, (Level.Bounds.Width * Level.tileSize) - NativeScreenSize.X);
        cameraPos.Y = MathHelper.Clamp(Player.Center.Y - (NativeScreenSize.Y / 2), 0, (Level.Bounds.Height * Level.tileSize) - NativeScreenSize.Y);

        viewMatrix = Matrix.CreateTranslation(new Vector3(-cameraPos.ToVector2(), 0));

        Item.UpdateAll();

        var mouseTile = Level.GetTileAtPosition(MouseWorldPos.ToVector2());
        var mouseSnappedPos = MathUtil.Snap(MouseWorldPos.ToVector2(), Level.tileSize);
        if(mouseTile != null && Player.IsHoldingTileRelatedItem())
        {
            if(mouseTile.id == "air" && Player.IsHoldingPlaceable())
            {
                if(Level.TileMeeting(new((mouseSnappedPos - Vector2.UnitX * 5).ToPoint(), new(Level.tileSize + 10, Level.tileSize)))
                || Level.TileMeeting(new((mouseSnappedPos - Vector2.UnitY * 5).ToPoint(), new(Level.tileSize, Level.tileSize + 10))))
                {
                    cursorRenderer.CursorPos = mouseSnappedPos;
                    cursorRenderer.State = CursorRenderer.DrawState.Visible;

                    cursorRenderer.positionInvalid = !Player.TileWithinReach(MouseWorldPos.ToVector2());
                }
                else cursorRenderer.State = CursorRenderer.DrawState.Hidden;
            }
            else if(mouseTile.id != "air")
            {
                cursorRenderer.CursorPos = mouseSnappedPos;
                cursorRenderer.State = CursorRenderer.DrawState.Visible;

                cursorRenderer.positionInvalid = !Player.TileWithinReach(MouseWorldPos.ToVector2());
            }
            else cursorRenderer.State = CursorRenderer.DrawState.Hidden;
        }
        else cursorRenderer.State = CursorRenderer.DrawState.Hidden;

        cursorRenderer.Update();

        wasActive = IsActive;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: viewMatrix);

        Level.Draw(_spriteBatch);

        clientPlayerRenderer.DrawPlayer(Player, _spriteBatch);

        Item.DrawAll(_spriteBatch);

        // cursorRenderer.Draw(_spriteBatch);

        _spriteBatch.End();

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        string fps = "fps:" + (int)MathHelper.Max(0f, (float)(1 / gameTime.ElapsedGameTime.TotalSeconds));

        _spriteBatch.DrawString(Font, fps, Vector2.Zero + Vector2.UnitY * 18 + Vector2.One, Color.Black);
        _spriteBatch.DrawString(Font, fps, Vector2.Zero + Vector2.UnitY * 18, Color.White);

        // _spriteBatch.DrawString(Font, MathF.Round(Player.velocity.X / Level.tileSize * 60).ToString(), Vector2.Zero + Vector2.UnitY * 30, Color.White);

        inventoryRenderer.Draw(_spriteBatch);

        _spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black * 0);

        int scale = (int)MathF.Ceiling((float)ScreenSize.Y / NativeScreenSize.Y);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateScale(scale));

        _spriteBatch.Draw(
            renderTarget,
            Vector2.Zero,
            null,
            Color.White,
            0,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            0
        );

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public static Player NearestPlayerTo(Point position)
    {
        // TODO: implement multiplayer
        return Player;
    }

    private static readonly List<string> missingAssets = new();

    public static T GetAsset<T>(string path)
    {
        if(missingAssets.Contains(path)) return default;

        try
        {
            return ContentManager.Load<T>(path);
        }
        catch(Exception e)
        {
            Console.Error.WriteLine(e.GetType().FullName + $": The content file \"{path}\" was not found.");
            missingAssets.Add(path);
            return default;
        }
    }
}
