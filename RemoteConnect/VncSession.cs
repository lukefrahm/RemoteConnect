using System;

namespace RemoteConnect
{
    internal sealed class VncSession : Connection
    {
        #region Properties
        private string Executable
        {
            get
            {
                return Environment.ExpandEnvironmentVariables(@"");
            }
        }
        private string CmdArgs
        {
            get
            {
                return "";
            }
        }
        #endregion

        #region Constructors
        internal VncSession(string host, Credentials credentials, bool keepAlive, Protocol protocol = Protocol.RDP)
            : base(host, credentials, keepAlive, protocol)
        {
            ConnectionProcess.StartInfo.FileName = Executable;
            ConnectionProcess.StartInfo.Arguments = CmdArgs;
        }
        #endregion

        #region Override Methods
        internal override void Connect()
        {
            
        }

        //public override void Dispose() { }
        #endregion
    }
}
