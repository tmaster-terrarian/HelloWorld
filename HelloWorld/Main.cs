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

namespace HelloWorld;

public class Main : Game
{
    public static int GamepadDeadZone { get; private set; }

    public static bool GamepadEnabled { get; private set; }

    public Point screenSize {
        get {
            return new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }
        set {
            _graphics.PreferredBackBufferWidth = value.X;
            _graphics.PreferredBackBufferHeight = value.Y;
        }
    }

    public static Level Level { get; private set; }
    public static Texture2D OnePixel { get; private set; }
    public static ContentManager ContentManager { get; private set; }
    public static GameWindow MainWindow { get; private set; }

    public static Player Player;

    RenderTarget2D renderTarget;

    SpriteFont font;

    Vector2 inputVector = Vector2.Zero;

    private static GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static Vector2 MouseWorldPos
    {
        get {
            return new Vector2(Mouse.GetState(MainWindow).X / (_graphics.PreferredBackBufferWidth / 320), Mouse.GetState(MainWindow).Y / (_graphics.PreferredBackBufferHeight / 180));
        }
    }

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

    public static class GlobalEvents
    {
        public delegate void GlobalEventHandler<TEventArgs>(TEventArgs e);

        public static event GlobalEventHandler<TileEvent> onTilePlace;
        public static event GlobalEventHandler<TileEvent> onTileUpdated;

        public static void DoTilePlace(TileEvent e) => onTilePlace?.Invoke(e);
        public static void DoTileUpdate(TileEvent e) => onTileUpdated?.Invoke(e);
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

        screenSize = new Point(320 * 3, 180 * 3);
        _graphics.ApplyChanges();

        MainWindow = Window;

        SoundEffect.SpeedOfSound = 1000000f;
        SoundEffect.MasterVolume = 0.5f;

        TileRegistry.Register();
        ItemRegistry.Register();

        Level = new Level(GraphicsDevice);

        Player = new Player()
        {
            position = new Vector2(320 / 2, 180 / 2)
        };

        GamepadDeadZone = 4096;

        this.renderTarget = new RenderTarget2D(
            GraphicsDevice,
            320,
            180,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24
        );

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        clientPlayerRenderer = new PlayerRenderer(GraphicsDevice);
        cursorRenderer = new CursorRenderer(GraphicsDevice);
        inventoryRenderer = new InventoryRenderer(GraphicsDevice, Player.Inventory);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        OnePixel = Content.Load<Texture2D>("Images/Other/onepixel");

        SoundRegistry.Register();

        Level.LoadContent();

        Level.SetTile("stone", new(0, 0));
        Level.SetTile("stone", new(1, 0));
        Level.SetTile("stone", new(2, 0));
        Level.SetTile("stone", new(0, 1));

        for(var x = 0; x < Level.width; x++)
        {
            for(int y = 1; y < 3; y++)
            {
                int Y = Level.height - y;
                Level.SetTile("stone", new(x, Y));
            }
        }

        Level.SetTile("stone", new(16, 16));

        clientPlayerRenderer.LoadContent();
        Player.LoadContent();

        cursorRenderer.LoadContent();

        inventoryRenderer.LoadContent();

        font = SpriteFontBuilder.BuildDefaultFont(Content.Load<Texture2D>("Fonts/default"));
    }

    protected override void Update(GameTime gameTime)
    {
        _keyboardState = Input.GetKeyboardState();
        _gamePadState  = Input.GetGamePadState();
        _joystickState = Input.GetJoystickState();
        _mouseState = Input.GetMouseState(Window);

        if(GamePadState.IsButtonDown(Buttons.Back) || KeyboardState.IsKeyDown(Keys.Escape))
            Exit();

        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds * 60;

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

        Player.Update(delta);

        bool playerItemPlaceable = false;

        var mouseTile = Level.GetTileAtPosition(MouseWorldPos);
        var mouseTilePos = Vector2.Floor(MouseWorldPos / Level.tileSize) * Level.tileSize;
        if(mouseTile != null && Player.IsHoldingTileItem())
        {
            if(mouseTile.id == "air" && Player.IsHoldingPlaceable())
            {
                if(
                    Level.TileMeeting(new Rectangle((mouseTilePos - Vector2.UnitX).ToPoint(), new Point(10, 8))) ||
                    Level.TileMeeting(new Rectangle((mouseTilePos - Vector2.UnitY).ToPoint(), new Point(8, 10)))
                )
                {
                    playerItemPlaceable = true;

                    cursorRenderer.CursorPos = mouseTilePos;
                    cursorRenderer.State = CursorRenderer.DrawState.Visible;

                    cursorRenderer.positionInvalid = Vector2.Distance(
                        Player.Center - Vector2.UnitY * 3 * Level.tileSize,
                        mouseTilePos
                    ) > 6 * Level.tileSize;
                }
                else cursorRenderer.State = CursorRenderer.DrawState.Hidden;
            }
            else if(mouseTile.id != "air")
            {
                cursorRenderer.CursorPos = mouseTilePos;
                cursorRenderer.State = CursorRenderer.DrawState.Visible;

                cursorRenderer.positionInvalid = Vector2.Distance(
                    Player.Center - Vector2.UnitY * 3 * Level.tileSize,
                    mouseTilePos + Vector2.One * (Level.tileSize / 2f)
                ) > 6 * Level.tileSize;
            }
            else cursorRenderer.State = CursorRenderer.DrawState.Hidden;
        }
        else cursorRenderer.State = CursorRenderer.DrawState.Hidden;

        if(playerItemPlaceable && MouseState.LeftButton.HasFlag(ButtonState.Pressed))
        {
            var pos = (mouseTilePos / Level.tileSize).ToPoint();
            var def = Player.HeldItem.GetDef<Registries.Item.TileItemDef>();

            def.CreateTile(Level, pos);

            GlobalEvents.DoTilePlace(new TileEvent(pos, Player, Player.HeldItem.id));

            Level.RefreshTileShapes(Level.Bounds);
        }

        cursorRenderer.Update(delta);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        Level.Draw(gameTime);

        clientPlayerRenderer.DrawPlayer(Player);

        cursorRenderer.Draw(gameTime);

        inventoryRenderer.Draw(gameTime);

        string fps = "fps:" + (int)MathHelper.Max(0f, (float)(1 / gameTime.ElapsedGameTime.TotalSeconds));

        _spriteBatch.DrawString(
            font,
            fps,
            Vector2.Zero + Vector2.UnitY * 18 + Vector2.One,
            Color.Black,
            0,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            1
        );
        _spriteBatch.DrawString(
            font,
            fps,
            Vector2.Zero + Vector2.UnitY * 18,
            Color.White,
            0,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            1
        );

        _spriteBatch.DrawString(
            font,
            MathF.Round(Player.velocity.X / 8 * 60).ToString(),
            Vector2.Zero + Vector2.UnitY * 30,
            Color.White,
            0,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            1
        );

        _spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        float scale = (float)Math.Ceiling(screenSize.Y / 180f);

        _spriteBatch.Draw(
            renderTarget,
            Vector2.Zero,
            null,
            Color.White,
            0,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0
        );

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
