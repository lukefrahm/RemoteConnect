using System;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace RemoteConnect
{
    [Cmdlet(VerbsCommon.Open, "Connection")]
    [Alias("connect", "rdp", "con", "oc")]
    public class RemoteConnectCmdlet : PSCmdlet
    {
        #region Properties
        #region Parameters
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "", HelpMessageBaseName = "")]
        [Alias("CN","Name","H")]
        [ValidatePattern(@"(\d{1,3}.){3}\d{1,3}|\A([a-zA-z\W]+\.)+[a-zA-z]+")]
        public string HostName
        {
            get
            {
                return HostName;
            }
            set
            {
                // If not an IP and doesn't explicitly have LUKE.int, alter to be FQDN
                if (!Regex.IsMatch(value, @"[\d\d?\d?.]{3}\d\d?\d?") && !value.EndsWith(".LUKE.int"))
                {
                    if (value.EndsWith(".LUKE"))
                    {
                        value += ".int";
                    }
                    else if (value.Contains("."))
                    {
                        throw new Exception("Invalid domain entered or host name is malformed.");
                    }
                    else
                    {
                        value += ".LUKE.int";
                    }
                }

                HostName = value;
            }
        }

        [Parameter(Position = 1, HelpMessage = "", HelpMessageBaseName = "")]
        public SwitchParameter RunAs
        {
            get
            {
                return RunAs;
            }
            set
            {
                RunAs = value.IsPresent ? value : new SwitchParameter(true);
            }
        }

        [Parameter(HelpMessage = "", HelpMessageBaseName = "")]
        public SwitchParameter KeepAlive
        {
            get
            {
                return KeepAlive;
            }
            set
            {
                KeepAlive = value.IsPresent ? value : new SwitchParameter(true);
            }
        }

        [Parameter(HelpMessage = "", HelpMessageBaseName = "")]
        [Alias("Prt", "Proto")]
        [ValidateSet("RDP","SSH","COM","VNC","")]
        public string Protocol
        {
            get
            {
                return Protocol.ToUpper() ?? "RDP";
            }
            set
            {
                Protocol = string.IsNullOrWhiteSpace(value) ? "RDP" : value;
            }
        }
        #endregion

        #region Private Fields
        private dynamic _connectionObject;
        #endregion
        #endregion

        #region Cmdlet Methods
        protected override void BeginProcessing()
        {
            Authenticate();
        }

        protected override void ProcessRecord()
        {
            _connectionObject.Connect();
        }

        protected override void EndProcessing()
        {
            _connectionObject.Dispose();
        }
        #endregion

        #region Private Methods
        private void Authenticate()
        {
            Credentials credentials;

            if (RunAs.ToBool())
            {
                credentials = new Credentials(useCurrentUser: false);

                do
                {
                    Host.UI.WriteLine("Username: ");
                    credentials.Username = Host.UI.ReadLine();
                    if (credentials.Username == null)
                    {
                        ShowError("ERROR: Invalid username format. Please try again.");
                    }
                } while (credentials.Username != null);
                do
                {
                    Host.UI.WriteLine("Password: ");
                    credentials.Password = Host.UI.ReadLineAsSecureString();
                    if (credentials.Password == null)
                    {
                        ShowError("ERROR: No password entered. Please try again.");
                    }
                } while (credentials.Password != null);

                if (!credentials.ValidateCredentials())
                {
                    throw new PSSecurityException("ERROR: Invalid credentials entered.");
                }
            }
            else
            {
                credentials = new Credentials(useCurrentUser: true);
            }

            _connectionObject = Connection.CreateDynamic(HostName, credentials, Protocol, KeepAlive);
        }

        private void ShowError(string errorMessage)
        {
            ConsoleColor defaultForegroundColor = Host.UI.RawUI.ForegroundColor;
            ConsoleColor defaultBackgroundColor = Host.UI.RawUI.BackgroundColor;
            
            if(defaultBackgroundColor == ConsoleColor.Red) 
                Host.UI.WriteLine(ConsoleColor.Red, ConsoleColor.Black, errorMessage);
            else
                Host.UI.WriteLine(ConsoleColor.Red, Host.UI.RawUI.BackgroundColor, errorMessage);

            Host.UI.RawUI.ForegroundColor = defaultForegroundColor;
            Host.UI.RawUI.BackgroundColor = defaultBackgroundColor;
        }
        #endregion
    }
}