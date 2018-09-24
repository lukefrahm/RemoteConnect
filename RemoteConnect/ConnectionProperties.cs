using System;
using System.Diagnostics;
using System.Security;

namespace RemoteConnect
{
    internal enum Protocol
    {
        RDP,
        SSH,
        COM,
        VNC
    }

    internal abstract class ConnectionProperties : IDisposable
    {
        private protected string Host { get; set; }
        private protected CredentialSet Credentials { get; set; }
        private protected Process ConnectionProcess { get; set; }
        private protected Protocol Protocol { get; set; }

        internal ConnectionProperties(string host, CredentialSet credentials, Protocol protocol, string fileName)
        {
            Construct(host, credentials, protocol, fileName);
        }
        internal ConnectionProperties(string host, string username, SecureString password, Protocol protocol, string fileName)
        {
            Construct(host, new CredentialSet(username, password), protocol, fileName);
        }

        private protected void Construct(string host, CredentialSet credentials, Protocol protocol, string fileName)
        {
            Host = host;
            Credentials = credentials;
            Protocol = protocol;

            ConnectionProcess = new Process
            {
                StartInfo =
                {
                    FileName = fileName,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = false,
                    CreateNoWindow = false
                }
            };

            ConnectionProcess = new Process
            {
                StartInfo =
                {
                    FileName = Environment.ExpandEnvironmentVariables(fileName),
                    Arguments = $@"/list",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
        }
        
        internal abstract void Connect(bool autoDispose = false);
        
        public abstract void Dispose();

        internal static object CreateDynamic(string host, string username, SecureString password, string protocol, string overrideFileName = null)
        {
            if (overrideFileName == null)
            {
                switch (protocol)
                {
                    case "SSH":
                        return new TerminalSession(host, username, password, Protocol.SSH);
                    case "COM":
                        return new TerminalSession(host, username, password, Protocol.COM);
                    case "VNC":
                        return new VncSession(host, username, password);
                    case "RDP":
                    default:
                        return new RdpSession(host, username, password);
                }
            }
            else
            {
                switch (protocol)
                {
                    case "SSH":
                        return new TerminalSession(host, username, password, Protocol.SSH, overrideFileName);
                    case "COM":
                        return new TerminalSession(host, username, password, Protocol.COM, overrideFileName);
                    case "VNC":
                        return new VncSession(host, username, password, overrideFileName);
                    case "RDP":
                    default:
                        return new RdpSession(host, username, password, overrideFileName);
                }
            }
        }
    }
}
