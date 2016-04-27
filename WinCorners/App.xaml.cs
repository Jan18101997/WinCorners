using Hardcodet.Wpf.TaskbarNotification;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WinCorners
{
    public partial class App : Application
    {
        private MainWindow mainWindow;
        private ShowPositionWindow showPositionWindow;
        public static List<Screen> Screens;
        private bool isRunning = true;
        private bool isPaused = false;
        private bool showPosition = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Screen.LoadScreens(out Screens) == false)
            {
                Screens = new List<Screen>();
                MessageBox.Show("Settings not loaded");
            }

            TaskbarIcon taskBarIcon = new TaskbarIcon();
            ContextMenu taskBarIconMenu = new ContextMenu();
            MenuItem taskBarIconMenuExit = new MenuItem();
            MenuItem taskBarIconMenuPaused = new MenuItem();
            MenuItem taskBarIconMenuCurrentPos = new MenuItem();

            taskBarIconMenuExit.Header = "Exit";
            taskBarIconMenuExit.Click += TaskBarIconMenuExit_Click;

            taskBarIconMenuPaused.Header = "Paused";
            taskBarIconMenuPaused.IsCheckable = true;
            taskBarIconMenuPaused.Checked += TaskBarIconMenuPaused_CheckedChanged;
            taskBarIconMenuPaused.Unchecked += TaskBarIconMenuPaused_CheckedChanged;

            taskBarIconMenuCurrentPos.Header = "Show Position";
            taskBarIconMenuCurrentPos.Click += TaskBarIconMenuCurrentPos_Click;

            taskBarIconMenu.Items.Add(taskBarIconMenuCurrentPos);
            taskBarIconMenu.Items.Add(taskBarIconMenuPaused);
            taskBarIconMenu.Items.Add(new Separator());
            taskBarIconMenu.Items.Add(taskBarIconMenuExit);

            taskBarIcon.Icon = WinCorners.Properties.Resources.AppIcon;
            taskBarIcon.ContextMenu = taskBarIconMenu;
            taskBarIcon.TrayMouseDoubleClick += TaskBarIcon_TrayMouseDoubleClick;
            taskBarIcon.Visibility = Visibility.Visible;

            Thread t = new Thread(CornerThread);
            t.IsBackground = true;
            t.Start();
        }

        private void TaskBarIconMenuCurrentPos_Click(object sender, RoutedEventArgs e)
        {
            if (showPositionWindow != null && showPositionWindow.Visibility == Visibility.Visible)
            {
                showPositionWindow.Close();

                showPositionWindow = null;
            }
            else
            {
                showPositionWindow = new ShowPositionWindow();
                showPositionWindow.Closed += ShowPositionWindow_Closed;

                showPositionWindow.Show();

                showPosition = true;
            }
        }

        private void ShowPositionWindow_Closed(object sender, System.EventArgs e)
        {
            showPosition = false;
        }

        private void TaskBarIconMenuPaused_CheckedChanged(object sender, RoutedEventArgs e)
        {
            isPaused = ((MenuItem)sender).IsChecked;
        }

        private void TaskBarIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (mainWindow != null && mainWindow.Visibility == Visibility.Visible)
            {
                mainWindow.Close();

                mainWindow = null;
            }
            else
            {
                mainWindow = new MainWindow();

                mainWindow.Show();
            }
        }

        private void TaskBarIconMenuExit_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow != null && mainWindow.Visibility == Visibility.Visible)
            {
                mainWindow.Close();

                mainWindow = null;
            }

            isRunning = false;

            this.Shutdown();
        }

        public void CornerThread()
        {
            POINT lpPoint;
            Point cursorPos;

            while (isRunning)
            {
                WinApi.GetCursorPos(out lpPoint);

                cursorPos = lpPoint;

                if (showPosition)
                    showPositionWindow.Position = cursorPos;

                if (isPaused == false)
                {
                    foreach (Screen screen in Screens)
                    {
                        foreach (HotCorner corner in screen.Corners)
                        {
                            if (corner.Command != null)
                                corner.Update(cursorPos);
                        }
                    }
                }

                Thread.Sleep(10);
            }
        }
    }
}