using Microsoft.VisualStudio.PlatformUI;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VtNetCore.XTermParser;
using VTSharp;
using VTSharp.Parsers;
using static System.FormattableString;

namespace Microsoft.VisualStudio.Terminal
{
    /// <summary>
    /// Interaction logic for ServiceToolWindowControl.
    /// </summary>
    public partial class TermWindowControl : UserControl, IDisposable
    {
        private readonly TermWindowPackage package;
        private bool pendingFocus;
        private TerminalScriptingObject scriptingObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceToolWindowControl"/> class.
        /// </summary>
        public TermWindowControl(TermWindowPackage package)
        {
            this.package = package;

            this.InitializeComponent();

            this.Focusable = true;
            this.GotFocus += TermWindowControl_GotFocus;
            this.LostFocus += TermWindowControl_LostFocus;
        }

        private void AnsiTerminal_RendererLoaded(object sender, (int row, int col) dimensions)
        {
            var args = new TermInitEventArgs(dimensions.col, dimensions.row, scriptingObject.GetSolutionDir());
            //this.ansiTerminal.SetPalette(TerminalThemer.GetTheme());
            this.TermInit?.Invoke(args);
        }

        private void AnsiTerminal_UserInput(object sender, string input)
        {
            this.TermData?.Invoke(this, input);
        }

        private void AnsiTerminal_TerminalResized(object sender, (int row, int col) dimensions)
        {
            this.TermResize?.Invoke(this, dimensions);
        }

        internal event Action<object, string> TermData;
        internal event Action<object, (int row, int col)> TermResize;
        internal event Action<TermInitEventArgs> TermInit;

        internal void PtyData(string data)
        {
            (this.Content as AnsiTerminal)?.ConsumePtyData(data);
        }

        internal async Task InitAsync(TerminalScriptingObject scriptingObject, CancellationToken cancellationToken)
        {
            this.scriptingObject = scriptingObject;
            var term =  new AnsiTerminal();
            term.RendererLoaded += AnsiTerminal_RendererLoaded;
            term.TerminalResized += AnsiTerminal_TerminalResized;
            term.UserInput += AnsiTerminal_UserInput;
            this.Content = term;
        }

        internal void Resize(int cols, int rows) { }

        internal void ChangeWorkingDirectory(string newDirectory) {}

        public void Dispose()
        {

        }

        internal void PtyExited(int? code) { }

        private void TermWindowControl_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TermWindowControl_LostFocus(object sender, RoutedEventArgs e) =>
            this.pendingFocus = false;

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            //this.ansiTerminal.SetPalette(TerminalThemer.GetTheme());
        }

        private string ConvertToString(KeyEventArgs e)
        {
            string key = null;
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        key = "\x1b";
                    }
                    break;

                case Key.C:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        key = "\x03";
                    }
                    break;

                case Key.Tab:
                    {
                        key = "\t";
                    }
                    break;
                case Key.Up:
                    {
                        key = "\x1b[A";
                    }
                    break;
                case Key.Down:
                    {
                        key = "\x1b[B";
                    }
                    break;
                case Key.Left:
                    {
                        key = "\x1b[D";
                    }
                    break;
                case Key.Right:
                    {
                        key = "\x1b[C";
                    }
                    break;
                case Key.Enter:
                    if (Keyboard.Modifiers == 0)
                    {
                        key = "\u000d";
                    }
                    break;
                case Key.Back:
                    {
                        key = "\x7f";
                    }
                    break;
                case Key.Space:
                    {
                        key = " ";
                    }
                    break;
                case Key.OemPeriod:
                    {
                        key = ".";
                    }
                    break;
                case Key.System:
                    {
                        key = "";
                    }
                    break;
                default:
                    {
                        KeyConverter kc = new KeyConverter();
                        key = kc.ConvertToString(e.Key);
                    }
                    break;
            }

            return key;
        }
    }
}