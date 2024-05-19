using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelloWorld.Graphics;

public class DrawContext
{
    public readonly GameTime gameTime;
    public readonly SpriteBatch spriteBatch;
    public readonly float delta;

    public DrawContext(GameTime gameTime, SpriteBatch spriteBatch)
    {
        this.gameTime = gameTime;
        this.spriteBatch = spriteBatch;

        delta = (float)gameTime.ElapsedGameTime.TotalSeconds * 60;
    }
}
