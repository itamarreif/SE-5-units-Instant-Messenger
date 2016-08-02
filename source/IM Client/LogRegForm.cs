using System;
using System.Windows.Forms;

namespace IM_Client
{
    public partial class LogRegForm : Form
    {
        IM_Client im;        

        public LogRegForm()
        {
            InitializeComponent();

            this.im = new IM_Client();
            
            im.LoginOK += OnIm_LoginOK;
            im.RegisterOK += OnIm_RegisterOK;
            im.LoginFailed += OnIm_LoginFailed;
            im.RegisterFailed += OnIm_RegisterFailed;            
        }

        private string usernameInput;
        private string passwordInput;

        private void loginButton_Click(object sender, EventArgs e)
        {
            if (usernameTextBox.Text != null && passwordTextBox.Text != null) 
            {
                this.usernameInput = usernameTextBox.Text;
                this.passwordInput = passwordTextBox.Text;

                im.Login(usernameInput, passwordInput);
                status.Text = "Logging In...";
            }
        }
        private void OnIm_LoginOK(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                status.Text = "Logged In!";
                registerButton.Enabled = false;
                loginButton.Enabled = false;
                usernameTextBox.Enabled = false;
                passwordTextBox.Enabled = false;

                Contacts c = new Contacts(im);
                c.Show();
                this.Hide();
            }));
        }
        private void OnIm_LoginFailed(object sender, IMErrorEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {

                MessageBox.Show(String.Format("Login Failed! Error Code {0}", e.Error.ToString()));
                usernameTextBox.Text = "";
                passwordTextBox.Text = "";
                this.status.Text = "Log In or Register";
                im.Disconnect();
                im = new IM_Client();
                im.LoginOK += OnIm_LoginOK;
                im.RegisterOK += OnIm_RegisterOK;
                im.LoginFailed += OnIm_LoginFailed;
                im.RegisterFailed += OnIm_RegisterFailed;
            }));
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            if (usernameTextBox.Text != null && passwordTextBox.Text != null)
            {
                this.usernameInput = usernameTextBox.Text;
                this.passwordInput = passwordTextBox.Text;

                im.Register(usernameInput, passwordInput);
                status.Text = "Registering...";
            }

        }
        private void OnIm_RegisterOK(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate 
            {
                status.Text = "Registered and Logged In!";
                registerButton.Enabled = false;
                loginButton.Enabled = false;
                usernameTextBox.Enabled = false;
                passwordTextBox.Enabled = false;

                Contacts c = new Contacts(im);
                c.Show();
                this.Hide();
            }));
        }
        private void OnIm_RegisterFailed(object sender, IMErrorEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                MessageBox.Show("Register Failed! Error Code {0}", e.Error.ToString());
                usernameTextBox.Text = "";
                passwordTextBox.Text = "";
                this.status.Text = "Log In or Register";
                im = new IM_Client();
            }));
        }
    }
}
