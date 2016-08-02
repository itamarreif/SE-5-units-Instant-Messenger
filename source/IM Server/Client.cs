using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Security.Authentication;


namespace IM_Server
{
    class Client
    {
        public Client(Program p, TcpClient c)
        {
            prog = p;
            client = c;
            (new Thread(new ThreadStart(SetupConn))).Start(); // Start new connection with TcpClient c on a new thread
        }

        Program prog;
        public TcpClient client;
        public NetworkStream netStream;  // Raw-data stream of connection.
        public SslStream ssl;            // Encrypts connection using SSL.
        public BinaryReader br;          // Read simple data
        public BinaryWriter bw;          // Write simple data
        UserInfo userInfo;

        void SetupConn()  // Set up connection
        {
            Console.WriteLine("[{0}] New connection!", DateTime.Now);
            netStream = client.GetStream();
            ssl = new SslStream(netStream, false);
            ssl.AuthenticateAsServer(prog.serverCert, false, SslProtocols.Tls, true);
            Console.WriteLine("[{0}] Connection authenticated!", DateTime.Now);
            br = new BinaryReader(ssl, Encoding.UTF8);
            bw = new BinaryWriter(ssl, Encoding.UTF8);

            bw.Write(IM_Hello); // Send confirmation to client
            bw.Flush();

            int hello = br.ReadInt32();
            if (hello == IM_Hello) // Confirmation from client valid
            {
                byte logMode = br.ReadByte();
                string user = br.ReadString();
                string pass = br.ReadString();
                if (user.Length < 10 || user.Length < 3) // User isnt too long
                {
                    if (pass.Length < 20 || pass.Length < 3) // Password isnt too long
                    {
                        if (logMode == IM_Register) // User registers
                        {
                            if (!prog.Users.ContainsKey(user)) // User doesn't exist in user collection
                            {
                                // Add user to user collection and associates it with current (this) connection
                                userInfo = new UserInfo(user, pass, this);
                                prog.Users.Add(user, userInfo);
                                bw.Write(IM_OK);
                                bw.Flush();
                                Console.WriteLine("[{0}] ({1}) Registered new user", DateTime.Now, user);
                                prog.SaveUsers(); // Update Users collection
                                Receiver();
                            }
                            else
                                bw.Write(IM_Exists);
                        }
                        else if (logMode == IM_Login) // User logs in
                        {                            
                            if (prog.Users.TryGetValue(user, out userInfo)) // User exists in user collection
                            {
                                if (pass == userInfo.Password) // Correct password
                                {
                                    if (userInfo.LoggedIn) // Disconnect whoever is logged in on this user
                                        userInfo.Connection.CloseConn();

                                    // Associate connection to the logged-in user
                                    userInfo.Connection = this;
                                    bw.Write(IM_OK);
                                    bw.Flush();
                                    Receiver();
                                }
                                else
                                    bw.Write(IM_WrongPass);
                            }
                            else
                                bw.Write(IM_NoExists);
                        }                        
                    }
                    else
                        bw.Write(IM_TooPassword);
                }
                else
                    bw.Write(IM_TooUsername);

            }

            CloseConn();
        }
        void CloseConn() // Close connection
        {
            try
            {
                userInfo.LoggedIn = false;
                br.Close();
                bw.Close();
                ssl.Close();
                client.Close();
                Console.WriteLine("[{0}] End of connection!", DateTime.Now);
            }
            catch { }
        }

        void Receiver() // Receive all incoming packets loop
        {
            Console.WriteLine("[{0}] ({1}) User logged in", DateTime.Now, userInfo.Username);
            userInfo.LoggedIn = true;
            try
            {
                while (client.Connected) // While connected
                {
                    byte type = br.ReadByte(); // Get incoming packet type

                    if (type==IM_IsAvailable) // Check if 'who' is available
                    {
                        string who = br.ReadString();
                        bw.Write(IM_IsAvailable);
                        bw.Write(who);

                        UserInfo info;
                        if (prog.Users.TryGetValue(who, out info)) // If 'who' is registered, check if logged in
                        {
                            if (info.LoggedIn)
                                bw.Write(true); // Available
                            else
                                bw.Write(false); // Unavailable
                        }
                        else 
                            bw.Write(false); // 'who' Is not registered - unavailable
                        bw.Flush();
                    }
                    else if (type==IM_Send) // A message is sent to another user
                    {
                        string to = br.ReadString();
                        string msg = br.ReadString();

                        UserInfo recipient;
                        if (prog.Users.TryGetValue(to, out recipient)) // Does 'recipient' exist?
                        {
                            if (recipient.LoggedIn) // Is 'recipient' logged in?
                            {
                                recipient.Connection.bw.Write(IM_Received); // Confirm message is received
                                recipient.Connection.bw.Write(userInfo.Username); // From who the message was sent
                                recipient.Connection.bw.Write(msg); // The message
                                recipient.Connection.bw.Flush();
                                Console.WriteLine("[{0}] ({1} -> {2}) Message sent!", DateTime.Now, userInfo.Username, recipient.Username);

                            }
                        }
                    }
                }
            }
            catch (IOException){ } // AKA no input = closed connection

            userInfo.LoggedIn = false;
            Console.WriteLine("[{0}] ({1}) User logged out", DateTime.Now, userInfo.Username);
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
    }
}
