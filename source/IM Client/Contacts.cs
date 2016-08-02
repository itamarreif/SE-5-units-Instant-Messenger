using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IM_Client
{
    public partial class Contacts : Form
    {
        IM_Client im;
        public Dictionary<string, SingleDialogue> singleChats = new Dictionary<string, SingleDialogue>();        

        public Contacts(IM_Client IM)
        {
            InitializeComponent();            
            this.im = IM;
            this.Text = im.Username;            
            this.ControlBox = false;
        }

        private void chatButton_Click(object sender, EventArgs e)
        {
            if (chatTextBox.Text == im.Username)
                MessageBox.Show("You cannnot talk to yourself.");
            else if (!singleChats.ContainsKey(chatTextBox.Text))
            {
                SingleDialogue sd = new SingleDialogue(im, chatTextBox.Text, this);
                singleChats.Add(chatTextBox.Text, sd);
                chatTextBox.Text = "";
                sd.Show();
            }            
            else
                MessageBox.Show(String.Format("Chat with {0} is already open.", chatTextBox.Text));
        }       

        private void logOut_Click(object sender, EventArgs e)
        {
            im.Disconnect();            
            Application.Exit();            
        }
    }
}
