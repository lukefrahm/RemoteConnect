using System;

namespace RemoteConnect
{
    internal sealed class TerminalSession : Connection
    {
        #region Properties
        private string Executable
        {
            get
            {
                return Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmd.exe");
            }
        }
        private string CmdArgs
        {
            get
            {
                return "plink -L " + Host + " " + Credentials.Username + " -pw " + Credentials.DecryptedPassword;
            }
        }
        #endregion

        #region Constructors
        internal TerminalSession(string host, Credentials credentials, bool keepAlive, Protocol protocol = Protocol.RDP)
            : base(host, credentials, keepAlive, protocol)
        {
            ConnectionProcess.StartInfo.FileName = Executable;
            ConnectionProcess.StartInfo.Arguments = CmdArgs;
        }
        #endregion

        #region Override Methods
        internal override void Connect()
        {
            ConnectionProcess.Start();
            ConnectionProcess.StandardInput.WriteLine(CmdArgs);
        }

        public override void Dispose()
        {
            if (!KeepAlive)
            {
                ConnectionProcess.WaitForExit();
                ConnectionProcess.StandardInput.WriteLine("logout");

                if (ConnectionProcess.HasExited)
                {
                    ConnectionProcess.Close();
                    ConnectionProcess.Dispose();
                }
            }
        }
        #endregion
    }
}
