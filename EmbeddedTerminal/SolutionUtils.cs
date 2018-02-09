using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
        private readonly Dictionary<Action<string>, uint> cookieMap = new Dictionary<Action<string>, uint>();

        public SolutionUtils(IVsSolution solutionService)
        {
            this.solutionService = solutionService;
        }

        public string GetSolutionDir()
        {
            this.solutionService.GetSolutionInfo(out var solutionDir, out _, out _);

            return solutionDir;
        }

        public event Action<string> SolutionChanged
        {
            add
            {
                this.solutionService.AdviseSolutionEvents(new SolutionEvents(this.solutionService, value), out var cookie);

                cookieMap[value] = cookie;
            }
            remove
            {
                this.solutionService.UnadviseSolutionEvents(cookieMap[value]);
                cookieMap.Remove(value);
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
                this.solutionService.GetSolutionInfo(out var solutionDir, out _, out _);
                this.handler(solutionDir);
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
