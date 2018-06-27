using Microsoft.VisualStudio.PlatformUI;
using Newtonsoft.Json;
using System.Drawing;

namespace Microsoft.VisualStudio.Terminal
{
    internal static class TerminalThemer
    {
        private static readonly TerminalTheme DarkTheme = new TerminalTheme()
        {
            Cursor = "#ffffff",
            Black = "#000000",
            Red = "#cd3131",
            Green = "#0dbc79",
            Yellow = "#e5e510",
            Blue = "#2472c8",
            Magenta = "#bc3fbc",
            Cyan = "#11a8cd",
            White = "#e5e5e5",
            BrightBlack = "#666666",
            BrightRed = "#f14c4c",
            BrightGreen = "#23d18b",
            BrightYellow = "#f5f543",
            BrightBlue = "#3b8eea",
            BrightMagenta = "#d670d6",
            BrightCyan = "#29b8db",
            BrightWhite = "#e5e5e5",
        };

        private static readonly TerminalTheme LightTheme = new TerminalTheme()
        {
            Cursor = "#ffffff",
            Black = "#000000",
            Red = "#cd3131",
            Green = "#00BC00",
            Yellow = "#949800",
            Blue = "#0451a5",
            Magenta = "#bc05bc",
            Cyan = "#0598bc",
            White = "#555555",
            BrightBlack = "#666666",
            BrightRed = "#cd3131",
            BrightGreen = "#14CE14",
            BrightYellow = "#b5ba00",
            BrightBlue = "#0451a5",
            BrightMagenta = "#bc05bc",
            BrightCyan = "#0598bc",
            BrightWhite = "#a5a5a5",
        };

        public static string GetTheme()
        {
            TerminalTheme theme;
            if (VSColorTheme.GetThemedColor(EnvironmentColors.ToolboxBackgroundColorKey).GetBrightness() < 0.5)
            {
                theme = DarkTheme;
            }
            else
            {
                theme = LightTheme;
            }

            theme.Background = ColorTranslator.ToHtml(VSColorTheme.GetThemedColor(EnvironmentColors.ToolboxBackgroundColorKey));
            theme.Foreground = ColorTranslator.ToHtml(VSColorTheme.GetThemedColor(EnvironmentColors.ToolboxContentTextColorKey));
            theme.Cursor = ColorTranslator.ToHtml(VSColorTheme.GetThemedColor(EnvironmentColors.ToolboxContentTextColorKey));
            theme.Border = ColorTranslator.ToHtml(VSColorTheme.GetThemedColor(EnvironmentColors.ToolboxDisabledContentTextColorKey));
            return JsonConvert.SerializeObject(theme);
        }
    }
}
