using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using WindowsInput.Native;

namespace WinCorners
{
    /// <summary>
    /// Interaktionslogik für HotKeyInputWindow.xaml
    /// </summary>
    public partial class HotKeyInputWindow : Window
    {
        public HotKeyInputWindow()
        {
            InitializeComponent();

            PrintStatus();
        }

        private List<Key> Keys = new List<Key>();

        public List<VirtualKeyCode> VirtualKeys
        {
            get
            {
                List<VirtualKeyCode> ret = new List<VirtualKeyCode>();

                foreach (Key item in Keys)
                {
                    ret.Add(item.ToVirtualKeyCode());
                }

                return ret;
            }
        }

        private void PrintStatus()
        {
            statusText.Text = "ESC - Cancel, Enter - OK, Back - Clear" + Environment.NewLine;

            foreach (Key item in Keys)
            {
                statusText.Text += item.ToString();

                if (item != Keys[Keys.Count - 1])
                    statusText.Text += "+";
            }
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    this.DialogResult = true;
                    break;

                case Key.Back:
                    Keys.Clear();
                    PrintStatus();
                    break;

                case Key.Escape:
                    this.DialogResult = false;
                    break;

                default:
                    if (Keys.Contains(e.Key) == false)
                    {
                        Keys.Add(e.Key);

                        Keys.Sort();
                        Keys.Reverse();

                        PrintStatus();
                    }
                    break;
            }
        }
    }
}