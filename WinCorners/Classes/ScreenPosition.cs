using System.Drawing;

namespace WinCorners
{
    public class ScreenPosition
    {
        public double Top;

        public double Bottom;

        public double Left;

        public double Right;

        public string ToSaveString()
        {
            return Top + "," + Bottom + "," + Left + "," + Right;
        }

        public static ScreenPosition FromSaveString(string ss)
        {
            ScreenPosition pos = new ScreenPosition();

            string[] parts = ss.Split(',');

            pos.Top = double.Parse(parts[0]);
            pos.Bottom = double.Parse(parts[1]);
            pos.Left = double.Parse(parts[2]);
            pos.Right = double.Parse(parts[3]);

            return pos;
        }

        public static implicit operator ScreenPosition(Rectangle rect)
        {
            return new ScreenPosition { Top = rect.Top, Bottom = rect.Bottom, Left = rect.Left, Right = rect.Right };
        }
    }
}