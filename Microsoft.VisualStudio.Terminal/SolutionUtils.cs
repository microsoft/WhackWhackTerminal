using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Workspace.VSIntegration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.Terminal
{
    internal class SolutionUtils
    {
        private readonly IVsSolution solutionService;
        private readonly IVsWorkspaceFactory workspaceService;
        private readonly JoinableTaskFactory jtf;

        public SolutionUtils(IVsSolution solutionService, IVsWorkspaceFactory workspaceService, JoinableTaskFactory jtf)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.solutionService = solutionService;
            this.workspaceService = workspaceService;
            this.jtf = jtf;

            var events = new SolutionEvents(solutionService, this);
            this.solutionService.AdviseSolutionEvents(events, out var cookie);
            events.Cookie = cookie;

            this.workspaceService.OnActiveWorkspaceChanged += WorkspaceChangedAsync;
        }

        public async Task<string> GetSolutionDirAsync(CancellationToken cancellationToken)
        {
            await this.jtf.SwitchToMainThreadAsync(cancellationToken);

            this.solutionService.GetSolutionInfo(out var solutionDir, out _, out _);

            // solution may sometimes be null in an open folder scenario.
            if (solutionDir == null)
            {
                return this.workspaceService.CurrentWorkspace?.Location;
            }

            return solutionDir;
        }

        public event EventHandler<string> SolutionChanged;

        private Task WorkspaceChangedAsync(object sender, EventArgs args)
        {
            var workspaceFactory = sender as IVsWorkspaceFactory;
            if (workspaceFactory?.CurrentWorkspace?.Location != null)
            {
                this.SolutionChanged?.Invoke(this, workspaceFactory?.CurrentWorkspace.Location);
            }

            return Task.CompletedTask;
        }

        private class SolutionEvents : IVsSolutionEvents
        {
            private readonly IVsSolution solutionService;
            private readonly SolutionUtils solutionUtils;

            public SolutionEvents(IVsSolution solutionService, SolutionUtils solutionUtils)
            {
                this.solutionService = solutionService;
                this.solutionUtils = solutionUtils;
            }

            public uint? Cookie
            {
                get;
                set;
            }

            public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                this.solutionService.GetSolutionInfo(out var solutionDir, out _, out _);
                this.solutionUtils.SolutionChanged?.Invoke(this.solutionUtils, solutionDir);
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            public int OnBeforeCloseSolution(object pUnkReserved)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }

            public int OnAfterCloseSolution(object pUnkReserved)
            {
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }
        }
    }
}
