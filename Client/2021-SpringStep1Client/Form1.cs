using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2021_SpringStep1Client
{
    public partial class Form1 : Form
    {
        bool terminating = false; //this is the will of the client to disconnect from the server. 
        bool connected = false; //Initially the client is not connected.
        bool disconnectPressed = false;
        string clientUsername = "";
        Socket clientSocket; 
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);// do it in order to not need any delegate to access the GUI object. 
            InitializeComponent();
        }

   
        private void Button_connect_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // IPv4, stream to exchange messages byte array forms.
            string IP = textBoxIP.Text;
            int Port;
            richTextBox1.Clear();
            if(Int32.TryParse(textBoxPort.Text, out Port)) // check if integer or not if not sure use it in that way with TryParse
            {
                string username = textBoxUsername.Text;
                //Possible Problems.
                if(username == "")
                { clientLogs.AppendText("Please enter you username.\n");}
                else if(IP == "")
                { clientLogs.AppendText("Please enter an IP address.\n"); }

                //If no problem related to textboxes in order to connect.
                else
                {
                    try 
                    {
                        clientSocket.Connect(IP, Port);
                        try
                        {
                            clientSocket.Send(Encoding.Default.GetBytes(username));
                            try
                            {
                                Byte[] responsebuffer = new Byte[64];
                                clientSocket.Receive(responsebuffer);
                                string response = Encoding.Default.GetString(responsebuffer);
                                //response = response.Substring(0, response.IndexOf("\0"));
                                response = response.Trim('\0');

                                if (response == "NOT_FOUND")
                                {
                                    clientLogs.AppendText("Please enter a valid username.\n");// not a db user, error. 
                                }
                                else if (response == "Already_Connected")
                                {
                                    clientLogs.AppendText("This user is already connected.\n");
                                }
                                else if (response == "SUCCESS")
                                {
                                    
                                    Button_connect.Enabled = false; // since we are connected we should not able to click to connect again. 
                                    button_SendPost.Enabled = true;
                                    button_Disconnect.Enabled = true;
                                    button_AllPosts.Enabled = true;
                                    removefriendbutton.Enabled = true;
                                    addfriendbutton.Enabled = true;
                                    deletebutton.Enabled = true;
                                    button_AllPosts.Enabled = true;
                                    friendspostbutton.Enabled = true;
                                    buttonmypost.Enabled = true;
                                    textBoxPost.Enabled = true; // should be able to send posts.
                                    disconnectPressed = false;
                                    connected = true;
                                    clientUsername = username;
                                    clientLogs.AppendText("Hello " + username + "! You are connected to the server.\n");

                                    Thread receiveThread = new Thread(Receive);
                                    receiveThread.Start();
                                }
                            }
                            catch
                            {
                                clientLogs.AppendText("There was a problem receiving response.\n");
                            }
                        }
                        catch
                        {
                            clientLogs.AppendText("Problem occured while username is sent.\n");
                        }
                    }
                    catch
                    {
                        clientLogs.AppendText("Could not connect to the server.\n");
                    }
                    
                }
            }
            else
            {
                clientLogs.AppendText("Check the port\n");
            }
        }

        private void button_SendPost_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            string message = "SEND_POSTS" + textBoxPost.Text;
            textBoxPost.Text = "";
            if (message != "" && message.Length <= 64)//correct format of the message. 
            {
                Byte[] buffer = Encoding.Default.GetBytes(message);
                try
                {
                    clientSocket.Send(buffer);

                    clientLogs.AppendText("You have successfully sent a post!\n");
                    clientLogs.AppendText(clientUsername + ": " + message.Substring(10) + " \n");
                }
                catch
                {
                    clientLogs.AppendText("There was a problem sending the post to the server.\n");
                }
            }

        }

        private void button_AllPosts_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            string message = "SHOW_POSTS";
            Byte[] buffer = Encoding.Default.GetBytes(message);
            try
            {
                clientLogs.AppendText("\nShowing all posts from clients: \n");
                clientSocket.Send(buffer);
            }
            catch
            {
                clientLogs.AppendText("There was a problem in the request of reaching posts page to server.\n");
            }

        }
        private void Receive()
        {
            while (connected)
            { 
                try
                {   Byte[] buffer = new Byte[1024];
                    clientSocket.Receive(buffer);

                    Byte[] response = Encoding.Default.GetBytes("receivedinfo");
                    clientSocket.Send(response);

                    string incomingMessage = Encoding.Default.GetString(buffer);
                    incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                    //incomingMessage = incomingMessage.Trim('\0');

                   string label = incomingMessage.Substring(0, 10);

                    if (label == "SHOW_POSTS")
                    {
                       clientLogs.AppendText(incomingMessage.Substring(10) + "\n");
                    }
                    else
                    {
                        
                        richTextBox1.AppendText(incomingMessage.Substring(10) + "\n");
                    }

                }
                catch
                {
                    if (!terminating && !disconnectPressed)//not terminating the action and not disconnected.
                    {
                        clientLogs.AppendText("The server has disconnected.\n");//Probably the server has stopped, client cannot connect then to that server. 
                        Button_connect.Enabled = true;
                        textBoxPost.Enabled = false;
                        button_SendPost.Enabled = false;
                        button_Disconnect.Enabled = false;
                        button_AllPosts.Enabled = false;
                    }

                    clientSocket.Close();
                    connected = false;
                }

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            connected = false;
            terminating = true;
            Environment.Exit(0);
            //throw new NotImplementedException();
        }

        private void button_Disconnect_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            string message = "DISCONNECT";
            Byte[] buffer = Encoding.Default.GetBytes(message);
            try
            {
                clientSocket.Send(buffer);

                disconnectPressed = true;

                Button_connect.Enabled = true;
                textBoxPost.Enabled = false;
                button_SendPost.Enabled = false;
                button_Disconnect.Enabled = false;
                button_AllPosts.Enabled = false;
                clientSocket.Close();
                connected = false;

                clientLogs.AppendText("Successfuly disconnected.\n");
                richTextBox1.Clear();
            }
            catch
            {
                clientLogs.AppendText("There was a problem sending disconnect request to server.\n");
            }
        }

        private void deletebutton_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            string message = "Deletepost"+textBoxPID.Text;
            Byte[] buffer = Encoding.Default.GetBytes(message);
            try
            {
                clientSocket.Send(buffer);
            }
            catch
            {
                clientLogs.AppendText("There was a problem in the request of reaching posts page to server.\n");
            }
        }

        private void removefriendbutton_Click(object sender, EventArgs e)
        {
            string message = "Deleteuser" + textBoxFriend.Text;
            richTextBox1.Clear();
            Byte[] buffer = Encoding.Default.GetBytes(message);
            try
            {
                
                clientSocket.Send(buffer);
            }
            catch
            {
                clientLogs.AppendText("There was a problem in the request of reaching posts page to server.\n");
            }
        }

        private void buttonmypost_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            string message = "SHOWMPOSTS";
            Byte[] buffer = Encoding.Default.GetBytes(message);
            try
            {
                clientLogs.AppendText("\nShowing your posts: \n");
                clientSocket.Send(buffer);
            }
            catch
            {
                clientLogs.AppendText("There was a problem in the request of reaching posts page to server.\n");
            }

        }

        private void addfriendbutton_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            string message = "Addfriends"+textBoxFriend.Text;
            
            Byte[] buffer = Encoding.Default.GetBytes(message);
            try
            {
                
                clientSocket.Send(buffer);
            }
            catch
            {
                clientLogs.AppendText("There was a problem in the request of reaching posts page to server.\n");
            }
        }

        private void friendspostbutton_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            string message = "friendpost";
            Byte[] buffer = Encoding.Default.GetBytes(message);
            try
            {
                clientLogs.AppendText("\nShowing all posts from clients friend list: \n");
                clientSocket.Send(buffer);
            }
            catch
            {
                clientLogs.AppendText("There was a problem in the request of reaching friends posts page to server.\n");
            }
        }
    }
}
