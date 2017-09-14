using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using mshtml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmbeddedTerminal
{

    /// <summary>
    /// Interaction logic for BetterBrowser.xaml
    /// </summary>
    public class BetterBrowser : ContentControl
    {
        [DllImport("urlmon.dll")]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        static extern int CoInternetSetFeatureEnabled(int FeatureEntry, uint dwFlags, [MarshalAs(UnmanagedType.Bool)]bool fEnable);

        public static readonly DependencyProperty CustomCSSProperty = DependencyProperty.Register(
      "CustomCSS", typeof(IList), typeof(BetterBrowser), new PropertyMetadata(new List<CustomCSSRule>()));

        public IList CustomCSS
        {
            get { return (IList)GetValue(CustomCSSProperty); }
            set { SetValue(CustomCSSProperty, value); }
        }

        public object ScriptingObject
        {
            get { return this.browser.ObjectForScripting; }
            set { this.browser.ObjectForScripting = value; }
        }


        private System.Windows.Forms.WebBrowser browser;
        private IHTMLStyleSheet styleSheet;

        public BetterBrowser()
        {
            this.CustomCSS = new List<CustomCSSRule>();

            //TODO: Investigate if there is a way to turn off this feature for only our browser instance
            CoInternetSetFeatureEnabled(8, 0x00000002, false);

            System.Windows.Forms.Integration.WindowsFormsHost host =
        new System.Windows.Forms.Integration.WindowsFormsHost();

            this.browser = new System.Windows.Forms.WebBrowser();
            browser.WebBrowserShortcutsEnabled = true;
            browser.PreviewKeyDown += Browser_PreviewKeyDown;
            this.browser.DocumentCompleted += Browser_DocumentCompleted;
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
            this.browser.IsWebBrowserContextMenuEnabled = false;
            this.browser.ScrollBarsEnabled = false;
            host.Child = browser;
            this.Content = host;
        }

        private void Browser_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            // Forces the terminal to capture the shortcut instead of sending it to VS
            if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.P || e.KeyCode == System.Windows.Forms.Keys.O || e.KeyCode == System.Windows.Forms.Keys.Tab))
            {
                e.IsInputKey = true;
            }
            else
            {
                e.IsInputKey = false;
            }
        }

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            if (this.browser.ReadyState == System.Windows.Forms.WebBrowserReadyState.Complete)
            {
                this.ThemeStyleSheet();
            }
        }

        public void Navigate(FileInfo file)
        {
            var uri = new Uri(file.FullName);
            this.Navigate(uri);
        }

        public void Navigate(Uri uri)
        {
            this.browser.Navigate(uri);
        }

        public object Invoke(string scriptName, params object[] args)
        {
            return this.browser.Document.InvokeScript(scriptName, args);
        }

        private void Browser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            var doc = (IHTMLDocument2)browser.Document.DomDocument;

            this.styleSheet = doc.createStyleSheet("themesheet.css");
            doc.createStyleSheet(TermWindowPackage.Instance?.OptionCustomCSSPath??"");
            this.ThemeStyleSheet();
        }

        private void ThemeStyleSheet()
        {
            this.styleSheet.cssText = "";
            // First we add the default rules
            this.styleSheet.addRule("body", CompileDefaultRules());

            foreach (CustomCSSRule item in CustomCSS)
            {
                string declarationSegment = String.Empty;
                foreach (CSSDeclaration declaration in item.Declarations)
                {
                    declarationSegment += declaration.ToString();
                }
                this.styleSheet.addRule(item.Selector, declarationSegment);
            }
        }

        private string CompileDefaultRules()
        {
            var declarationSegment = "";
            declarationSegment += "background-color: " + ColorTranslator.ToHtml(VSColorTheme.GetThemedColor(EnvironmentColors.ToolboxBackgroundColorKey)) + ";";
            declarationSegment += "scrollbar-base-color: " + ColorTranslator.ToHtml(VSColorTheme.GetThemedColor(EnvironmentColors.ScrollBarThumbBackgroundColorKey)) + ";";
            declarationSegment += "scrollbar-arrow-color: " + ColorTranslator.ToHtml(VSColorTheme.GetThemedColor(EnvironmentColors.ScrollBarArrowGlyphColorKey)) + ";";
            declarationSegment += "scrollbar-track-color: " + ColorTranslator.ToHtml(VSColorTheme.GetThemedColor(EnvironmentColors.ScrollBarArrowBackgroundColorKey)) + ";";
            declarationSegment += "scrollbar-face-color: " + ColorTranslator.ToHtml(VSColorTheme.GetThemedColor(EnvironmentColors.ScrollBarThumbBackgroundColorKey)) + ";";
            return declarationSegment;
        }

        
    }

    public class CustomCSSRule : DependencyObject
    {
        public string Selector
        {
            get;
            set;
        }

        public static readonly DependencyProperty DeclarationsProperty = DependencyProperty.Register(
      "Declarations", typeof(IList), typeof(CustomCSSRule), new PropertyMetadata(new List<CSSDeclaration>()));
        public IList Declarations
        {
            get { return (IList)GetValue(DeclarationsProperty); }
            set { SetValue(DeclarationsProperty, value); }
        }

        public CustomCSSRule()
        {
            this.Declarations = new List<CSSDeclaration>();
        }
    }

    public class CSSDeclaration: DependencyObject
    {
        public CSSProperty Attribute
        {
            get;
            set;
        }

        public ThemeResourceKey Value
        {
            get;
            set;
        }

        public override string ToString()
        {
            var color = ColorTranslator.ToHtml(VSColorTheme.GetThemedColor(this.Value));
            string property;
            switch (this.Attribute)
            {
                case CSSProperty.Color:
                    property = "color";
                    break;
                case CSSProperty.BackgroundColor:
                    property = "background-color";
                    break;
                case CSSProperty.OutlineColor:
                    property = "outline-color";
                    break;
                default:
                    return "";
            }

            return property + ": " + color + ";";

        }

        public enum CSSProperty
        {
            Color,
            BackgroundColor,
            OutlineColor
        }
    }
}