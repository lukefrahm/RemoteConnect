using System.Security;

namespace RemoteConnect
{
    internal sealed class TerminalSession : ConnectionProperties
    {
        private string TerminalCmd
        {
            get
            {
                return "plink -L " + Host + " " + Credentials.Username + " -pw " + Credentials.DecryptedPassword;
            }
        }

        internal TerminalSession(string host, string username, SecureString password, Protocol protocol, string overrideFileName = @"C:\Windows\System32\cmd") 
            : base(host, username, password, protocol, overrideFileName) { }

        internal override void Connect(bool autoDispose = false)
        {
            ConnectionProcess.Start();
            ConnectionProcess.StandardInput.WriteLine(TerminalCmd);
        }

        public override void Dispose()
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
}
