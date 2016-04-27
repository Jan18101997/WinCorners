using Jan18101997.Windows.Installer;
using Jan18101997.Windows.Shell;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace Installer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            filePath.Text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WinCorners";
        }

        private string outputText { get { return output.Text; } set { output.Text = value; outputScroll.ScrollToEnd(); } }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
            install.IsEnabled = false;
            filePath.IsEnabled = false;
            filePathChange.IsEnabled = false;
            progreass.IsIndeterminate = true;

            Thread installThread = new Thread(installApp);
            installThread.IsBackground = true;
            installThread.Start();
        }

        private void PathChange_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                filePath.Text = fbd.SelectedPath;
        }

        private void installApp()
        {
            try
            {
                string installPath = "";
                bool deskLink = false;
                bool startLink = false;
                RegisterFor userRegister = RegisterFor.AllUser;

                Dispatcher.Invoke(new Action(() =>
                {
                    installPath = filePath.Text;
                    deskLink = (bool)desktopLink.IsChecked;
                    startLink = (bool)startmenulink.IsChecked;

                    if ((bool)installJustMe.IsChecked)
                        userRegister = RegisterFor.CurrentUser;
                }));

                if (Directory.Exists(installPath) == true) //prüft pb ziel leer ist wenn ja frag nach wegen leeren
                    if (Directory.GetFiles(installPath).Length != 0)
                        if (MessageBox.Show("The Path is not Empty! Do you want to Delete all files?", "Not Empty!", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                            Directory.Delete(installPath, true);

                Directory.CreateDirectory(installPath);

                Assembly assembly = Assembly.GetExecutingAssembly();
                string[] resources = assembly.GetManifestResourceNames(); //alle embedet files

                Dispatcher.Invoke(new Action(() =>
                {
                    progreass.IsIndeterminate = false;
                    progreass.Maximum = resources.Length + 1;
                }));

                ExtractFiles(resources, assembly, installPath);

                Dispatcher.Invoke(new Action(() =>
                {
                    progreass.Value = progreass.Maximum;
                    outputText += "Extracting uninstaller..." + Environment.NewLine;
                }));

                File.Copy(Process.GetCurrentProcess().MainModule.FileName, installPath + "\\uninstaller.exe"); //kopiert den installer/uninstaller

                Dispatcher.Invoke(new Action(() =>
                {
                    progreass.IsIndeterminate = true;
                    outputText += "Creating registry keys..." + Environment.NewLine;
                }));

                if (deskLink)
                    MSShellLink.CreateLink(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\WinCorners.lnk", installPath + "\\WinCorners.exe");

                if (startLink)
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\WinCorners"); //erstellt startmenü ordner
                    MSShellLink.CreateLink(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\WinCorners\\WinCorners.lnk", installPath + "\\WinCorners.exe");
                    MSShellLink.CreateLink(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\WinCorners\\Uninstall.lnk", installPath + "\\uninstaller.exe");
                }

                RegisterUninstaller ru = new RegisterUninstaller();
                ru.RegisterFor = userRegister;
                ru.ApplicationIdentifier = App.ApplicationID;
                ru.ProductName = "WinCorners";
                ru.ProductVersion = "1.0.0";
                ru.Publisher = "Jan18101997";
                ru.InstallLocation = installPath;
                ru.UninstallString = installPath + "\\uninstaller.exe";
                ru.RegisterApp();

                MessageBox.Show("Done", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    outputText += "Error while installing " + ex.Message + Environment.NewLine;
                }));
            }

            Dispatcher.Invoke(new Action(() =>
            {
                outputText += "Done!" + Environment.NewLine;
                install.IsEnabled = true;
                filePath.IsEnabled = true;
                filePathChange.IsEnabled = true;
                progreass.IsIndeterminate = false;
                progreass.Value = 0;
            }));
        }

        private void ExtractFiles(string[] resources, Assembly assembly, string installPath)
        {
            for (int i = 0; i < resources.Length; i++) //Extracting
            {
                if (resources[i].Contains("Installer.InstallFiles.")) //sucht nach den passenden dateien
                {
                    string fileName = resources[i].Replace("Installer.InstallFiles.", ""); //ruft den dateinamen ab

                    Dispatcher.Invoke(new Action(() =>
                    {
                        progreass.Value = i + 1;
                        outputText += "Extracting " + fileName + "..." + Environment.NewLine;
                    }));

                    //entpackungs zeug
                    Stream si = assembly.GetManifestResourceStream(resources[i]);
                    FileStream so = new FileStream(installPath + "\\" + fileName, FileMode.Create);

                    byte[] buffer = new byte[1024];

                    int bytesRead;
                    while ((bytesRead = si.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        so.Write(buffer, 0, bytesRead);
                    }

                    so.Close();
                }
            }
        }
    }
}