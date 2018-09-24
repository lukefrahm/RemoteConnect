using System;
using System.Management.Automation;
using System.Security;

namespace RemoteConnect
{
    [Cmdlet(VerbsCommon.Get, "Repo")]
    [Alias("clone", "fetch", "g-r", "gr")]
    public class RemoteConnectCmdlet : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "", HelpMessageBaseName = "")]
        [Alias("CN","Name","H")]
        public string HostName { get; set; }

        [Parameter(Position = 1, HelpMessage = "", HelpMessageBaseName = "")]
        public SwitchParameter RunAs { get; set; }

        [Parameter(HelpMessage = "", HelpMessageBaseName = "")]
        [Alias("Prt", "Proto")]
        [ValidateSet("RDP","SSH","COM","VNC")]
        public string Protocol { get; set; }

        private string _userName = null;
        private SecureString _password = null;

        protected override void BeginProcessing()
        {
            dynamic connectionObject = ConnectionProperties.CreateDynamic(HostName, _userName, _password, Protocol);
            connectionObject.Connect();

            if(RunAs.ToBool())
            {
                do
                {
                    Console.WriteLine("Username: ");
                    _userName = Console.ReadLine();
                    if (_userName == null)
                    {
                        ConsoleColor defaultColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR: Invalid username format. Please try again.");
                        Console.ForegroundColor = defaultColor;
                    }
                } while (_userName != null);
                do
                {
                    Console.WriteLine("Password: ");
                    _password = Host.UI.ReadLineAsSecureString();
                    if (_password == null)
                    {
                        ConsoleColor defaultColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("");
                        Console.ForegroundColor = defaultColor;
                    }
                } while (_password != null);
                
                if (!new CredentialSet(_userName, _password).ValidateCredentials())
                {
                    throw new PSSecurityException("ERROR: Invalid credentials entered.");
                }
            }

            // If this is reached, either the current user (validated) or a validated other user will be used
            // If not, the app will crash due to invalid creds
        }

        protected override void ProcessRecord()
        {
            switch (Protocol)
            {
                case "RDP":
                    new RdpSession(HostName, _userName, _password).Connect(autoDispose: true);
                    break;
                case "SSH":
                    new TerminalSession(HostName, _userName, _password, RemoteConnect.Protocol.SSH).Connect(autoDispose: true);
                    break;
                case "COM":
                    new TerminalSession(HostName, _userName, _password, RemoteConnect.Protocol.COM).Connect(autoDispose: true);
                    break;
                case "VNC":
                    new VncSession(HostName, _userName, _password).Connect(autoDispose: true);
                    break;
                default:
                    break;
            }
        }

        protected override void EndProcessing()
        {
            if (_password != null)
            {
                _password.Dispose();
            }
        }
    }
}