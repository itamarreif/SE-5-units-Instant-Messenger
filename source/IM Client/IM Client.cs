using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace IM_Client
{
    public class IM_Client
    {
        public Thread TcpThread;    // Connection Thread
        public bool Conn = false;   // Is connected/connecting?
        private bool Logged = false; // Is logged in?
        private string User;         // Username
        private string Pass;         // Password
        public bool Reg;            // Register Mode
        
        public string Server { get { return "localhost"; } }
        public int Port { get { return 2000; } }

        public bool IsLoggedIn { get { return Logged; } }
        public string Username { get { return User; } }
        public string Password { get { return Pass; } }

        // Start connection thread, login, register
        void connect(string username, string password, bool register)
        {
            if (!Conn)
            {
                Conn = true;
                User = username;
                Pass = password;
                Reg = register;
                TcpThread = new Thread(new ThreadStart(SetupConn)); // New thread for new connection
                TcpThread.Start();
            }
        }

        public void Login(string username, string password)
        {
            connect(username, password, false);
        }
        public void Register(string username, string password)
        {
            connect(username, password, true);
        }
        public void Disconnect()
        {
            if (Conn)
                CloseConn();
        }
        public void IsAvailable(string user)
        {
            if (Conn)
            {
                bw.Write(IM_IsAvailable);
                bw.Write(user);
                bw.Flush();
            }
        } // At launch of chat instance (OnTalkButton_Click
        public void SendMessage(string to, string msg)
        {
            bw.Write(IM_Send);
            bw.Write(to);
            bw.Write(msg);
            bw.Flush();
        }

        public TcpClient client;
        public NetworkStream netStream;  // Raw-data stream of connection.
        public SslStream ssl;            // SSL connection
        public BinaryReader br;          // Read simple data
        public BinaryWriter bw;          // Write simple data

        void SetupConn()  // Setup connection
        {
            client = new TcpClient(Server, Port);            
            netStream = client.GetStream();
            ssl = new SslStream(netStream, false, new RemoteCertificateValidationCallback(ValidateCert));
            ssl.AuthenticateAsClient("Itamar Reif");
            br = new BinaryReader(ssl, Encoding.UTF8);
            bw = new BinaryWriter(ssl, Encoding.UTF8);
                       
            int hello = br.ReadInt32();
            if (hello == IM_Hello) // Receive confirmation from server
            {
                bw.Write(IM_Hello);
                bw.Flush(); // Send back confirmation to server

                bw.Write(Reg ? IM_Register : IM_Login); // Register or log in
                bw.Write(Username);
                bw.Write(Password);
                bw.Flush();

                byte ans = br.ReadByte();
                if (ans == IM_OK) // Login/Register OK
                {
                    if (Reg)
                    {
                        OnRegisterOK();
                        Receiver(); // Packet receiving loop
                    }
                    else
                    {
                        OnLoginOK();
                        Receiver(); // Packet receiving loop
                    }
                }
                else // Login/Register FAIL
                {
                    IMErrorEventArgs error = new IMErrorEventArgs((IMError)ans);
                    if (Reg)
                        OnRegisterFailed(error);
                    else
                        OnLoginFailed(error);
                }                
            }
            
            CloseConn();
        }

        void CloseConn() // Close connection.
        {
            br.Close();
            bw.Close();
            netStream.Close();
            ssl.Close();
            OnDisconnected();
            client.Close();
        }

        void Receiver() // Packet receiving loop
        {
            Logged = true;

            try
            {
                while (client.Connected)
                {
                    byte type = br.ReadByte();
                    if (type == IM_IsAvailable)
                    {
                        string user = br.ReadString();
                        bool isAvail = br.ReadBoolean();
                        OnUserAvail(new IMAvailEventArgs(user, isAvail));
                    }
                    else if (type == IM_Received) 
                    {
                        string from = br.ReadString();
                        string msg = br.ReadString();
                        OnMessageRec(new IMReceivedEventArgs(from, msg));
                    }
                }
            }
            catch (IOException) { } // AKA no input = closed connection

            Logged = false;
        }

        // Events
        public event EventHandler LoginOK;
        public event EventHandler RegisterOK;
        public event EventHandler<IMErrorEventArgs> LoginFailed;
        public event EventHandler<IMErrorEventArgs> RegisterFailed;
        public event EventHandler Disconnected;
        public event EventHandler<IMAvailEventArgs> UserAvailable;
        public event EventHandler<IMReceivedEventArgs> MessageReceived;

        protected virtual void OnLoginOK()
        {
            if (LoginOK != null)
                LoginOK(this, EventArgs.Empty);
        }
        protected virtual void OnRegisterOK()
        {
            if (RegisterOK != null)
                RegisterOK(this, EventArgs.Empty);
        }
        protected virtual void OnLoginFailed(IMErrorEventArgs e)
        {
            if (LoginFailed != null)
                LoginFailed(this, e);
        }
        protected virtual void OnRegisterFailed(IMErrorEventArgs e)
        {
            if (RegisterFailed != null)
                RegisterFailed(this, e);
        }
        protected virtual void OnDisconnected()
        {
            if (Disconnected != null)
                Disconnected(this, EventArgs.Empty);
        }
        protected virtual void OnUserAvail(IMAvailEventArgs e)
        {
            if (UserAvailable != null)
                UserAvailable(this, e);
        }
        protected virtual void OnMessageRec(IMReceivedEventArgs e)
        {
            if (MessageReceived != null)
                MessageReceived(this, e);
        }              
       
        public const int IM_Hello = 2012;      // Hello
        public const byte IM_OK = 0;           // OK
        public const byte IM_Login = 1;        // Login
        public const byte IM_Register = 2;     // Register
        public const byte IM_TooUsername = 3;  // Too long username
        public const byte IM_TooPassword = 4;  // Too long password
        public const byte IM_Exists = 5;       // Already exists
        public const byte IM_NoExists = 6;     // Doesn't exists
        public const byte IM_WrongPass = 7;    // Wrong password
        public const byte IM_IsAvailable = 8;  // Is user available?
        public const byte IM_Send = 9;        // Send message
        public const byte IM_Received = 10;    // Message received
        
        public static bool ValidateCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None || sslPolicyErrors==SslPolicyErrors.RemoteCertificateChainErrors) // Assume cert is not trusted by local machine (personal)
                return true;
            else
            {
                MessageBox.Show(string.Format("SSL Error:{0}", sslPolicyErrors.ToString()));
                return false;                
            }
            // return true;
        }
    }
}
