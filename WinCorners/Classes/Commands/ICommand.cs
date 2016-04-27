namespace WinCorners
{
    public interface ICommand
    {
        bool Run();

        string ToSaveString();

        void FromSaveString(string s);

        bool ShowSettingsWindow(bool isEdit);

        string GetCommandName();

        string GetCommandValues();
    }
}