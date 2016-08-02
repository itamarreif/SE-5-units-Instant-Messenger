using System;

namespace IM_Server
{
    [Serializable]
    class UserInfo
    {
        public string Username;
        public string Password;
        [NonSerialized] public bool LoggedIn;       // Is logged in and connected?
        [NonSerialized] public Client Connection;   // Connection info

        public UserInfo(string user, string pass)
        {
            this.Username = user;
            this.Password = pass;
            this.LoggedIn = false;
        }

        public UserInfo(string user, string pass, Client conn)
        {
            this.Username = user;
            this.Password = pass;
            this.LoggedIn = true;
            this.Connection = conn;
        }
    }
}
