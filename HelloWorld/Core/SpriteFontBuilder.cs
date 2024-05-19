using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelloWorld.Core;

public static class SpriteFontBuilder
{
    internal class P
    {
        public int height;

        public P(int height)
        {
            this.height = height;
        }

        public Point to(int width)
        {
            return new Point(width, height); // me too lazy
        }
    }

    public static SpriteFont BuildDefaultFont(Texture2D texture)
    {
        P p = new P(14);

        List<Rectangle> glyphs = new List<Rectangle> {
            new Rectangle(new Point(2, 2), p.to(3)),
            new Rectangle(new Point(83, 50), p.to(2)),
            new Rectangle(new Point(78, 50), p.to(3)),
            new Rectangle(new Point(71, 50), p.to(5)),
            new Rectangle(new Point(64, 50), p.to(5)),
            new Rectangle(new Point(56, 50), p.to(6)),
            new Rectangle(new Point(47, 50), p.to(7)),
            new Rectangle(new Point(44, 50), p.to(1)),
            new Rectangle(new Point(39, 50), p.to(3)),
            new Rectangle(new Point(34, 50), p.to(3)),
            new Rectangle(new Point(87, 50), p.to(3)),
            new Rectangle(new Point(27, 50), p.to(5)),
            new Rectangle(new Point(16, 50), p.to(2)),
            new Rectangle(new Point(10, 50), p.to(4)),
            new Rectangle(new Point(7, 50), p.to(1)),
            new Rectangle(new Point(2, 50), p.to(3)),
            new Rectangle(new Point(117, 34), p.to(6)),
            new Rectangle(new Point(109, 34), p.to(6)),
            new Rectangle(new Point(102, 34), p.to(5)),
            new Rectangle(new Point(95, 34), p.to(5)),
            new Rectangle(new Point(87, 34), p.to(6)),
            new Rectangle(new Point(20, 50), p.to(5)),
            new Rectangle(new Point(92, 50), p.to(6)),
            new Rectangle(new Point(100, 50), p.to(6)),
            new Rectangle(new Point(108, 50), p.to(6)),
            new Rectangle(new Point(5, 82), p.to(6)),
            new Rectangle(new Point(2, 82), p.to(1)),
            new Rectangle(new Point(121, 66), p.to(2)),
            new Rectangle(new Point(116, 66), p.to(3)),
            new Rectangle(new Point(110, 66), p.to(4)),
            new Rectangle(new Point(105, 66), p.to(3)),
            new Rectangle(new Point(97, 66), p.to(6)),
            new Rectangle(new Point(90, 66), p.to(5)),
            new Rectangle(new Point(82, 66), p.to(6)),
            new Rectangle(new Point(74, 66), p.to(6)),
            new Rectangle(new Point(67, 66), p.to(5)),
            new Rectangle(new Point(59, 66), p.to(6)),
            new Rectangle(new Point(52, 66), p.to(5)),
            new Rectangle(new Point(45, 66), p.to(5)),
            new Rectangle(new Point(37, 66), p.to(6)),
            new Rectangle(new Point(29, 66), p.to(6)),
            new Rectangle(new Point(25, 66), p.to(2)),
            new Rectangle(new Point(17, 66), p.to(6)),
            new Rectangle(new Point(9, 66), p.to(6)),
            new Rectangle(new Point(2, 66), p.to(5)),
            new Rectangle(new Point(116, 50), p.to(7)),
            new Rectangle(new Point(79, 34), p.to(6)),
            new Rectangle(new Point(71, 34), p.to(6)),
            new Rectangle(new Point(63, 34), p.to(6)),
            new Rectangle(new Point(30, 18), p.to(6)),
            new Rectangle(new Point(17, 18), p.to(6)),
            new Rectangle(new Point(10, 18), p.to(5)),
            new Rectangle(new Point(2, 18), p.to(6)),
            new Rectangle(new Point(117, 2), p.to(6)),
            new Rectangle(new Point(108, 2), p.to(7)),
            new Rectangle(new Point(99, 2), p.to(7)),
            new Rectangle(new Point(91, 2), p.to(6)),
            new Rectangle(new Point(83, 2), p.to(6)),
            new Rectangle(new Point(75, 2), p.to(6)),
            new Rectangle(new Point(25, 18), p.to(3)),
            new Rectangle(new Point(70, 2), p.to(3)),
            new Rectangle(new Point(59, 2), p.to(3)),
            new Rectangle(new Point(52, 2), p.to(5)),
            new Rectangle(new Point(45, 2), p.to(5)),
            new Rectangle(new Point(41, 2), p.to(2)),
            new Rectangle(new Point(34, 2), p.to(5)),
            new Rectangle(new Point(27, 2), p.to(5)),
            new Rectangle(new Point(21, 2), p.to(4)),
            new Rectangle(new Point(14, 2), p.to(5)),
            new Rectangle(new Point(7, 2), p.to(5)),
            new Rectangle(new Point(64, 2), p.to(4)),
            new Rectangle(new Point(38, 18), p.to(5)),
            new Rectangle(new Point(107, 18), p.to(5)),
            new Rectangle(new Point(45, 18), p.to(2)),
            new Rectangle(new Point(51, 34), p.to(3)), // #106 has kerning left of -1
            new Rectangle(new Point(44, 34), p.to(5)),
            new Rectangle(new Point(40, 34), p.to(2)),
            new Rectangle(new Point(31, 34), p.to(7)),
            new Rectangle(new Point(24, 34), p.to(5)),
            new Rectangle(new Point(17, 34), p.to(5)),
            new Rectangle(new Point(10, 34), p.to(5)),
            new Rectangle(new Point(2, 34), p.to(6)),
            new Rectangle(new Point(120, 18), p.to(5)),
            new Rectangle(new Point(56, 34), p.to(5)),
            new Rectangle(new Point(114, 18), p.to(4)),
            new Rectangle(new Point(100, 18), p.to(5)),
            new Rectangle(new Point(93, 18), p.to(5)),
            new Rectangle(new Point(84, 18), p.to(7)),
            new Rectangle(new Point(77, 18), p.to(5)),
            new Rectangle(new Point(70, 18), p.to(5)),
            new Rectangle(new Point(64, 18), p.to(4)),
            new Rectangle(new Point(58, 18), p.to(4)),
            new Rectangle(new Point(55, 18), p.to(1)),
            new Rectangle(new Point(49, 18), p.to(4)),
            new Rectangle(new Point(13, 82), p.to(5)),
            new Rectangle(new Point(20, 82), p.to(9)), // last has kerning left of 3
        };

        List<Rectangle> croppings = new List<Rectangle>();
        List<Vector3> kernings = new List<Vector3>();

        for(int i = 0; i < 96; i++)
        {
            croppings.Add(new Rectangle(Point.Zero, glyphs[i].Size));
            kernings.Add(new Vector3(3, 0, 3));
        }

        kernings[74] = new Vector3(2, 0, 3);
        kernings[95] = new Vector3(5, 0, 3);

        return new SpriteFont
        (
            texture,
            glyphs,
            croppings,
            new List<char> { // characters
                '\x20',
                '\x21',
                '\x22',
                '\x23',
                '\x24',
                '\x25',
                '\x26',
                '\x27',
                '\x28',
                '\x29',
                '\x2A',
                '\x2B',
                '\x2C',
                '\x2D',
                '\x2E',
                '\x2F',
                '\x30',
                '\x31',
                '\x32',
                '\x33',
                '\x34',
                '\x35',
                '\x36',
                '\x37',
                '\x38',
                '\x39',
                '\x3A',
                '\x3B',
                '\x3C',
                '\x3D',
                '\x3E',
                '\x3F',
                '\x40',
                '\x41',
                '\x42',
                '\x43',
                '\x44',
                '\x45',
                '\x46',
                '\x47',
                '\x48',
                '\x49',
                '\x4A',
                '\x4B',
                '\x4C',
                '\x4D',
                '\x4E',
                '\x4F',
                '\x50',
                '\x51',
                '\x52',
                '\x53',
                '\x54',
                '\x55',
                '\x56',
                '\x57',
                '\x58',
                '\x59',
                '\x5A',
                '\x5B',
                '\x5C',
                '\x5D',
                '\x5E',
                '\x5F',
                '\x60',
                '\x61',
                '\x62',
                '\x63',
                '\x64',
                '\x65',
                '\x66',
                '\x67',
                '\x68',
                '\x69',
                '\x6A',
                '\x6B',
                '\x6C',
                '\x6D',
                '\x6E',
                '\x6F',
                '\x70',
                '\x71',
                '\x72',
                '\x73',
                '\x74',
                '\x75',
                '\x76',
                '\x77',
                '\x78',
                '\x79',
                '\x7A',
                '\x7B',
                '\x7C',
                '\x7D',
                '\x7E',
                '\u25AF',
            },
            14, // line height
            1,  // spacing
            kernings,
            '\u25AF' // default character
        );
    }
}
