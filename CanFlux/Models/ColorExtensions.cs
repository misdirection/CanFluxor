using System.Drawing;

namespace CanFlux.Models
{
    public static class ColorExtensions
    {
        public static string ToColorCode(this Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        public static Color ToColor(this string str)
        {
            var red = int.Parse(str.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            var green = int.Parse(str.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            var blue = int.Parse(str.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(255, red, green, blue);
        }
    }
}
