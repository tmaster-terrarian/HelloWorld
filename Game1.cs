using System;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HelloWorld;

public class Game1 : Game
{
    Texture2D ballTexture;
    Vector2 ballPosition;
    Vector2 ballOrigin;
    Vector2 ballVelocity;
    float ballSpeed;

    public float gamepadDeadzone;
    public bool gamepadEnabled;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Vector2 SCREEN_SIZE {
        get {
            return new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }
    }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        gamepadEnabled = false;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        ballPosition = new Vector2(SCREEN_SIZE.X / 2, SCREEN_SIZE.Y / 2);
        ballOrigin = Vector2.Zero;
        ballVelocity = Vector2.Zero;
        ballSpeed = 4f;

        gamepadDeadzone = 0.1f;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here

        ballTexture = Content.Load<Texture2D>("Images/ball");
        ballOrigin = new Vector2(0.5f * ballTexture.Width, 0.5f * ballTexture.Height);
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
                Math.Abs(jsState.Axes[0]) >= gamepadDeadzone ? Math.Sign(jsState.Axes[0]) : 0,
                Math.Abs(jsState.Axes[1]) >= gamepadDeadzone ? Math.Sign(jsState.Axes[1]) : 0
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

        ballVelocity.X = Utils.Approach(ballVelocity.X, velTarget.X, 0.24f * delta);
        ballVelocity.Y = Utils.Approach(ballVelocity.Y, velTarget.Y, 0.24f * delta);

        ballPosition += ballVelocity;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        _spriteBatch.Begin();
        _spriteBatch.Draw(
            ballTexture,            // texture
            ballPosition,           // position
            null,                   // idk what sourceRectangle means yet
            Color.White,            // blend color
            0f,                     // rotation
            ballOrigin,             // offset
            Vector2.One,            // scale
            SpriteEffects.None,     // flip; horizontal OR vertical
            0f                      // depth, not sure if higher depth is closer or further away from view
        );
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
