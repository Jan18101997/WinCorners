using Jan18101997.Windows.Installer;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Installer
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public const string ApplicationID = "{WinCorners}";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Process.GetCurrentProcess().MainModule.FileName.Contains("uninstaller"))
                uninstallApp();
            else
            {
                MainWindow main = new MainWindow();

                main.Show();
            }
        }

        private void uninstallApp()
        {
            if (MessageBox.Show("Do you realy want to unsinstall this Application?", "Uninstall", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string dir = "";

                    RegisterUninstaller ru = RegisterUninstaller.ReadKey(ApplicationID, RegisterFor.AllUser);
                    dir = ru.InstallLocation;
                    ru.UnregisterApp();

                    if (string.IsNullOrEmpty(dir))
                        throw new ArgumentException("Unable to find InstallDir");

                    string startDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\InfoCashInifileCheck";
                    if (Directory.Exists(startDir))
                        Directory.Delete(startDir, true);

                    Process proc = new Process();
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.StartInfo.Verb = "runas";
                    proc.StartInfo.FileName = "cmd.exe";
                    proc.StartInfo.Arguments = "/c \"timeout 4 & rd /S /Q \"" + dir + "\"\" & mshta \"javascript:var sh=new ActiveXObject( 'WScript.Shell' ); sh.Popup( 'Application removed!', 10, 'Done!', 64 );close()\"";
                    proc.Start();
                    Environment.Exit(1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to uninstall! " + ex, "Uninstall", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1359);
                }
            }

            Environment.Exit(1223);
        }
    }
}