using System;
using System.Diagnostics;

namespace RemoteConnect
{
    internal enum Protocol
    {
        RDP,
        SSH,
        COM,
        VNC
    }

    internal abstract class Connection : IDisposable
    {
        #region Properties
        private protected string Host { get; set; }
        private protected Credentials Credentials { get; set; }
        private protected Process ConnectionProcess { get; set; }
        private protected Protocol Protocol { get; set; }
        private protected bool KeepAlive { get; set; }
        #endregion

        #region Constructors
        internal Connection(bool keepAlive)
        {
            KeepAlive = keepAlive;
        }

        internal Connection(string host, Credentials credentials, bool keepAlive, Protocol protocol)
        {
            Host = host;
            Credentials = credentials;
            Protocol = protocol;
            KeepAlive = keepAlive;

            ConnectionProcess = new Process
            {
                StartInfo =
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = false
                }
            };
        }
        #endregion

        #region Instance Methods
        internal abstract void Connect();

        public virtual void Dispose()
        {
            if (Credentials.Password != null)
            {
                Credentials.Password.Dispose();
            }
        }
        #endregion

        #region Static Methods
        internal static dynamic CreateDynamic(string host, Credentials credentials, string protocol, bool keepAlive)
        {
            switch (protocol)
            {
                case "SSH":
                    return new TerminalSession(host, credentials, keepAlive, Protocol.SSH);
                case "COM":
                    return new TerminalSession(host, credentials, keepAlive, Protocol.COM);
                case "VNC":
                    return new VncSession(host, credentials, keepAlive);
                case "RDP":
                default:
                    return new RdpSession(host, credentials, keepAlive);
            }
        }
        #endregion
    }
}
