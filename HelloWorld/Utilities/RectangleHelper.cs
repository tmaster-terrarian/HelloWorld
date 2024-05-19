using Microsoft.Xna.Framework;

namespace HelloWorld;

public static class RectangleHelper
{
    public static Rectangle Shift(Rectangle rectangle, Point offset)
    {
        rectangle.X += offset.X;
        rectangle.Y += offset.Y;
        return rectangle;
    }
    public static Rectangle Shift(Rectangle rectangle, Vector2 offset) => Shift(rectangle, new Point((int)offset.X, (int)offset.Y));

    public static Rectangle ScalePosition(Rectangle rectangle, Point scale)
    {
        rectangle.X *= scale.X;
        rectangle.Y *= scale.Y;
        rectangle.Width *= scale.X;
        rectangle.Height *= scale.Y;
        return rectangle;
    }

    public static Rectangle ScalePosition(Rectangle rectangle, Vector2 scale)
    {
        rectangle.X = (int)(rectangle.X * scale.X);
        rectangle.Y = (int)(rectangle.Y * scale.Y);
        rectangle.Width = (int)(rectangle.Width * scale.X);
        rectangle.Height = (int)(rectangle.Height * scale.Y);
        return rectangle;
    }

    public static Rectangle ScalePosition(Rectangle rectangle, int scale) => ScalePosition(rectangle, new Point(scale, scale));
    public static Rectangle ScalePosition(Rectangle rectangle, float scale) => ScalePosition(rectangle, Vector2.One * scale);
}
