using System;
using System.Text.RegularExpressions;
using WindowsInput;
using WindowsInput.Native;

namespace WinCorners
{
    public class HotKeyCommand : ICommand
    {
        private InputSimulator InSimulator = new InputSimulator();

        public VirtualKeyCode[] Keys;

        public bool Run()
        {
            if (Keys == null)
                return false;

            for (int i = 0; i < Keys.Length; i++)
            {
                InSimulator.Keyboard.KeyDown(Keys[i]);
            }

            for (int i = 0; i < Keys.Length; i++)
            {
                InSimulator.Keyboard.KeyUp(Keys[i]);
            }

            return true;
        }

        public string ToSaveString()
        {
            string ret = "{Keys:";

            for (int i = 0; i < Keys.Length; i++)
            {
                ret += Keys[i].ToString();

                if (i != Keys.Length - 1)
                    ret += ",";
            }

            return ret + "}";
        }

        public void FromSaveString(string s)
        {
            Match match = Regex.Match(s, "{Keys:(.*)}");

            string[] keys = match.Groups[1].Value.Split(',');

            Keys = new VirtualKeyCode[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                Keys[i] = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), keys[i]);
            }
        }

        public bool ShowSettingsWindow(bool isEdit)
        {
            HotKeyInputWindow hkiw = new HotKeyInputWindow();

            if (hkiw.ShowDialog() == true)
            {
                Keys = hkiw.VirtualKeys.ToArray();

                return true;
            }

            return false;
        }

        public string GetCommandName()
        {
            return "HotKey";
        }

        public string GetCommandValues()
        {
            if (Keys == null)
                return "";

            string ret = "";

            for (int i = 0; i < Keys.Length; i++)
            {
                ret += Keys[i].ToKey();

                if (i != Keys.Length - 1)
                    ret += "+";
            }

            return ret;
        }
    }
}