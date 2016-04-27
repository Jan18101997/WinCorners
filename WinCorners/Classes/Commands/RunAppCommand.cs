using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WinCorners
{
    public class RunAppCommand : ICommand
    { 
        public Process Process { get; set; } = new Process();

        public bool Run()
        {
            try
            {
                Process.Start();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string ToSaveString()
        {
            return "{FilePath:" + Process.StartInfo.FileName + "}{Arguments:" + Process.StartInfo.Arguments + "}";
        }

        public void FromSaveString(string s)
        {
            Match match = Regex.Match(s, "{FilePath:(.*)}{Arguments:(.*)}");

            Process.StartInfo.FileName = match.Groups[1].Value;
            Process.StartInfo.Arguments = match.Groups[2].Value;
        }

        public bool ShowSettingsWindow(bool isEdit)
        {
            TextInputWindow tiCommand = null;
            TextInputWindow tiArgument = null;

            if (isEdit)
            {
                tiCommand = new TextInputWindow("Command", Process.StartInfo.FileName);
                tiArgument = new TextInputWindow("Arguments", Process.StartInfo.Arguments);
            }
            else
            {
                tiCommand = new TextInputWindow("Command");
                tiArgument = new TextInputWindow("Arguments");
            }

            if (tiCommand.ShowDialog() == false)
                return false;

            if (tiArgument.ShowDialog() == false)
                return false;

            Process.StartInfo.FileName = tiCommand.Text;
            Process.StartInfo.Arguments = tiArgument.Text;

            return true;
        }

        public string GetCommandName()
        {
            return "Run Application";
        }

        public string GetCommandValues()
        {
            return Process.StartInfo.FileName;
        }
    }
}