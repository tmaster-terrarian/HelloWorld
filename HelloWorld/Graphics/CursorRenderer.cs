using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelloWorld.Graphics;

public class CursorRenderer : IDrawable
{
    private Texture2D _texture;

    private readonly SpriteBatch _spriteBatch;

    private Vector2 _cursorPos = Vector2.Zero;
    private Vector2 _targetCursorPos = Vector2.Zero;
    private Vector2 _lastTargetCursorPos = Vector2.Zero;

    public enum DrawState
    {
        Visible,
        Hidden,
    }
    private DrawState _state = DrawState.Hidden;

    private float _alpha = 1f;
    private float _alphaTarget = 1f;
    private float _alphaRate = 3f/60f;

    private float _spriteFrameID = 0;
    private Rectangle _spriteFrame;

    public bool positionInvalid = false;

    public DrawState State
    {
        get => _state;
        set {
            switch(value)
            {
                case DrawState.Visible:
                {
                    if(_state == DrawState.Hidden && _alpha == 0)
                    {
                        _cursorPos = _targetCursorPos;
                    }
                    _alphaTarget = 1;
                    break;
                }
                case DrawState.Hidden:
                {
                    _alphaTarget = 0;
                    break;
                }
            }
            _state = value;
        }
    }

    public Vector2 CursorPos
    {
        get => _targetCursorPos;
        set {
            _lastTargetCursorPos = _targetCursorPos;
            _targetCursorPos = value;
        }
    }

    public CursorRenderer(GraphicsDevice graphicsDevice)
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _spriteFrame = new Rectangle(0, 0, 16, 16);
    }

    public void LoadContent()
    {
        _texture = Main.ContentManager.Load<Texture2D>("Images/UI/cursor");
    }

    public void Update(float delta)
    {
        _alpha = MathUtil.Approach(_alpha, _alphaTarget * (positionInvalid ? 0.5f : 1), _alphaRate * delta);

        _spriteFrameID = MathUtil.Approach(_spriteFrameID, 0, 0.2f * delta);

        if(_lastTargetCursorPos != _targetCursorPos)
        {
            _spriteFrameID = 1;
        }

        // uncomment for animation
        // _spriteFrame.X = (int)MathF.Ceiling(_spriteFrameID) * 16;

        _cursorPos = Vector2.LerpPrecise(_cursorPos, _targetCursorPos, 0.25f * delta);
    }

    public void Draw(DrawContext drawContext)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _spriteBatch.Draw(
            _texture,
            _cursorPos,
            _spriteFrame,
            Color.White * _alpha,
            0,
            Vector2.One * 4,
            1,
            SpriteEffects.None,
            1
        );

        _spriteBatch.End();
    }
}
