using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelloWorld.Graphics;

public interface IDrawable
{
    public abstract void Draw(DrawContext drawContext);
}
