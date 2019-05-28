using Microsoft.ServiceHub.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Terminal.VsService;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
    [ProvideToolWindow(typeof(TermWindow), Style = VsDockStyle.Tabbed, Window = "34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3")]
    [ProvideToolWindow(typeof(RendererWindow), Transient = true, MultiInstances = true, Style = VsDockStyle.Tabbed, Window = TermWindow.ToolWindowGuid)]
    [ProvideOptionPage(typeof(TerminalOptionPage), "Whack Whack Terminal", "General", 0, 0, true)]
    [ProvideService(typeof(STerminalService), IsAsyncQueryable = true)]
    public sealed class TermWindowPackage : AsyncPackage
    {
        /// <summary>
        /// TermWindowPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "35b633bd-cdfb-4eda-8c26-f557e419eb8a";

        private IVsSettingsManager settingsManager;
        private int nextToolWindowId = 1;

        /// <summary> 
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            this.AddService(typeof(STerminalService), CreateServiceAsync, promote: true);

            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            this.settingsManager = (IVsSettingsManager)await this.GetServiceAsync(typeof(SVsSettingsManager));
            await TermWindowCommand.InitializeCommandAsync(this);
        }

        private Task<object> CreateServiceAsync(IAsyncServiceContainer container, CancellationToken cancellationToken, Type serviceType) =>
            Task.FromResult<object>(new EmbeddedTerminalService(this));

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType) =>
            toolWindowType == new Guid(TermWindow.ToolWindowGuid) || toolWindowType == new Guid(RendererWindow.ToolWindowGuid) ? this : null;

        protected override string GetToolWindowTitle(Type toolWindowType, int id) =>
            typeof(TermWindowPane).IsAssignableFrom(toolWindowType) ? "Terminal Window" : base.GetToolWindowTitle(toolWindowType, id);

        protected override Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken) =>
            Task.FromResult(typeof(TermWindowPane).IsAssignableFrom(toolWindowType) ? this : ToolWindowCreationContext.Unspecified);

        internal async Task<EmbeddedTerminal> CreateTerminalAsync(EmbeddedTerminalOptions options, TermWindowPane pane)
        {
            var rpcStreamTask = JoinableTaskFactory.RunAsync(async () =>
            {
                var client = new HubClient();
                return await client.RequestServiceAsync("wwt.pty", DisposalToken);
            });

            pane = pane ?? await CreateToolWindowAsync(options.Name);

            var result = new EmbeddedTerminal(this, pane, options, rpcStreamTask.Task);
            await result.InitAsync(DisposalToken);
            return result;
        }

        internal async Task<TerminalRenderer> CreateTerminalRendererAsync(string name)
        {
            var pane = await CreateToolWindowAsync(name);
            var result = new TerminalRenderer(this, pane);
            await result.InitAsync(DisposalToken);
            return result;
        }

        private async Task<RendererWindow> CreateToolWindowAsync(string name)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
            var pane = (RendererWindow)await FindToolWindowAsync(
                typeof(RendererWindow),
                nextToolWindowId++,
                create: true,
                cancellationToken: DisposalToken);
            pane.Caption = name;
            return pane;
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

        public string OptionShellPath
        {
            get
            {
                TerminalOptionsModel optionsModel = new TerminalOptionsModel(this.settingsManager);
                return optionsModel.ShellPath;
            }
        }
    }
}
