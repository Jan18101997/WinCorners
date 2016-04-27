using System.Windows;

namespace WinCorners
{
    public class HotCorner
    {
        public Point Position1 { get; set; } = new Point();

        public Point Position2 { get; set; } = new Point();

        public ICommand Command { get; set; }

        public bool RunOnce { get; set; }

        public bool DisableAtMouseDown { get; set; }

        private bool runoncelast = false;

        public void Update(Point cursorPos)
        {
            if (cursorPos.X >= Position1.X && cursorPos.Y >= Position1.Y && cursorPos.X <= Position2.X && cursorPos.Y <= Position2.Y)
            {
                if (DisableAtMouseDown && (WinApi.GetKeyState(0x1) & 0x80) != 0)
                    runoncelast = true;

                if (runoncelast == false)
                    Command.Run();

                runoncelast = RunOnce;
            }
            else
                runoncelast = false;
        }
    }
}