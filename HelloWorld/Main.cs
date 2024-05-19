using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using HelloWorld.Core;
using HelloWorld.Registries;
using HelloWorld.Graphics;
using Microsoft.Xna.Framework.Audio;

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

    Player ClientPlayer;

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

    public static readonly TileRegistry TileRegistry = new TileRegistry();
    public static readonly SoundRegistry SoundRegistry = new SoundRegistry();

    public Main()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        ContentManager = Content;

        GamepadEnabled = false;
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

        IsFixedTimeStep = false;

        ClientPlayer = new Player()
        {
            position = new Vector2(320 / 2, 180 / 2)
        };

        Level = new Level();

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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        OnePixel = Content.Load<Texture2D>("Images/Other/onepixel");

        SoundRegistry.Register();

        Level.LoadContent();
        clientPlayerRenderer.LoadContent();
        ClientPlayer.LoadContent();

        font = SpriteFontBuilder.BuildDefaultFont(Content.Load<Texture2D>("Fonts/default"));
    }

    Vector2 inputVector = Vector2.Zero;

    private static KeyboardState keyboardState;
    private static GamePadState gamePadState;
    private static JoystickState joystickState;

    public static KeyboardState KeyboardState { get => keyboardState; protected set => keyboardState = value; }
    public static GamePadState GamePadState { get => gamePadState; protected set => gamePadState = value; }
    public static JoystickState JoystickState { get => joystickState; protected set => joystickState = value; }

    protected override void Update(GameTime gameTime)
    {
        keyboardState = Input.GetKeyboardState();
        gamePadState  = Input.GetGamePadState();
        joystickState = Input.GetJoystickState();

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

        ClientPlayer.Update(delta);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        DrawContext drawContext = new DrawContext(gameTime, _spriteBatch);

        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        clientPlayerRenderer.DrawPlayer(ClientPlayer);

        Level.Draw(drawContext);

        string fps = "fps:" + (int)(1/gameTime.ElapsedGameTime.TotalSeconds);

        _spriteBatch.DrawString(
            font,
            fps,
            Vector2.Zero + Vector2.One,
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
            Vector2.Zero,
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
