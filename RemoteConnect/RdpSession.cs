using System;
using System.Diagnostics;
using System.Security;
using System.Text.RegularExpressions;

namespace RemoteConnect
{
    internal sealed class RdpSession : ConnectionProperties
    {
        internal RdpSession(string host, string username, SecureString password, string overrideFileName = @"%SystemRoot%\system32\cmdkey.exe")
            : base(host, username, password, Protocol.RDP, overrideFileName)
        {
            if (!host.EndsWith(".LUKE.int") && !Regex.IsMatch(host, @"[\d\d?\d?.]{3}\d\d?\d?"))
            {
                if (host.EndsWith(".LUKE"))
                {
                    host += ".int";
                }
                else if (host.Contains("."))
                {
                    throw new Exception("Invalid domain entered or host name is malformed.");
                }
                else
                {
                    host += ".LUKE.int";
                }
            }
            Host = host;
        }

        internal override void Connect(bool autoDispose = false)
        {
            ConnectionProcess.Start();
            ConnectionProcess.WaitForExit();
            if (!ConnectionProcess.StandardOutput.ReadToEnd().Contains($@"TERMSRV/{Host}"))
            {
                this.Host = Host;
                ConnectionProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe"),
                        Arguments = $@"/generic:TERMSRV/{Host} /user:{Credentials.Username} /pass:{Credentials.DecryptedPassword}",
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                ConnectionProcess.Start();
            }

            if (autoDispose)
            {
                Dispose();
            }
        }

        public override void Dispose()
        {
            if (Host != null)
            {
                var cmdkey = new Process
                {
                    StartInfo =
            {
                FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe"),
                Arguments = $@"/delete:TERMSRV/{Host}",
                WindowStyle = ProcessWindowStyle.Hidden
            }
                };
                cmdkey.Start();
            }
        }
    }
}
