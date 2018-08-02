using Microsoft.VisualStudio.PlatformUI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.VisualStudio.Terminal
{
    internal static class TerminalThemer
    {
        private static readonly Dictionary<int, Color> DarkTheme = new Dictionary<int, Color>()
        {
            { 0,  new Color { R = 0, G = 0, B = 0} },
            { 1,  new Color { R = 0xcd, G = 0x31, B = 0x31 } },
            { 2,  new Color { R = 0x0d, G = 0xbc, B = 0x79 } },
            { 3,  new Color { R = 0xe5, G = 0xe5, B = 0x10 } },
            { 4,  new Color { R = 0x24, G = 0x72, B = 0xc8 } },
            { 5,  new Color { R = 0xbc, G = 0x3f, B = 0xbc } },
            { 6,  new Color { R = 0x11, G = 0xa8, B = 0xcd } },
            { 7,  new Color { R = 0xe5, G = 0xe5, B = 0xe5 } },
            { 8,  new Color { R = 0x66, G = 0x66, B = 0x66 } },
            { 9,  new Color { R = 0xf1, G = 0x4c, B = 0x4c } },
            { 10,  new Color { R = 0x23, G = 0xd1, B = 0x8b } },
            { 11,  new Color { R = 0xf5, G = 0xf5, B = 0x43 } },
            { 12,  new Color { R = 0x3b, G = 0x8e, B = 0xea } },
            { 13,  new Color { R = 0xd6, G = 0x70, B = 0xd6 } },
            { 14,  new Color { R = 0x29, G = 0xb8, B = 0xdb } },
            { 15,  new Color { R = 0xe5, G = 0xe5, B = 0xe5 } },
        };

        private static readonly Dictionary<int, Color> LightTheme = new Dictionary<int, Color>()
        {
            { 0,  new Color { R = 0, G = 0, B = 0} },
            { 1,  new Color { R = 0xcd, G = 0x31, B = 0x31 } },
            { 2,  new Color { R = 0x00, G = 0xbc, B = 0x00 } },
            { 3,  new Color { R = 0x94, G = 0x98, B = 0x00 } },
            { 4,  new Color { R = 0x04, G = 0x51, B = 0xa5 } },
            { 5,  new Color { R = 0xbc, G = 0x05, B = 0xbc } },
            { 6,  new Color { R = 0x05, G = 0x98, B = 0xbc } },
            { 7,  new Color { R = 0x55, G = 0x55, B = 0x55 } },
            { 8,  new Color { R = 0x66, G = 0x66, B = 0x66 } },
            { 9,  new Color { R = 0xcd, G = 0x31, B = 0x31 } },
            { 10,  new Color { R = 0x14, G = 0xce, B = 0x14 } },
            { 11,  new Color { R = 0xb5, G = 0xba, B = 0x00 } },
            { 12,  new Color { R = 0x04, G = 0x51, B = 0xa5 } },
            { 13,  new Color { R = 0xbc, G = 0x05, B = 0xbc } },
            { 14,  new Color { R = 0x05, G = 0x98, B = 0xbc } },
            { 15,  new Color { R = 0xa5, G = 0xa5, B = 0xa5 } },
        };

        public static Dictionary<int, Color> GetTheme()
        {
            if (VSColorTheme.GetThemedColor(EnvironmentColors.ToolboxBackgroundColorKey).GetBrightness() < 0.5)
            {
                return DarkTheme;
            }
            else
            {
                return LightTheme;
            }
        }
    }
}
