using System;
using System.Windows.Media;

namespace bliUtils
{

    public class ColorUtils
    {
        public static Color getColorFromInt(int c)
        {

            byte[] bytes = BitConverter.GetBytes(c);
            return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        public static int colorToInt(Color c)
        {

            return (c.A << 24) | (c.R << 16) | (c.G << 8) | c.B;
        }

        public static string ToHexColor(Color color)
        {
            return String.Format("#{0}{1}{2}{3}",
                                 color.A.ToString("X2"),
                                 color.R.ToString("X2"),
                                 color.G.ToString("X2"),
                                 color.B.ToString("X2"));
        }

        public static Color fromHexColor(string hex)
        {
            if (hex != null)
                return (Color)ColorConverter.ConvertFromString(hex);
            else
                return Colors.Transparent;
        }
    }
}
