using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RemoteConnect
{
    internal sealed class RdpSession : Connection
    {
        #region Properties
        private string Executable
        {
            get
            {
                return Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
            }
        }
        private string CmdArgs
        {
            get
            {
                return $@"/generic:TERMSRV/{Host} /user:{Credentials.Username} /pass:{Credentials.DecryptedPassword}";
            }
        }
        #endregion

        #region Constructors
        internal RdpSession(string host, Credentials credentials, bool keepAlive, Protocol protocol = Protocol.RDP)
            : base(host, credentials, keepAlive, protocol)
        {
            ConnectionProcess.StartInfo.FileName = Executable;
            ConnectionProcess.StartInfo.Arguments = CmdArgs;
            
            Host = host;
        }
        #endregion

        #region Override Methods
        internal override void Connect()
        {
            ConnectionProcess.Start();
            ConnectionProcess.WaitForExit();
            if (!ConnectionProcess.StandardOutput.ReadToEnd().Contains($@"TERMSRV/{Host}"))
            {
                ConnectionProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = Executable,
                        Arguments = CmdArgs,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                ConnectionProcess.Start();
            }
        }

        public override void Dispose() 
        {
            if (Host != null)
            {
                Process cmdkey = new Process
                {
                    StartInfo =
                    {
                        FileName = Executable,
                        Arguments = $@"/delete:TERMSRV/{Host}",
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                cmdkey.Start();
                cmdkey.Dispose();
            }

            if (!KeepAlive)
            {
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
