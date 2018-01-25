using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using Task = System.Threading.Tasks.Task;

namespace EmbeddedTerminal
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(TermWindow), Window = "34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3")]
    [Guid(TermWindowPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(TerminalOptionPage), "Whack Whack Terminal", "General", 0, 0, true)]
    public sealed class TermWindowPackage : AsyncPackage
    {
        /// <summary>
        /// TermWindowPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "35b633bd-cdfb-4eda-8c26-f557e419eb8a";

        #region Package Members

        /// <summary> 
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);
            await TermWindowCommand.InitializeCommandAsync(this);
            TermWindowPackage.Instance = this;
        }

        public static TermWindowPackage Instance
        {
            get;
            private set;
        }

        public DefaultTerminal OptionTerminal
        {
            get
            {
                TerminalOptionsModel optionsModel = new TerminalOptionsModel(this);
                ThreadHelper.JoinableTaskFactory.Run(optionsModel.LoadDataAsync);
                return optionsModel.OptionTerminal;
            }
        }

        public string OptionCustomCSSPath
        {
            get
            {
                TerminalOptionsModel optionsModel = new TerminalOptionsModel(this);
                ThreadHelper.JoinableTaskFactory.Run(optionsModel.LoadDataAsync);
                return optionsModel.CustomCSSPath;
            }
        }

        public string OptionStartupArgument
        {
            get
            {
                TerminalOptionsModel optionsModel = new TerminalOptionsModel(this);
                ThreadHelper.JoinableTaskFactory.Run(optionsModel.LoadDataAsync);
                return optionsModel.StartupArgument;
            }
        }

        #endregion
    }
}
