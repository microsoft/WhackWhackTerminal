using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Workspace.VSIntegration.Contracts;
using StreamJsonRpc;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.Terminal.VsService
{
    internal sealed class EmbeddedTerminal : TerminalRenderer
    {
        private readonly EmbeddedTerminalOptions options;
        private readonly AsyncLazy<JsonRpc> rpc;
        private readonly AsyncLazy<SolutionUtils> solutionUtils;
        private bool isRpcDisconnected;

        public EmbeddedTerminal(TermWindowPackage package, TermWindowPane pane, EmbeddedTerminalOptions options, Task<Stream> rpcStreamTask) : base(package, pane)
        {
            this.options = options;
            this.rpc = new AsyncLazy<JsonRpc>(() => GetJsonRpcAsync(rpcStreamTask), package.JoinableTaskFactory);

            if (options.WorkingDirectory == null)
            {
                // Use solution directory
                this.solutionUtils = new AsyncLazy<SolutionUtils>(GetSolutionUtilsAsync, package.JoinableTaskFactory);

                // Start getting solution utils but don't block on the result.
                // Solution utils need MEF and sometimes it takes long time to initialize.
                this.solutionUtils.GetValueAsync().FileAndForget("WhackWhackTerminal/GetSolutionUtils");
            }
        }

        protected internal override string SolutionDirectory =>
            this.options.WorkingDirectory ?? this.package.JoinableTaskFactory.Run(() => GetSolutionDirectoryAsync(this.package.DisposalToken));

        internal protected override void OnTerminalResized(int cols, int rows)
        {
            base.OnTerminalResized(cols, rows);
            ResizeTermAsync(cols, rows).FileAndForget("WhackWhackTerminal/ResizePty");
        }

        internal protected override void OnTerminalDataRecieved(string data)
        {
            base.OnTerminalDataRecieved(data);
            SendTermDataAsync(data).FileAndForget("WhackWhackTerminal/TermData");
        }

        internal protected override void OnTerminalClosed()
        {
            base.OnTerminalClosed();
            CloseTermAsync().FileAndForget("WhackWhackTerminal/closeTerm");
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            CloseRpcAsync().FileAndForget("WhackWhackTerminal/closeRpc");
        }

        internal protected override void OnTerminalInit(object sender, TermInitEventArgs e)
        {
            base.OnTerminalInit(sender, e);
            InitTermAsync(e).FileAndForget("WhackWhackTerminal/InitPty");
        }

        private async Task<JsonRpc> GetJsonRpcAsync(Task<Stream> rpcStreamTask)
        {
            var stream = await rpcStreamTask;
            var target = new TerminalEvent(this.package, this);
            return JsonRpc.Attach(stream, target);
        }

        private async Task ResizeTermAsync(int cols, int rows)
        {
            var rpc = await this.rpc.GetValueAsync();
            if (!this.isRpcDisconnected)
            {
                await rpc.InvokeAsync("resizeTerm", cols, rows);
            }
        }

        private async Task SendTermDataAsync(string data)
        {
            var rpc = await this.rpc.GetValueAsync();
            if (!this.isRpcDisconnected)
            {
                await rpc.InvokeAsync("termData", data);
            }
        }

        private async Task CloseTermAsync()
        {
            var rpc = await this.rpc.GetValueAsync();
            if (!this.isRpcDisconnected)
            {
                await rpc.InvokeAsync("closeTerm");
            }
        }

        private async Task CloseRpcAsync()
        {
            var rpc = await this.rpc.GetValueAsync();
            rpc.Dispose();
            this.isRpcDisconnected = true;
        }

        private async Task InitTermAsync(TermInitEventArgs e)
        {
            var rpc = await this.rpc.GetValueAsync();
            if (!this.isRpcDisconnected)
            {
                var path = this.options.ShellPath ??
                    (this.package.OptionTerminal == DefaultTerminal.Other ? this.package.OptionShellPath : this.package.OptionTerminal.ToString());
                var args = ((object)this.options.Args) ?? this.package.OptionStartupArgument;
                await rpc.InvokeAsync("initTerm", path, e.Cols, e.Rows, e.Directory, args, this.options.Environment);
            }
        }

        private async Task<SolutionUtils> GetSolutionUtilsAsync()
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync(this.package.DisposalToken);
            var solutionService = (IVsSolution)await this.package.GetServiceAsync(typeof(SVsSolution));
            var componentModel = (IComponentModel)await this.package.GetServiceAsync(typeof(SComponentModel));
            var workspaceService = componentModel.GetService<IVsFolderWorkspaceService>();
            var result = new SolutionUtils(solutionService, workspaceService, this.package.JoinableTaskFactory);
            result.SolutionChanged += (sender, solutionDir) =>
            {
                if (package.OptionChangeDirectory)
                {
                    this.ChangeWorkingDirectoryAsync(solutionDir).FileAndForget("WhackWhackTerminal/changeWorkingDirectory");
                }
            };

            return result;
        }

        private async Task<string> GetSolutionDirectoryAsync(CancellationToken cancellationToken)
        {
            var solutionUtils = await this.solutionUtils.GetValueAsync(cancellationToken);
            return await solutionUtils.GetSolutionDirAsync(cancellationToken) ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        // RPC event target
        internal sealed class TerminalEvent
        {
            private readonly TermWindowPackage package;
            private readonly EmbeddedTerminal terminal;

            public TerminalEvent(TermWindowPackage package, EmbeddedTerminal terminal)
            {
                this.package = package;
                this.terminal = terminal;
            }

            [JsonRpcMethod("PtyData")]
            public Task PtyDataAsync(string data) =>
                this.terminal.PtyDataAsync(data, this.package.DisposalToken);

            [JsonRpcMethod("PtyExit")]
            public Task PtyExitAsync(int? code) =>
                this.terminal.PtyExitedAsync(code, this.package.DisposalToken);
        }
    }
}
