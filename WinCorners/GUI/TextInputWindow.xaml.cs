using System.Windows;
using System.Windows.Input;

namespace WinCorners
{
    /// <summary>
    /// Interaktionslogik für TextInputWindow.xaml
    /// </summary>
    public partial class TextInputWindow : Window
    {
        public TextInputWindow(string name)
        {
            InitializeComponent();

            this.Title = name;
        }

        public TextInputWindow(string name, string value)
        {
            InitializeComponent();

            this.Title = name;
            textBox.Text = value;
        }

        public string Text { get { return textBox.Text; } }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBox.Focus();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    this.DialogResult = true;
                    break;

                case Key.Escape:
                    this.DialogResult = false;
                    break;
            }
        }
    }
}