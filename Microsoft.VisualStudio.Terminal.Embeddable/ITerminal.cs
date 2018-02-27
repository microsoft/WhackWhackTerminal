﻿using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Terminal
{
    [ComImport, Guid("E195D61C-2821-49F1-BE0E-B2CD82F1F856")]
    public interface ITerminal
    {
        Task ShowAsync();
        Task HideAsync();
        Task CloseAsync();
        Task ChangeWorkingDirectoryAsync(string newDirectory);

        event EventHandler Closed;
    }
}