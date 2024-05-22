using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using HelloWorld.Core;
using HelloWorld.Graphics;
using HelloWorld.Registries;

namespace HelloWorld;

public class Main : Game
{
    public int GamepadDeadZone;
    public bool GamepadEnabled;
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

    private static GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static Vector2 MouseWorldPos
    {
        get {
            return new Vector2(Mouse.GetState(MainWindow).X / (_graphics.PreferredBackBufferWidth / 320), Mouse.GetState(MainWindow).Y / (_graphics.PreferredBackBufferHeight / 180));
        }
    }

    private IPlayerRenderer clientPlayerRenderer;
    private CursorRenderer cursorRenderer;
    private InventoryRenderer inventoryRenderer;

    public static readonly TileRegistry TileRegistry = new TileRegistry();
    public static readonly SoundRegistry SoundRegistry = new SoundRegistry();
    public static readonly ItemRegistry ItemRegistry = new ItemRegistry();

    public Main()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        ContentManager = Content;

        GamepadEnabled = false;

        IsFixedTimeStep = true;
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

        Level = new Level(GraphicsDevice);
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

        clientPlayerRenderer.LoadContent();
        Player.LoadContent();

        cursorRenderer.LoadContent();

        inventoryRenderer.LoadContent();

        font = SpriteFontBuilder.BuildDefaultFont(Content.Load<Texture2D>("Fonts/default"));
    }

    Vector2 inputVector = Vector2.Zero;

    private static KeyboardState _keyboardState;
    private static GamePadState _gamePadState;
    private static JoystickState _joystickState;
    private static MouseState _mouseState;

    public static KeyboardState KeyboardState => _keyboardState;
    public static GamePadState GamePadState => _gamePadState;
    public static JoystickState JoystickState => _joystickState;
    public static MouseState MouseState => _mouseState;

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

        var mouseTile = Level.GetTileAtPosition(MouseWorldPos);
        if(mouseTile != null)
        {
            if(mouseTile.id == "air" || !Player.IsHoldingTileItem())
            {
                if
                (
                    Player.IsHoldingPlaceable()
                    && Level.TileMeeting(new Rectangle((Vector2.Floor(MouseWorldPos / Level.tileSize) * Level.tileSize - Vector2.One).ToPoint(), new Point(10, 10)))
                    && !Level.TileMeeting(new Rectangle((Vector2.Floor(MouseWorldPos / Level.tileSize) * Level.tileSize + Vector2.One * 2).ToPoint(), new Point(2, 2)))
                )
                {
                    cursorRenderer.CursorPos = Vector2.Floor(MouseWorldPos / Level.tileSize) * Level.tileSize;
                    cursorRenderer.State = CursorRenderer.DrawState.Visible;

                    cursorRenderer.positionInvalid = Vector2.Distance(
                        Player.Center - Vector2.UnitY * 3 * Level.tileSize,
                        Vector2.Floor(MouseWorldPos / Level.tileSize) * Level.tileSize
                    ) > 6 * Level.tileSize;
                }
                else
                    cursorRenderer.State = CursorRenderer.DrawState.Hidden;
            }
            else
            {
                cursorRenderer.CursorPos = Vector2.Floor(MouseWorldPos / Level.tileSize) * Level.tileSize;
                cursorRenderer.State = CursorRenderer.DrawState.Visible;

                cursorRenderer.positionInvalid = Vector2.Distance(
                    Player.Center - Vector2.UnitY * 3 * Level.tileSize,
                    Vector2.Floor(MouseWorldPos / Level.tileSize) * Level.tileSize
                ) > 6 * Level.tileSize;
            }
        }

        cursorRenderer.Update(delta);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        DrawContext drawContext = new DrawContext(gameTime, _spriteBatch);

        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        clientPlayerRenderer.DrawPlayer(Player);

        Level.Draw(drawContext);

        cursorRenderer.Draw(drawContext);

        inventoryRenderer.Draw(drawContext);

        string fps = "fps:" + (int)MathHelper.Max(0f, (float)(1 / gameTime.ElapsedGameTime.TotalSeconds));

        _spriteBatch.DrawString(
            font,
            fps,
            Vector2.Zero + Vector2.One + Vector2.UnitY * 18,
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
