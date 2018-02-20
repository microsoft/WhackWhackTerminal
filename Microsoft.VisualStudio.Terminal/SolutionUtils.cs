using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Workspace;
using Microsoft.VisualStudio.Workspace.VSIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
namespace EmbeddedTerminal
{
    internal class SolutionUtils
    {
        private readonly IVsSolution solutionService;
        private readonly IVsWorkspaceFactory workspaceService;
        private readonly Dictionary<Action<string>, uint> cookieMap = new Dictionary<Action<string>, uint>();
        private readonly Dictionary<Action<string>, Func<object, EventArgs, Task>> lambdaMap = new Dictionary<Action<string>, Func<object, EventArgs, Task>>();

        public SolutionUtils(IVsSolution solutionService, IVsWorkspaceFactory workspaceService)
        {
            this.solutionService = solutionService;
            this.workspaceService = workspaceService;
        }

        public string GetSolutionDir()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.solutionService.GetSolutionInfo(out var solutionDir, out _, out _);

            // solution may sometimes be null in an open folder scenario.
            if (solutionDir == null)
            {
                return this.workspaceService.CurrentWorkspace?.Location;
            }

            return solutionDir;
        }

        public event Action<string> SolutionChanged
        {
            add
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                this.solutionService.AdviseSolutionEvents(new SolutionEvents(this.solutionService, value), out var cookie);

                Func<object, EventArgs, Task> adapterLambda = (sender, _) => 
                {
                    var thing = sender as IVsWorkspaceFactory;
                    if (thing?.CurrentWorkspace?.Location != null)
                    {
                        value(thing?.CurrentWorkspace.Location);
                    }

                    return Task.CompletedTask;
                };

                this.workspaceService.OnActiveWorkspaceChanged += adapterLambda;
                lambdaMap[value] = adapterLambda;
                cookieMap[value] = cookie;
            }
            remove
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                this.solutionService.UnadviseSolutionEvents(cookieMap[value]);
                cookieMap.Remove(value);
                lambdaMap.Remove(value);
            }
        }

        private class SolutionEvents : IVsSolutionEvents
        {
            private readonly IVsSolution solutionService;
            private readonly Action<string> handler;

            public SolutionEvents(IVsSolution solutionService, Action<string> handler)
            {
                this.solutionService = solutionService;
                this.handler = handler;
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
                this.handler(solutionDir);
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
