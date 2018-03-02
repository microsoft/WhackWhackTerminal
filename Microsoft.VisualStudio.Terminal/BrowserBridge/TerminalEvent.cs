namespace Microsoft.VisualStudio.Terminal
{
    using Microsoft.VisualStudio.PlatformUI;
    using Microsoft.VisualStudio.ScriptedHost.Messaging;
    using StreamJsonRpc;
    using System;
    using System.Threading.Tasks;

    internal class TerminalEvent: JsonPortMarshaler
    {
        private readonly TermWindowPackage package;
        private EventHandler<DataEventArgs> ptyDataEventHandler;
        private EventHandler<string> ptyExitedEventHandler;

        public TerminalEvent(TermWindowPackage package)
        {
            this.package = package;
        }

        [JsonRpcMethod("PtyData")]
        public void PtyData(string data)
        {
            this.ptyDataEventHandler?.Invoke(this, new DataEventArgs(data));
        }

        [JsonRpcMethod("PtyExit")]
        public void PtyExit(int? code)
        {
            this.ptyExitedEventHandler?.Invoke(this, "");
        }

        protected override void InitializeMarshaler()
        {
            this.ptyDataEventHandler = this.RegisterEvent<DataEventArgs>("ptyData");
            this.ptyExitedEventHandler = this.RegisterEvent<string>("ptyExited");
        }
    }

    public class DataEventArgs: EventArgs
    {
        public string Value { get; set; }

        public DataEventArgs(string value)
        {
            Value = value;
        }
    }
}