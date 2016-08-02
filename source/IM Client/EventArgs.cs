namespace IM_Client
{
    using System;

    public enum IMError : byte
    {
        TooUserName = IM_Client.IM_TooUsername,
        TooPassword = IM_Client.IM_TooPassword,
        Exists = IM_Client.IM_Exists,
        NoExists = IM_Client.IM_NoExists,
        WrongPassword = IM_Client.IM_WrongPass
    }

    public class IMErrorEventArgs : EventArgs
    {
        private IMError err;

        public IMErrorEventArgs(IMError error)
        {
            this.err = error;
        }

        public IMError Error { get { return err; } }
    }

    public class IMAvailEventArgs : EventArgs
    {
        private string user;
        private bool avail;

        public IMAvailEventArgs(string User, bool Avail)
        {
            this.user = User;
            this.avail = Avail;
        }

        public string Username { get { return user; } }
        public bool IsAvailable { get { return avail; } }
    }

    public class IMReceivedEventArgs : EventArgs
    {        
        private string user;
        private string msg;

        public IMReceivedEventArgs(string User, string Msg)
        {
            this.user = User;
            this.msg = Msg;
        }

        public string From { get { return user; } }
        public string Message { get { return msg; } }
    }
}
