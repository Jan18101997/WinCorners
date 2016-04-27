using System.Collections.Generic;
using System.Windows.Input;
using WindowsInput.Native;

namespace WinCorners
{
    internal static class Extention
    {
        public static Screen getScreen(this List<Screen> screens, string deviceID)
        {
            foreach (Screen item in screens)
            {
                if (item.ScreendID == deviceID)
                    return item;
            }

            return null;
        }

        public static VirtualKeyCode ToVirtualKeyCode(this Key sender)
        {
            return (VirtualKeyCode)KeyInterop.VirtualKeyFromKey(sender);
        }

        public static Key ToKey(this VirtualKeyCode sender)
        {
            return KeyInterop.KeyFromVirtualKey((int)sender);
        }
    }
}