namespace _2021_SpringStep1Client
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.button_Disconnect = new System.Windows.Forms.Button();
            this.Button_connect = new System.Windows.Forms.Button();
            this.clientLogs = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPost = new System.Windows.Forms.TextBox();
            this.button_SendPost = new System.Windows.Forms.Button();
            this.button_AllPosts = new System.Windows.Forms.Button();
            this.textBoxFriend = new System.Windows.Forms.TextBox();
            this.textBoxPID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.addfriendbutton = new System.Windows.Forms.Button();
            this.friendspostbutton = new System.Windows.Forms.Button();
            this.deletebutton = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.removefriendbutton = new System.Windows.Forms.Button();
            this.buttonmypost = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 99);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 59);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Port:";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(75, 20);
            this.textBoxIP.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(156, 20);
            this.textBoxIP.TabIndex = 3;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(75, 59);
            this.textBoxPort.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(156, 20);
            this.textBoxPort.TabIndex = 4;
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(75, 99);
            this.textBoxUsername.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(156, 20);
            this.textBoxUsername.TabIndex = 5;
            // 
            // button_Disconnect
            // 
            this.button_Disconnect.Enabled = false;
            this.button_Disconnect.Location = new System.Drawing.Point(269, 83);
            this.button_Disconnect.Margin = new System.Windows.Forms.Padding(2);
            this.button_Disconnect.Name = "button_Disconnect";
            this.button_Disconnect.Size = new System.Drawing.Size(76, 32);
            this.button_Disconnect.TabIndex = 7;
            this.button_Disconnect.Text = "Disconnect";
            this.button_Disconnect.UseVisualStyleBackColor = true;
            this.button_Disconnect.Click += new System.EventHandler(this.button_Disconnect_Click);
            // 
            // Button_connect
            // 
            this.Button_connect.Location = new System.Drawing.Point(269, 20);
            this.Button_connect.Margin = new System.Windows.Forms.Padding(2);
            this.Button_connect.Name = "Button_connect";
            this.Button_connect.Size = new System.Drawing.Size(76, 32);
            this.Button_connect.TabIndex = 8;
            this.Button_connect.Text = "Connect";
            this.Button_connect.UseVisualStyleBackColor = true;
            this.Button_connect.Click += new System.EventHandler(this.Button_connect_Click);
            // 
            // clientLogs
            // 
            this.clientLogs.Location = new System.Drawing.Point(376, 20);
            this.clientLogs.Margin = new System.Windows.Forms.Padding(2);
            this.clientLogs.Name = "clientLogs";
            this.clientLogs.ReadOnly = true;
            this.clientLogs.Size = new System.Drawing.Size(262, 367);
            this.clientLogs.TabIndex = 9;
            this.clientLogs.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 358);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Post:";
            // 
            // textBoxPost
            // 
            this.textBoxPost.Enabled = false;
            this.textBoxPost.Location = new System.Drawing.Point(82, 358);
            this.textBoxPost.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxPost.Name = "textBoxPost";
            this.textBoxPost.Size = new System.Drawing.Size(151, 20);
            this.textBoxPost.TabIndex = 11;
            // 
            // button_SendPost
            // 
            this.button_SendPost.Enabled = false;
            this.button_SendPost.Location = new System.Drawing.Point(251, 358);
            this.button_SendPost.Margin = new System.Windows.Forms.Padding(2);
            this.button_SendPost.Name = "button_SendPost";
            this.button_SendPost.Size = new System.Drawing.Size(75, 29);
            this.button_SendPost.TabIndex = 12;
            this.button_SendPost.Text = "Send";
            this.button_SendPost.UseVisualStyleBackColor = true;
            this.button_SendPost.Click += new System.EventHandler(this.button_SendPost_Click);
            // 
            // button_AllPosts
            // 
            this.button_AllPosts.Enabled = false;
            this.button_AllPosts.Location = new System.Drawing.Point(361, 418);
            this.button_AllPosts.Margin = new System.Windows.Forms.Padding(2);
            this.button_AllPosts.Name = "button_AllPosts";
            this.button_AllPosts.Size = new System.Drawing.Size(75, 29);
            this.button_AllPosts.TabIndex = 13;
            this.button_AllPosts.Text = "All Posts";
            this.button_AllPosts.UseVisualStyleBackColor = true;
            this.button_AllPosts.Click += new System.EventHandler(this.button_AllPosts_Click);
            // 
            // textBoxFriend
            // 
            this.textBoxFriend.Location = new System.Drawing.Point(83, 313);
            this.textBoxFriend.Name = "textBoxFriend";
            this.textBoxFriend.Size = new System.Drawing.Size(148, 20);
            this.textBoxFriend.TabIndex = 14;
            // 
            // textBoxPID
            // 
            this.textBoxPID.Location = new System.Drawing.Point(82, 418);
            this.textBoxPID.Name = "textBoxPID";
            this.textBoxPID.Size = new System.Drawing.Size(151, 20);
            this.textBoxPID.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 418);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Post ID:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 313);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Username:";
            // 
            // addfriendbutton
            // 
            this.addfriendbutton.Enabled = false;
            this.addfriendbutton.Location = new System.Drawing.Point(251, 313);
            this.addfriendbutton.Margin = new System.Windows.Forms.Padding(2);
            this.addfriendbutton.Name = "addfriendbutton";
            this.addfriendbutton.Size = new System.Drawing.Size(75, 29);
            this.addfriendbutton.TabIndex = 18;
            this.addfriendbutton.Text = "Add Friend";
            this.addfriendbutton.UseVisualStyleBackColor = true;
            this.addfriendbutton.Click += new System.EventHandler(this.addfriendbutton_Click);
            // 
            // friendspostbutton
            // 
            this.friendspostbutton.Enabled = false;
            this.friendspostbutton.Location = new System.Drawing.Point(533, 418);
            this.friendspostbutton.Margin = new System.Windows.Forms.Padding(2);
            this.friendspostbutton.Name = "friendspostbutton";
            this.friendspostbutton.Size = new System.Drawing.Size(75, 29);
            this.friendspostbutton.TabIndex = 19;
            this.friendspostbutton.Text = "Friend\'s Post";
            this.friendspostbutton.UseVisualStyleBackColor = true;
            this.friendspostbutton.Click += new System.EventHandler(this.friendspostbutton_Click);
            // 
            // deletebutton
            // 
            this.deletebutton.Enabled = false;
            this.deletebutton.Location = new System.Drawing.Point(251, 418);
            this.deletebutton.Margin = new System.Windows.Forms.Padding(2);
            this.deletebutton.Name = "deletebutton";
            this.deletebutton.Size = new System.Drawing.Size(75, 29);
            this.deletebutton.TabIndex = 20;
            this.deletebutton.Text = "Delete";
            this.deletebutton.UseVisualStyleBackColor = true;
            this.deletebutton.Click += new System.EventHandler(this.deletebutton_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(78, 135);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(151, 134);
            this.richTextBox1.TabIndex = 21;
            this.richTextBox1.Text = "";
            // 
            // removefriendbutton
            // 
            this.removefriendbutton.Enabled = false;
            this.removefriendbutton.Location = new System.Drawing.Point(115, 274);
            this.removefriendbutton.Margin = new System.Windows.Forms.Padding(2);
            this.removefriendbutton.Name = "removefriendbutton";
            this.removefriendbutton.Size = new System.Drawing.Size(92, 29);
            this.removefriendbutton.TabIndex = 22;
            this.removefriendbutton.Text = "Remove Friend";
            this.removefriendbutton.UseVisualStyleBackColor = true;
            this.removefriendbutton.Click += new System.EventHandler(this.removefriendbutton_Click);
            // 
            // buttonmypost
            // 
            this.buttonmypost.Enabled = false;
            this.buttonmypost.Location = new System.Drawing.Point(444, 447);
            this.buttonmypost.Margin = new System.Windows.Forms.Padding(2);
            this.buttonmypost.Name = "buttonmypost";
            this.buttonmypost.Size = new System.Drawing.Size(75, 29);
            this.buttonmypost.TabIndex = 23;
            this.buttonmypost.Text = "My Posts";
            this.buttonmypost.UseVisualStyleBackColor = true;
            this.buttonmypost.Click += new System.EventHandler(this.buttonmypost_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 487);
            this.Controls.Add(this.buttonmypost);
            this.Controls.Add(this.removefriendbutton);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.deletebutton);
            this.Controls.Add(this.friendspostbutton);
            this.Controls.Add(this.addfriendbutton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxPID);
            this.Controls.Add(this.textBoxFriend);
            this.Controls.Add(this.button_AllPosts);
            this.Controls.Add(this.button_SendPost);
            this.Controls.Add(this.textBoxPost);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.clientLogs);
            this.Controls.Add(this.Button_connect);
            this.Controls.Add(this.button_Disconnect);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Button button_Disconnect;
        private System.Windows.Forms.Button Button_connect;
        private System.Windows.Forms.RichTextBox clientLogs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxPost;
        private System.Windows.Forms.Button button_SendPost;
        private System.Windows.Forms.Button button_AllPosts;
        private System.Windows.Forms.TextBox textBoxFriend;
        private System.Windows.Forms.TextBox textBoxPID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button addfriendbutton;
        private System.Windows.Forms.Button friendspostbutton;
        private System.Windows.Forms.Button deletebutton;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button removefriendbutton;
        private System.Windows.Forms.Button buttonmypost;
    }
}

