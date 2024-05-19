using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelloWorld.Graphics;

public interface IPlayerRenderer
{
    public void LoadContent();

    public void DrawPlayer(Player player);
}
