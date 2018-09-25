using System;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Security;

namespace RemoteConnect
{
    internal class Credentials
    {
        #region Properties
        internal string Username { get; set; }
        internal SecureString Password { get; set; }
        internal bool UseCurrentUser { get; private set; }
        internal string DecryptedPassword { get { return DecryptPassword(); } }
        #endregion

        #region Constructors
        internal Credentials(bool useCurrentUser = true)
        {            
            Username = null;
            Password = null;
            UseCurrentUser = true;
        }
        #endregion

        #region Instance Methods
        internal bool ValidateCredentials()
        {
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
            {
               return pc.ValidateCredentials(Username, DecryptedPassword);
            }
        }
        #endregion

        #region Private Methods
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
        #endregion
    }
}
