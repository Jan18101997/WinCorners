using System;
using System.Windows;

namespace WinCorners
{
    /// <summary>
    /// Interaktionslogik für ShowPositionWindow.xaml
    /// </summary>
    public partial class ShowPositionWindow : Window
    {
        public ShowPositionWindow()
        {
            InitializeComponent();
        }

        public Point Position
        {
            set
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    posLabel.Content = "X: " + value.X + " Y:" + value.Y;
                }));
            }
        }
    }
}