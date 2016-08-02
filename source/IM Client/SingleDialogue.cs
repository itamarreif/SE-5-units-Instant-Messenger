using System;
using System.Windows.Forms;

namespace IM_Client
{
    public partial class SingleDialogue : Form
    {
        IM_Client im;
        public string sendTo;
        Contacts contacts;

        public SingleDialogue(IM_Client IM, string sendTO, Contacts c)
        {
            InitializeComponent();

            this.contacts = c;
            this.im = IM;
            this.sendTo = sendTO;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            im.SendMessage(sendTo, this.sendTextBox.Text);
            this.talkTextBox.Text += String.Format("{0}: {1}\r\n", im.Username, this.sendTextBox.Text);
            this.sendTextBox.Text = "";
        }

        private void SingleDialogue_Load(object sender, EventArgs e)
        {
            this.Text = sendTo;
            availHandler = new EventHandler<IMAvailEventArgs>(OnIm_UserAvailable);
            receivedHandler = new EventHandler<IMReceivedEventArgs>(OnIm_MessageReceived);
            this.talkTextBox.ReadOnly = true;
            this.sendTextBox.Enabled = false;
            im.UserAvailable += availHandler;
            im.MessageReceived += receivedHandler;

            this.timer.Start();
            im.IsAvailable(sendTo);
        }

        EventHandler<IMAvailEventArgs> availHandler;
        EventHandler<IMReceivedEventArgs> receivedHandler;

        private bool lastAvail = false;
        private void OnIm_UserAvailable(object sender, IMAvailEventArgs e)
        {
            if (e.Username == sendTo)
                if (e.IsAvailable != lastAvail) 
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        lastAvail = e.IsAvailable;
                        this.sendTextBox.Enabled = true;
                        string avail = (lastAvail ? "Available" : "Unavailable");
                        this.sendTextBox.Enabled = lastAvail;
                        this.talkTextBox.Text += string.Format("[{0} is {1}]\r\n", sendTo, avail);
                    }));
        }        

        private void OnIm_MessageReceived(object sender, IMReceivedEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                if (e.From == sendTo)
                    this.talkTextBox.Text += String.Format("{0}: {1}\r\n", e.From, e.Message);
            }));
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            im.IsAvailable(sendTo);
        }       

        private void SingleDialogue_FormClosing(object sender, FormClosingEventArgs e)
        {
            im.UserAvailable -= availHandler;
            im.MessageReceived -= receivedHandler;
            contacts.singleChats.Remove(sendTo);
        }

        private void endChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
