using System;
using System.Security;

namespace RemoteConnect
{
    internal sealed class VncSession : ConnectionProperties, IDisposable
    {
        internal VncSession(string host, string username, SecureString password, string overrideFileName = @"")
            : base(host, username, password, Protocol.VNC, overrideFileName)
        {

        }

        internal override void Connect(bool autoDispose = false)
        {
            
        }

        public override void Dispose()
        {
            
        }
    }
}
