using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using GAY.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GAY;

public class TheGame : Game
{
    Texture2D ballTexture;
    Vector2 ballPosition;
    Vector2 ballOrigin;
    Vector2 ballVelocity;
    float ballSpeed;

    Level level;

    Rectangle ballBBox;

    RenderTarget2D renderTarget;

    SpriteFont font;

    public int gamepadDeadZone;
    public bool gamepadEnabled;
    public Point screenSize {
        get {
            return new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }
        set {
            _graphics.PreferredBackBufferWidth = value.X;
            _graphics.PreferredBackBufferHeight = value.Y;
        }
    }

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public TheGame()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        gamepadEnabled = false;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        Window.Title = "MONOGAYME";
        screenSize = new Point(320 * 3, 180 * 3);

        ballPosition = new Vector2(320 / 2, 180 / 2);
        ballOrigin = Vector2.Zero;
        ballVelocity = Vector2.Zero;
        ballSpeed = 4f;

        // new JsonSerializer().Deserialize()

        level = new Level(Content.ServiceProvider);

        gamepadDeadZone = 4096;

        this.renderTarget = new RenderTarget2D(
            GraphicsDevice,
            320,
            180,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24
        );

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here

        level.LoadContent();

        ballTexture = Content.Load<Texture2D>("Images/ball");
        ballOrigin = new Vector2(0.5f * ballTexture.Width, 0.5f * ballTexture.Height);
        //ballBBox = new BoundingBox(new Vector3(new Vector2(0, 0), 0), new Vector3(new Vector2(ballTexture.Width, ballTexture.Height), 0.0001f));
        ballBBox = new Rectangle(0, 0, ballTexture.Width, ballTexture.Height);

        font = SpriteFontBuilder.BuildDefaultFont(Content.Load<Texture2D>("Fonts/default"));
    }

    protected override void Update(GameTime gameTime)
    {
        var kbState = Keyboard.GetState();
        var gpState = GamePad.GetState(0);
        var jsState = Joystick.GetState(0);

        if(gpState.Buttons.Back == ButtonState.Pressed || kbState.IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds * 60;

        Vector2 inputVector;

        if(Joystick.LastConnectedIndex == 0 && gamepadEnabled)
        {
            inputVector = new Vector2(
                Math.Abs(jsState.Axes[0]) >= gamepadDeadZone ? Math.Sign(jsState.Axes[0]) : 0,
                Math.Abs(jsState.Axes[1]) >= gamepadDeadZone ? Math.Sign(jsState.Axes[1]) : 0
            );
        }
        else
        {
            inputVector = new Vector2(
                (kbState.IsKeyDown(Keys.Right) ? 1f : 0f) - (kbState.IsKeyDown(Keys.Left) ? 1f : 0f),
                (kbState.IsKeyDown(Keys.Down) ? 1f : 0f) - (kbState.IsKeyDown(Keys.Up) ? 1f : 0f)
            );
        }

        if(inputVector != Vector2.Zero) inputVector.Normalize();

        Vector2 velTarget = inputVector * ballSpeed * delta;

        Utils.Approach(ref ballVelocity.X, velTarget.X, 0.24f * delta);
        Utils.Approach(ref ballVelocity.Y, velTarget.Y, 0.24f * delta);

        ballPosition += ballVelocity;
        ballBBox.X = (int)(ballPosition.X - ballOrigin.X);
        ballBBox.Y = (int)(ballPosition.Y - ballOrigin.Y);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _spriteBatch.Draw(
            ballTexture,
            new Vector2((int)Math.Round(ballPosition.X), (int)Math.Round(ballPosition.Y)),
            null,
            Color.White,
            0f,
            ballOrigin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
        level.Draw(gameTime, _spriteBatch);

        _spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        float scale = (float)Math.Ceiling((float)screenSize.Y / 180);

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

        string fps = Math.Floor(gameTime.ElapsedGameTime.TotalSeconds * 60 * 60).ToString() + "fps";

        _spriteBatch.DrawString(
            font,
            fps,
            Vector2.Zero + Vector2.One * scale,
            Color.Black,
            0,
            Vector2.Zero,
            scale,
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
            scale,
            SpriteEffects.None,
            1
        );

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
