namespace Microsoft.VisualStudio.Terminal
{
    internal interface ITerminalScriptingObject
    {
        string GetTheme();
        string GetFontFamily();
        int GetFontSize();
        string GetSolutionDir();
        void InitPty(int cols, int rows, string directory);
        void ClosePty();
        void CopyStringToClipboard(string stringToCopy);
        string GetClipboard();
        void TermData(string data);
        void ResizePty(int cols, int rows);
        string GetLinkRegex();
        void HandleLocalLink(string uri);
        bool ValidateLocalLink(string link);
    }
}