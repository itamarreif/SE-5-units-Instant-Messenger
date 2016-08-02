using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization.Formatters.Binary;

namespace IM_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            Console.WriteLine();
            Console.WriteLine("Press enter to close program.");
            Console.ReadLine();
        }

        public IPAddress ip = IPAddress.Parse("127.0.0.1"); // Currently set to localhost
        public int port = 2000;
        public bool running = true;
        public TcpListener server;  
        public Dictionary<string, UserInfo> Users;
        public X509Certificate2 serverCert = new X509Certificate2("serverCert.pfx", "2486");

        public Program()
        {
            Console.Title = "InstantMessenger Server";
            Console.WriteLine("----- InstantMessenger Server -----");
            Users = new Dictionary<string, UserInfo>();
            LoadUsers();
            Console.WriteLine("[{0}] Starting server...", DateTime.Now);

            server = new TcpListener(ip, port);
            server.Start();
            Console.WriteLine("[{0}] Server is running properly!", DateTime.Now);

            Listen();
        }

        void Listen()
        {
            while (running)
            {
                TcpClient tcpClient = server.AcceptTcpClient();
                Client client = new Client(this, tcpClient);
            }
        }

        string usersFileName = Environment.CurrentDirectory + "\\users.dat";
        public void SaveUsers() // Save users data to file
        {
            try
            {
                Console.WriteLine("[{0}] Saving users...", DateTime.Now);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = new FileStream(usersFileName, FileMode.Create, FileAccess.Write);
                bf.Serialize(file, Users.Values.ToArray()); // Conevert Users to array and serialize to file
                file.Close();
                Console.WriteLine("[{0}] Users saved!", DateTime.Now);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void LoadUsers() // Load user data
        {
            try
            {
                Console.WriteLine("[{0}] Loading users...", DateTime.Now);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = new FileStream(usersFileName, FileMode.Open, FileAccess.Read);
                UserInfo[] infos = (UserInfo[])bf.Deserialize(file); // Deserialize info from file into UserInfo array
                file.Close();

                Users = infos.ToDictionary((UserInfo u) => u.Username, (UserInfo u) => u); // Converts the "infos" array to a dictionary using <UserInfo>u's Username property as key and "u" as value
            }
            catch { }
        }

    }
}
