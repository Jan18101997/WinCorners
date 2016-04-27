using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WinCorners
{
    public partial class HotCornerCreateWindow : Window
    {
        public HotCornerCreateWindow(ScreenPosition pos, CornerPreviewWindow previewWindow)
        {
            InitializeComponent();

            Corner = new HotCorner();
            Corner.Position1 = new Point(pos.Left, pos.Top);
            Corner.Position2 = new Point(pos.Left, pos.Top);

            PreviewWindow = previewWindow;

            SetnumericMaxMin(pos);

            PrintCommadSelect();
        }

        public HotCornerCreateWindow(ScreenPosition pos, HotCorner corner, CornerPreviewWindow previewWindow)
        {
            InitializeComponent();

            cancel.Visibility = Visibility.Hidden;

            Corner = corner;
            runOnce.IsChecked = Corner.RunOnce;
            disableAtMouseDown.IsChecked = Corner.DisableAtMouseDown;
            PreviewWindow = previewWindow;

            SetnumericMaxMin(pos);

            PreviewWindow.RemovePreview(corner);

            PrintCommadSelect();
        }

        public HotCorner Corner { get; private set; }

        private CornerPreviewWindow PreviewWindow;

        private void SetnumericMaxMin(ScreenPosition pos)
        {
            pos1X.Minimum = pos.Left;
            pos1X.Maximum = pos.Right;

            pos1Y.Minimum = pos.Top;
            pos1Y.Maximum = pos.Bottom;

            pos1X.Value = Corner.Position1.X;
            pos1Y.Value = Corner.Position1.Y;

            pos2X.Minimum = Corner.Position1.X;
            pos2X.Maximum = pos.Right;

            pos2Y.Minimum = Corner.Position1.Y;
            pos2Y.Maximum = pos.Bottom;

            pos2X.Value = Corner.Position2.X;
            pos2Y.Value = Corner.Position2.Y;
        }

        private void PrintCommadSelect()
        {
            bool itemSelected = false;

            foreach (Type item in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (item.GetInterfaces().Contains(typeof(ICommand)) && item.GetConstructor(Type.EmptyTypes) != null)
                {
                    ICommand commandItem = (ICommand)Activator.CreateInstance(item);

                    ComboBoxItem cbi = new ComboBoxItem();
                    cbi.Content = commandItem.GetCommandName();
                    cbi.Tag = commandItem.GetType();

                    commandSelector.Items.Add(cbi);

                    if (Corner.Command != null && Corner.Command.GetType() == item)
                    {
                        commandSelector.SelectedItem = cbi;

                        itemSelected = true;
                    }
                }
            }

            if (itemSelected == false)
                commandSelector.SelectedIndex = 0;
        }

        private void PrintCommandStatus()
        {
            if (Corner.Command == null)
                return;

            commandStatus.Content = Corner.Command.GetCommandValues();

            PreviewWindow.RemovePreview(Corner);
            PreviewWindow.AddPreview(Corner);
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            if (Corner.Command == null)
            {
                if (MessageBox.Show("Your command is not set! Do you want to ignore all changes?", "Null", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    this.DialogResult = false;
            }
            else
                this.DialogResult = true;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void commadSettings_Click(object sender, RoutedEventArgs e)
        {
            if (commandSelector.SelectedIndex == -1)
                return;

            ComboBoxItem cbi = (ComboBoxItem)commandSelector.SelectedItem;

            if (Corner.Command == null || (Type)cbi.Tag != Corner.Command.GetType())
            {
                Corner.Command = (ICommand)Activator.CreateInstance((Type)cbi.Tag);

                Corner.Command.ShowSettingsWindow(false);
            }
            else
                Corner.Command.ShowSettingsWindow(true);

            PrintCommandStatus();
        }

        private void runOnce_CheckedChange(object sender, RoutedEventArgs e)
        {
            Corner.RunOnce = (bool)runOnce.IsChecked;
        }

        private void disableAtMouseDown_CheckedChange(object sender, RoutedEventArgs e)
        {
            Corner.DisableAtMouseDown = (bool)disableAtMouseDown.IsChecked;
        }

        private void commandSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PrintCommandStatus();
        }

        private void pos1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (pos1X.Value == null || pos1Y.Value == null)
                return;

            Corner.Position1 = new Point((double)pos1X.Value, (double)pos1Y.Value);

            pos2X.Minimum = (double)pos1X.Value;
            pos2Y.Minimum = (double)pos1Y.Value;

            if (pos2X.Minimum > pos2X.Value)
                pos2X.Value = pos2X.Minimum;

            if (pos2Y.Minimum > pos2Y.Value)
                pos2Y.Value = pos2Y.Minimum;

            PreviewWindow.RemovePreview(Corner);
            PreviewWindow.AddPreview(Corner);
        }

        private void pos2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (pos2X.Value == null || pos2Y.Value == null)
                return;

            Corner.Position2 = new Point((double)pos2X.Value, (double)pos2Y.Value);

            PreviewWindow.RemovePreview(Corner);
            PreviewWindow.AddPreview(Corner);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    ok_Click(this, new RoutedEventArgs());
                    break;

                case Key.Escape:
                    this.DialogResult = false;
                    break;
            }
        }
    }
}