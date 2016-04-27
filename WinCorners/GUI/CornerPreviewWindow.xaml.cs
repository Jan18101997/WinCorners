using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WinCorners
{
    /// <summary>
    /// Interaktionslogik für CornerPreviewWindow.xaml
    /// </summary>
    public partial class CornerPreviewWindow : Window
    {
        public CornerPreviewWindow(Screen screen)
        {
            InitializeComponent();

            this.Left = screen.ScreenPosition.Left;
            this.Top = screen.ScreenPosition.Top;

            this.Height = screen.ScreenWidth;
            this.Width = screen.ScreenHeight;

            Screen = screen;
        }

        public Screen Screen;

        public void AddPreview(HotCorner corner)
        {
            Border border = new Border();
            TextBlock tBlok = new TextBlock();
            tBlok.VerticalAlignment = VerticalAlignment.Center;
            tBlok.TextAlignment = TextAlignment.Center;
            if(corner.Command != null)
                tBlok.Text = corner.Command.GetCommandName() + Environment.NewLine + corner.Command.GetCommandValues();

            border.Child = tBlok;

            double width = (corner.Position2.X - Screen.ScreenPosition.Left) - (corner.Position1.X - Screen.ScreenPosition.Left);
            double height = (corner.Position2.Y - Screen.ScreenPosition.Top) - (corner.Position1.Y - Screen.ScreenPosition.Top);

            if (width < 0)
                width = width * -1;

            if (height < 0)
                height = height * -1;

            border.Width = width;
            border.Height = height;
            border.Background = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
            border.BorderBrush = Brushes.DarkBlue;
            border.BorderThickness = new Thickness(1);
            border.Tag = corner;

            if (border.Width < 1)
                border.Width = 1;

            if (border.Height < 1)
                border.Height = 1;

            body.Children.Add(border);
            Canvas.SetLeft(border, corner.Position1.X - Screen.ScreenPosition.Left);
            Canvas.SetTop(border, corner.Position1.Y - Screen.ScreenPosition.Top);
        }

        public void ClearPreview()
        {
            body.Children.Clear();
        }

        public void RemovePreview(HotCorner corner)
        {
            foreach (FrameworkElement item in body.Children)
            {
                if (item.Tag == corner)
                {
                    body.Children.Remove(item);
                    break;
                }
            }
        }
    }
}