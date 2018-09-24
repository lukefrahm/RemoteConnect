using System;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Security;

namespace RemoteConnect
{
    internal class CredentialSet
    {
        internal string Username { get; private set; }
        internal SecureString Password { get; private set; }
        internal bool UseCurrentUser { get; private set; }
        internal string DecryptedPassword
        {
            get
            {
                return DecryptPassword();
            }
        }

        internal CredentialSet(string username, SecureString password)
        {
            Username = username;
            Password = password;
            UseCurrentUser = false;
        }

        internal CredentialSet(bool useCurrentUser = true)
        {
            Username = null;
            Password = null;
            UseCurrentUser = true;
        }

        internal bool ValidateCredentials()
        {
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
            {
               return pc.ValidateCredentials(Username, DecryptedPassword);
            }
        }

        private string DecryptPassword()
        {
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(Password);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            catch
            {
                return null;
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
