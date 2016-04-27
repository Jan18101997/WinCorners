using System;
using System.Windows;
using System.Windows.Controls;

namespace WinCorners
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            foreach (System.Windows.Forms.Screen item in System.Windows.Forms.Screen.AllScreens)
            {
                screenSelect.Items.Add(new ComboBoxItem() { Content = item.DeviceName });
            }
        }

        private Screen currentScreen;
        private System.Windows.Forms.Screen currentScreenDevice;
        private bool isLoaded = false;
        private CornerPreviewWindow cornerPreviewWindow;

        private void screenSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (screenSelect.SelectedIndex == -1)
                return;

            currentScreenDevice = System.Windows.Forms.Screen.AllScreens[screenSelect.SelectedIndex];
            currentScreen = App.Screens.getScreen(((ComboBoxItem)screenSelect.SelectedItem).Content.ToString());

            if (currentScreen == null)
            {
                currentScreen = new Screen();
                currentScreen.ScreendID = ((ComboBoxItem)screenSelect.SelectedItem).Content.ToString();
                currentScreen.ScreenWidth = currentScreenDevice.Bounds.Height;
                currentScreen.ScreenHeight = currentScreenDevice.Bounds.Width;
                currentScreen.ScreenPosition = currentScreenDevice.Bounds;
                App.Screens.Add(currentScreen);
            }

            PrintScreenInformation();
        }

        private void PrintScreenInformation()
        {
            currentDevicePrimary.Content = currentScreenDevice.Primary.ToString();
            currentDeviceWidth.Content = currentScreenDevice.Bounds.Width + "px";
            currentDeviceHeight.Content = currentScreenDevice.Bounds.Height + "px";

            PrintCornerList();
        }

        private void PrintCornerList()
        {
            cornerList.Items.Clear();

            foreach (HotCorner item in currentScreen.Corners)
            {
                CornerListItem cli = new CornerListItem();
                cli.originalItem = item;
                cli.pos1 = item.Position1.ToString();
                cli.pos2 = item.Position2.ToString();
                cli.command = item.Command.GetCommandValues();

                cornerList.Items.Add(cli);
            }

            PrintScreenPreview();
        }

        private void PrintScreenPreview()
        {
            if (currentScreen == null || isLoaded == false)
                return;

            if (cornerPreviewWindow != null && cornerPreviewWindow.Visibility == Visibility.Visible && cornerPreviewWindow.Screen.ScreendID != currentScreen.ScreendID)
            {
                cornerPreviewWindow.Close();
                cornerPreviewWindow = null;
            }

            if(cornerPreviewWindow == null)
            { 
                cornerPreviewWindow = new CornerPreviewWindow(currentScreen);
                cornerPreviewWindow.Show();
            }
            else
                cornerPreviewWindow.ClearPreview();

            foreach (HotCorner item in currentScreen.Corners)
            {
                cornerPreviewWindow.AddPreview(item);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;

            screenSelect.SelectedIndex = 0;
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            HotCornerCreateWindow hke = new HotCornerCreateWindow(currentScreen.ScreenPosition, cornerPreviewWindow);

            if (hke.ShowDialog() == true)
                currentScreen.Corners.Add(hke.Corner);

            PrintCornerList();
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            if (cornerList.SelectedItem == null)
                return;

            HotCornerCreateWindow hke = new HotCornerCreateWindow(currentScreen.ScreenPosition, ((CornerListItem)cornerList.SelectedItem).originalItem, cornerPreviewWindow);

            hke.ShowDialog();

            PrintCornerList();
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            if (cornerList.SelectedItem == null)
                return;

            currentScreen.Corners.Remove(((CornerListItem)cornerList.SelectedItem).originalItem);

            PrintCornerList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (cornerPreviewWindow != null && cornerPreviewWindow.Visibility == Visibility.Visible)
                cornerPreviewWindow.Close();

            Screen.SaveScreens(App.Screens);
        }
    }

    public class CornerListItem
    {
        public string pos1 { get; set; }
        public string pos2 { get; set; }
        public string command { get; set; }

        public HotCorner originalItem { get; set; }
    }
}