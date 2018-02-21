using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Terminal.VsService;
using Microsoft.ServiceHub.Client;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Workspace.VSIntegration.Contracts;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.Terminal
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
    [Guid(TermWindowPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(TermWindow), Transient = true, MultiInstances = true, Style = VsDockStyle.Tabbed, Window = "34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3")]
    [ProvideOptionPage(typeof(TerminalOptionPage), "Whack Whack Terminal", "General", 0, 0, true)]
    [ProvideService(typeof(SEmbeddedTerminalService), IsAsyncQueryable = true)]
    public sealed class TermWindowPackage : AsyncPackage
    {
        /// <summary>
        /// TermWindowPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "35b633bd-cdfb-4eda-8c26-f557e419eb8a";

        private IVsSettingsManager settingsManager;

        /// <summary> 
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);
            this.AddService(typeof(SEmbeddedTerminalService), CreateServiceAsync, promote: true);

            await this.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.settingsManager = (IVsSettingsManager)await this.GetServiceAsync(typeof(SVsSettingsManager));

            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await TermWindowCommand.InitializeCommandAsync(this);
        }

        private Task<object> CreateServiceAsync(IAsyncServiceContainer container, CancellationToken cancellationToken, Type serviceType)
        {
            return Task.FromResult((object)new EmbeddedTerminalService(this));
        }

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType)
        {
            if (toolWindowType.Equals(new Guid(TermWindow.TermWindowGuidString)))
            {
                return this;
            }

            return null;
        }

        protected override string GetToolWindowTitle(Type toolWindowType, int id)
        {
            if (toolWindowType == typeof(TermWindow))
            {
                return "Terminal Window";
            }
            return base.GetToolWindowTitle(toolWindowType, id);
        }

        protected override async Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync();
            var solutionService = (IVsSolution)await this.GetServiceAsync(typeof(SVsSolution));
            var componentModel = (IComponentModel)await this.GetServiceAsync(typeof(SComponentModel));
            var workspaceService = componentModel.GetService<IVsFolderWorkspaceService>();
            var solutionUtils = new SolutionUtils(solutionService, workspaceService);

            var client = new HubClient();
            var clientStream = await client.RequestServiceAsync("wwt.pty");
            return new ToolWindowContext()
            {
                Package = this,
                ServiceHubStream = clientStream,
                SolutionUtils = solutionUtils,
            };
        }

        public DefaultTerminal OptionTerminal
        {
            get
            {
                TerminalOptionsModel optionsModel = new TerminalOptionsModel(this.settingsManager);
                return optionsModel.OptionTerminal;
            }
        }

        public string OptionStartupArgument
        {
            get
            {
                TerminalOptionsModel optionsModel = new TerminalOptionsModel(this.settingsManager);
                return optionsModel.StartupArgument;
            }
        }

        public bool OptionChangeDirectory
        {
            get
            {
                TerminalOptionsModel optionsModel = new TerminalOptionsModel(this.settingsManager);
                return optionsModel.ChangeDirectory;
            }
        }

        public string OptionFontFamily
        {
            get
            {
                TerminalOptionsModel optionsModel = new TerminalOptionsModel(this.settingsManager);
                return optionsModel.FontFamily;
            }
        }

        public int OptionFontSize
        {
            get
            {
                TerminalOptionsModel optionsModel = new TerminalOptionsModel(this.settingsManager);
                return optionsModel.FontSize;
            }
        }
    }
}
