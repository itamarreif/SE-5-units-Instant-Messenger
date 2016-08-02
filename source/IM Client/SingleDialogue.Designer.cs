namespace IM_Client
{
    partial class SingleDialogue
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.talkTextBox = new System.Windows.Forms.RichTextBox();
            this.sendTextBox = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.endChatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendButton = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // talkTextBox
            // 
            this.talkTextBox.Location = new System.Drawing.Point(12, 27);
            this.talkTextBox.Name = "talkTextBox";
            this.talkTextBox.Size = new System.Drawing.Size(262, 311);
            this.talkTextBox.TabIndex = 0;
            this.talkTextBox.Text = "";
            // 
            // sendTextBox
            // 
            this.sendTextBox.Location = new System.Drawing.Point(12, 344);
            this.sendTextBox.Name = "sendTextBox";
            this.sendTextBox.Size = new System.Drawing.Size(193, 65);
            this.sendTextBox.TabIndex = 1;
            this.sendTextBox.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.endChatToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(286, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // endChatToolStripMenuItem
            // 
            this.endChatToolStripMenuItem.Name = "endChatToolStripMenuItem";
            this.endChatToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.endChatToolStripMenuItem.Text = "End Chat";
            this.endChatToolStripMenuItem.Click += new System.EventHandler(this.endChatToolStripMenuItem_Click);
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(211, 344);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(63, 65);
            this.sendButton.TabIndex = 3;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // SingleDialogue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 421);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.sendTextBox);
            this.Controls.Add(this.talkTextBox);
            this.Controls.Add(this.menuStrip1);
            this.Name = "SingleDialogue";
            this.Text = "SingleDialogue";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SingleDialogue_FormClosing);
            this.Load += new System.EventHandler(this.SingleDialogue_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox talkTextBox;
        private System.Windows.Forms.RichTextBox sendTextBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem endChatToolStripMenuItem;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Timer timer;
    }
}