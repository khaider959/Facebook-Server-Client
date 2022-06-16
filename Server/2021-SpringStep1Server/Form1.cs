using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2021_SpringStep1Server
{
    public partial class Form1 : Form
    {
        int lineCount = File.ReadLines(@"../../user-db.txt").Count();

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> clientSockets = new List<Socket>();
        List<string> clientusernames = new List<string>();

        String fpath = Directory.GetCurrentDirectory() + "\\friendlist.txt";

        int postCount = CountPost();

        bool terminating = false;
        bool listening = false;
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }

        private void button_listen_Click(object sender, EventArgs e)
        {
            int port;
            if (Int32.TryParse(textBox_port.Text, out port))
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port); //listen in any interface, initialize end point here. 
                serverSocket.Bind(endPoint);
                serverSocket.Listen(3); 

                listening = true;
                button_listen.Enabled = false;

                //When client disconnect no problem in the server so no need to check here with try. 
                Thread acceptThread = new Thread(Accept); // Thread to accept new clients from now on. 
                acceptThread.Start();

                serverLogs.AppendText("Started listening on port: " + port + "\n");

            }
            else
            {
                serverLogs.AppendText("Please check port number \n");
            }

        }

        private void Accept() //Accepting new clients to the server. 
        {

            while (listening)
            {
                try
                {
                    Socket newClient = serverSocket.Accept(); // accept corresponding sockets for clients.

                    // Need to check username whether from database or not.
                    Thread usernameCheckThread = new Thread(() => usernameCheck(newClient));
                    usernameCheckThread.Start();
                }
                catch
                {
                    if (terminating) // If we close the server. No crash, correctly closed and not listening from now on. 
                    {
                        listening = false;
                    }
                    else //Problem occured here. 
                    {
                        serverLogs.AppendText("The socket stopped working.\n");
                    }

                }
            }

        }

        private void usernameCheck(Socket thisClient)
        {
            string message = "NOT_FOUND"; // will be used for usernames from outside the database. 
            try
            {
                Byte[] username_buffer = new Byte[64];
                thisClient.Receive(username_buffer);

                string username = Encoding.Default.GetString(username_buffer); // Convert byte array to string.
                username = username.Substring(0, username.IndexOf("\0"));
                //username = username.Trim('\0');

                if (clientusernames.Contains(username)) // if in database but already connected.
                {
                    serverLogs.AppendText(username + " has tried to connect from another client!\n");
                    message = "Already_Connected";
                }
                else
                {
                    var lines = File.ReadLines(@"../../user-db.txt"); // check the txt line by line.
                    foreach (var line in lines)
                    {
                        if (line == username) // if the db contains the username, can connect !
                        {
                            clientSockets.Add(thisClient);
                            clientusernames.Add(username);
                            message = "SUCCESS";
                            serverLogs.AppendText(username + " has connected.\n");
                            
                            //After the client is connected, Received information from the client's actions.
                            Thread receiveThread = new Thread(() => Receive(thisClient, username)); //Receive posts.
                            receiveThread.Start();
                        }
                    }

                }
                if(message=="NOT_FOUND")
                {
                    serverLogs.AppendText(username + " tried to connect to the server but cannot!\n");
                }
                try
                {
                    thisClient.Send(Encoding.Default.GetBytes(message)); //send the corresponding message to the client.
                    
                }
                catch
                {
                    serverLogs.AppendText("There was a problem when sending the username response to the client.\n");
                }
            }

            catch
            {
                serverLogs.AppendText("Problem receiving username.\n");
            }


        }

        private void Receive(Socket thisClient, string username)//Actions from clients.
        {
            bool connected = true; //To receive information, should be connected by default.
            while (connected && !terminating) //still connected and not closing.
            {
                try
                {
                    sendfriendlist(thisClient, username);
                    Byte[] buffer = new Byte[64];
                    thisClient.Receive(buffer);//Gets information related to thisclient.

                    string incomingMessage = Encoding.Default.GetString(buffer);//convert byte array to string.
                    //incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                    incomingMessage = incomingMessage.Trim('\0');

                    //string label = incomingMessage.Substring(0, 10);

                    if (incomingMessage.Substring(0, 10) == "DISCONNECT")
                    {
                        thisClient.Close();
                        clientSockets.Remove(thisClient);
                        clientusernames.Remove(username);//remove it from the connected list.
                        connected = false;
                        serverLogs.AppendText(username + " has disconnected\n");
                    }
                    else if (incomingMessage.Substring(0, 10) == "SHOW_POSTS")
                    {
                        allposts(thisClient, username); //This function will print all posts when requested. 
                    }
                    else if (incomingMessage.Substring(0, 10) == "SEND_POSTS")
                    {
                        string post = incomingMessage.Substring(10);
                        postCount += 1;
                        postToLog(username, postCount, post);
                    }
                    else if (incomingMessage.Substring(0, 10) == "Deletepost")
                    {
                        string postid = incomingMessage.Substring(10);
                   
                        deletepost(postid, username,thisClient);
                    }
                    else if (incomingMessage.Substring(0, 10) == "SHOWMPOSTS")
                    {
                        user_posts(thisClient, username);
                    }
                    else if (incomingMessage.Substring(0, 10) == "Addfriends")
                    {

                        string friendname = incomingMessage.Substring(10);
                        var lines = File.ReadLines(@"../../user-db.txt"); // check the txt line by line.
                        bool flag = false;
                        foreach (var line in lines)
                        {
                            if (line == friendname) // if the db contains the username, can connect !
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            addfriend(thisClient, username, friendname);
                        }
                        else
                        {
                            try
                            {
                                Byte[] friendmessage = Encoding.Default.GetBytes("SHOW_POSTS" + friendname + " is not in user database. \n");
                                thisClient.Send(friendmessage);
                                Byte[] response = new Byte[64];
                                thisClient.Receive(response);
                                string received = Encoding.Default.GetString(response);
                            }
                            catch
                            {
                                serverLogs.AppendText("couldn't add friend \n");
                            }
                        }
                    }
                    else if (incomingMessage.Substring(0, 10) == "Deleteuser")
                    {
                        string friendname = incomingMessage.Substring(10);
                        deletefriend(thisClient, username, friendname);
                    }
                    else if (incomingMessage.Substring(0, 10) == "friendpost")
                    {
                        string friendname = incomingMessage.Substring(10);
                        friend_posts(thisClient, username);
                    }

                }
                catch
                {
                    if (!terminating)
                    {
                        serverLogs.AppendText(username + " has disconnected.\n");
                    }
                    thisClient.Close();
                    clientSockets.Remove(thisClient);
                    clientusernames.Remove(username);
                    connected = false;
                }
            }

        }
        private void sendfriendlist(Socket thisClient, string username)
        {
            string[] lines = File.ReadAllLines(fpath);
            bool flag = false;
            for (int i=0; i < lines.Length; i++)
            {
                string[] names = lines[i].Split('-');
                for (int a=1; a< names.Length; a++)
                {
                    if (names[0] == username)
                    {
                        try
                        {
                            Byte[] friendmessage = Encoding.Default.GetBytes("SHOW_POSTz"+names[a]);
                            thisClient.Send(friendmessage);
                            Byte[] response = new Byte[64];
                            thisClient.Receive(response);
                            string received = Encoding.Default.GetString(response);
                        }
                        catch
                        {
                            serverLogs.AppendText("couldn't add friend \n");
                        }
                    }
                }
            }
        }
        private void addfriend(Socket thisClient, string username, string friendname)
        {
            Console.WriteLine("add friend function");
            if (new FileInfo(fpath).Length == 0)//checking if file is empty
            {

                using (StreamWriter file = new StreamWriter(fpath, append: true))//append all posts to a file.
                {
                    file.WriteLine(username + "-" + friendname);
                    file.WriteLine(friendname + "-" + username);
                }
                serverLogs.AppendText(username + " has added " + friendname + " as a friend\n");
                try
                {
                    Byte[] friendmessage = Encoding.Default.GetBytes("SHOW_POSTSYou have added " + friendname + " to your friends list. \n");
                    thisClient.Send(friendmessage);
                    Byte[] response = new Byte[64];
                    thisClient.Receive(response);
                    string received = Encoding.Default.GetString(response);
                }
                catch
                {
                    serverLogs.AppendText("couldn't add friend \n");
                }

            }
            else
            {

                
                string[] lines = File.ReadAllLines(fpath);
                bool found = false;
                bool founduser = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split('-');
                    Console.WriteLine(values[0]);
                                   
                    if (values[0] == username)
                    {
                        founduser = true;
                        Console.WriteLine("Username has entered here");
                        for (int j = 1; j < values.Length; j++)
                        {
                            if (values[j] == friendname)
                            {
                                Console.WriteLine("friend already exists");
                                serverLogs.AppendText(friendname + " already exists in " + username + "'s friend list" + "\n");
                                found = true;
                                try
                                {
                                    Byte[] friendmessage = Encoding.Default.GetBytes("SHOW_POSTS" + friendname + " already exists in your friend list" + "\n");
                                    thisClient.Send(friendmessage);
                                    Byte[] response = new Byte[64];
                                    thisClient.Receive(response);
                                    string received = Encoding.Default.GetString(response);
                                }
                                catch
                                {
                                    serverLogs.AppendText("couldn't add friend \n");
                                }
                            }
                        }
                        if (!found)
                        {
                            Console.WriteLine("adding friend to list");
                            lines[i] = lines[i] + "-" + friendname;
                            File.WriteAllLines(fpath, lines);
                            try
                            {
                                Byte[] friendmessage = Encoding.Default.GetBytes("SHOW_POSTSYou have added " + friendname + " to your friends list. \n");
                                serverLogs.AppendText("Added " + friendname + " to " + username + "'s friend list" + " \n");
                                thisClient.Send(friendmessage);
                                Byte[] response = new Byte[64];
                                thisClient.Receive(response);
                                string received = Encoding.Default.GetString(response);
                            }
                            catch
                            {
                                serverLogs.AppendText("couldn't add friend \n");
                            }
                        }


                        break;
                    }
                   

                }
                if (!founduser)
                {
                    using (StreamWriter file = new StreamWriter(fpath, append: true))//append all posts to a file.
                    {
                        file.WriteLine(username + "-" + friendname);
                        
                    }
                }
                if (!found)
                {
                    Console.WriteLine("Doing reverse friend add");
                    bool exist = false;
                    string[] newlines;
                    newlines = File.ReadAllLines(fpath);
                    for (int b = 0; b < newlines.Length; b++)
                    {
                        string[] newvalues = newlines[b].Split('-');
                        if (newvalues[0] == friendname)
                        {
                            Console.WriteLine("found reverse friend and adding");
                            newlines[b] = newlines[b] + "-" + username;
                            File.WriteAllLines(fpath, newlines);
                            exist = true;

                        }

                    }
                    if (!exist)
                    {
                        using (StreamWriter file = new StreamWriter(fpath, append: true))//append all posts to a file.
                        {
                            Console.WriteLine("adding first instance of reverse friend");
                            file.WriteLine(friendname + "-" + username);
                        }
                    }

                }

                if (!found)
                {
                    int index = 0;
                    bool send = false;
                    try
                    {
                        for (int i = 0; i < clientusernames.ToArray().Length; i++)
                        {
                            if (clientusernames[i] == friendname)
                            {
                                index = i;
                                send = true;
                                break;
                            }
                        }
                        try
                        {
                            if (send)
                            {
                                Byte[] friendmessage = Encoding.Default.GetBytes("SHOW_POSTSYou have been added to " + username + "'s friends list. \n");

                                clientSockets[index].Send(friendmessage);
                                Byte[] response = new Byte[64];
                                thisClient.Receive(response);
                                string received = Encoding.Default.GetString(response);
                            }

                        }
                        catch
                        {
                            serverLogs.AppendText("couldn't add friend \n");
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.StackTrace);
                        Console.WriteLine(exc.Message);
                    }
                }


            }

        }

        private void deletefriend(Socket thisClient, string username, string friendname)
        {
            Console.WriteLine("delete friend function");
            if (new FileInfo(fpath).Length == 0)//checking if file is empty
            {

                
                serverLogs.AppendText("friend list is empty \n");
                try
                {
                    Byte[] friendmessage = Encoding.Default.GetBytes("SHOW_POSTSNo friend list \n");
                    thisClient.Send(friendmessage);
                    Byte[] response = new Byte[64];
                    thisClient.Receive(response);
                    string received = Encoding.Default.GetString(response);
                }
                catch
                {
                    serverLogs.AppendText("couldn't remove friend \n");
                }

            }
            else
            {


                string[] lines = File.ReadAllLines(fpath);
                bool found = false;
                bool founduser = false;
                List<string> readd = new List<string>();
                string linetoadd="";
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split('-');
                    Console.WriteLine(values[0]);

                    if (values[0] == username)
                    {
                        founduser = true;
                        Console.WriteLine("Username has entered here");
                        linetoadd += values[0];
                        for (int j = 1; j < values.Length; j++)
                        {

                            if (values[j] == friendname)
                            {
   
                                Console.WriteLine("friend exists");
                  
                                found = true;
  
                            }
                            else
                            {
                                linetoadd += "-" + values[j];
                            }
                        }
                        if (!found)
                        {
                            Console.WriteLine("friend not in list");
     
                            File.WriteAllLines(fpath, lines);
                            try
                            {
                                Byte[] friendmessage = Encoding.Default.GetBytes("SHOW_POSTSYou have not added " + friendname + " to your friends list. \n");
                                serverLogs.AppendText( friendname  +" not in " + username+"'s friend list \n");
                                thisClient.Send(friendmessage);
                                Byte[] response = new Byte[64];
                                thisClient.Receive(response);
                                string received = Encoding.Default.GetString(response);
                            }
                            catch
                            {
                                serverLogs.AppendText("couldn't add friend \n");
                            }
                        }

                        readd.Add(linetoadd);
                        
                    }
                    else
                    {
                        readd.Add(lines[i]);
                    }


                }
                File.WriteAllLines(fpath, readd.ToArray());
                List<string> newadd = new List<string>();
                string newlinetoadd = "";
                if (found)
                {
                    Console.WriteLine("Doing reverse friend add");
                    
                    string[] newlines;
                    newlines = File.ReadAllLines(fpath);
                    for (int b = 0; b < newlines.Length; b++)
                    {
                        string[] newvalues = newlines[b].Split('-');
                        if (newvalues[0] == friendname)
                        {
                            Console.WriteLine("found reverse friend and adding");
                            newlinetoadd += newvalues[0];
                            for (int a = 1; a < newvalues.Length; a++)
                            {
                                if (newvalues[a] != username)
                                {
                                    newlinetoadd += "-" + newvalues[a];
                                }
                            }

                            newadd.Add(newlinetoadd);

                        }
                        else
                        {
                            newadd.Add(newlines[b]);
                        }

                    }
                    File.WriteAllLines(fpath, newadd.ToArray());
                    try
                    {
                        Byte[] friendmessage = Encoding.Default.GetBytes("SHOW_POSTSYou have removed " + friendname + " from your friends list. \n");
                        serverLogs.AppendText(friendname + " removed from " + username + "'s friend list \n");
                        thisClient.Send(friendmessage);
                        Byte[] response = new Byte[64];
                        thisClient.Receive(response);
                        string received = Encoding.Default.GetString(response);
                    }
                    catch
                    {
                        serverLogs.AppendText("couldn't add friend \n");
                    }
                }
                if (found)
                {
                    int index=0;
                    bool send = false;
                    try
                    {
                        for (int i = 0; i < clientusernames.ToArray().Length; i++)
                        {
                            if (clientusernames[i] == friendname)
                            {
                                index = i;
                                send = true;
                                break;
                            }
                        }
                        try
                        {
                            if (send) {
                                Byte[] friendmessage = Encoding.Default.GetBytes("SHOW_POSTSYou have been removed from " + username + "'s friends list. \n");

                                clientSockets[index].Send(friendmessage);
                                Byte[] response = new Byte[64];
                                thisClient.Receive(response);
                                string received = Encoding.Default.GetString(response);
                            }
                            
                        }
                        catch
                        {
                            serverLogs.AppendText("couldn't add friend \n");
                        }
                    }
                    catch(Exception exc)
                    {
                        Console.WriteLine(exc.StackTrace);
                        Console.WriteLine(exc.Message);
                    }
                }
            }
            
        }
        private void friend_posts(Socket thisClient, string username)
        {
            try {
                string allposts = File.ReadAllText(@"../../posts.txt");
                string pattern = @"\d\d\d\d[-]\d\d[-]\d\d[T]\d\d[:]\d\d[:]\d\d";
                string[] friendlist = File.ReadAllLines(fpath);
                List<string> userfriendlist = new List<string>();
                Regex regex = new Regex(pattern);
                string[] splitted = regex.Split(allposts);
                MatchCollection matches = Regex.Matches(allposts, pattern);
                if (friendlist.Length != 0)
                {
                    Console.WriteLine("Friend list not empty");
                    for (int x = 0; x < friendlist.Length; x++)
                    {
                        string[] splitvals = friendlist[x].Split('-');
                        if (splitvals[0] == username)
                        {
                            for (int y = 1; y < splitvals.Length; y++)
                            {
                                userfriendlist.Add(splitvals[y]);
                            }
                            break;
                        }
                    }

                    for (int s = 0; s < userfriendlist.ToArray().Length; s++)
                    {

                        for (int i = 1; i < splitted.Length; i++)
                        {
                            int beforeid = splitted[i].IndexOf("/", 2);
                            int afterid = splitted[i].IndexOf("/", beforeid + 1);
                            string Name = splitted[i].Substring(2, beforeid - 2);
                            string pID = splitted[i].Substring(beforeid + 1, afterid - beforeid - 1);
                            string post = splitted[i].Substring(afterid + 1, splitted[i].Length - 3 - afterid);
                            if (userfriendlist[s] == Name)
                            {
                                try
                                {
                                    Byte[] buffer1 = Encoding.Default.GetBytes("SHOW_POSTSUsername: " + Name);
                                    try
                                    {
                                        thisClient.Send(buffer1);
                                        Byte[] response = new Byte[64];
                                        thisClient.Receive(response);
                                        string received = Encoding.Default.GetString(response);
                                        Byte[] buffer2 = Encoding.Default.GetBytes("SHOW_POSTSPostID: " + pID);
                                        try
                                        {
                                            thisClient.Send(buffer2);
                                            Byte[] response2 = new Byte[64];
                                            thisClient.Receive(response);
                                            string received2 = Encoding.Default.GetString(response);
                                            Byte[] buffer3 = Encoding.Default.GetBytes("SHOW_POSTSPost: " + post);
                                            try
                                            {
                                                thisClient.Send(buffer3);
                                                Byte[] response3 = new Byte[64];
                                                thisClient.Receive(response);
                                                string received3 = Encoding.Default.GetString(response);
                                                Byte[] buffer4 = Encoding.Default.GetBytes("SHOW_POSTSTime: " + matches[i - 1] + "\n");
                                                try
                                                {
                                                    thisClient.Send(buffer4);
                                                    Byte[] response4 = new Byte[64];
                                                    thisClient.Receive(response);
                                                    string received4 = Encoding.Default.GetString(response);
                                                }
                                                catch
                                                {
                                                    serverLogs.AppendText("There was a problem sending the time.\n");
                                                }
                                            }
                                            catch
                                            {
                                                serverLogs.AppendText("There was a problem sending the post.\n");
                                            }
                                        }
                                        catch
                                        {
                                            serverLogs.AppendText("There was a problem sending the post ID.\n");
                                        }

                                    }
                                    catch
                                    {
                                        serverLogs.AppendText("There was a problem sending the username.\n");
                                    }

                                }
                                catch
                                {
                                    serverLogs.AppendText("There was a problem with the GetBytes function.\n");
                                }
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        Byte[] friendmessage = Encoding.Default.GetBytes("SHOW_POSTSFriends list is empty. \n");
                        serverLogs.AppendText("Friend list is empty \n");
                        thisClient.Send(friendmessage);
                        Byte[] response = new Byte[64];
                        thisClient.Receive(response);
                        string received = Encoding.Default.GetString(response);
                    }
                    catch
                    {
                        serverLogs.AppendText("couldn't send friendlist file error \n");
                    }
                }

                serverLogs.AppendText("Showed all posts for " + username + "'s friendlist.\n");
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
                Console.WriteLine(exc.StackTrace);

            }

        }
        private void user_posts(Socket thisClient, string username)
        {
            string allposts = File.ReadAllText(@"../../posts.txt");
            string pattern = @"\d\d\d\d[-]\d\d[-]\d\d[T]\d\d[:]\d\d[:]\d\d";

            Regex regex = new Regex(pattern);
            string[] splitted = regex.Split(allposts);
            MatchCollection matches = Regex.Matches(allposts, pattern);
            for (int i = 1; i < splitted.Length; i++)
            {
                int beforeid = splitted[i].IndexOf("/", 2);
                int afterid = splitted[i].IndexOf("/", beforeid + 1);
                string Name = splitted[i].Substring(2, beforeid - 2);
                string pID = splitted[i].Substring(beforeid + 1, afterid - beforeid - 1);
                string post = splitted[i].Substring(afterid + 1, splitted[i].Length - 3 - afterid);
                if (username == Name)
                {
                    try
                    {
                        Byte[] buffer1 = Encoding.Default.GetBytes("SHOW_POSTSUsername: " + Name);
                        try
                        {
                            thisClient.Send(buffer1);
                            Byte[] response = new Byte[64];
                            thisClient.Receive(response);
                            string received = Encoding.Default.GetString(response);
                            Byte[] buffer2 = Encoding.Default.GetBytes("SHOW_POSTSPostID: " + pID);
                            try
                            {
                                thisClient.Send(buffer2);
                                Byte[] response2 = new Byte[64];
                                thisClient.Receive(response);
                                string received2 = Encoding.Default.GetString(response);
                                Byte[] buffer3 = Encoding.Default.GetBytes("SHOW_POSTSPost: " + post);
                                try
                                {
                                    thisClient.Send(buffer3);
                                    Byte[] response3 = new Byte[64];
                                    thisClient.Receive(response);
                                    string received3 = Encoding.Default.GetString(response);
                                    Byte[] buffer4 = Encoding.Default.GetBytes("SHOW_POSTSTime: " + matches[i - 1] + "\n");
                                    try
                                    {
                                        thisClient.Send(buffer4);
                                        Byte[] response4 = new Byte[64];
                                        thisClient.Receive(response);
                                        string received4 = Encoding.Default.GetString(response);
                                    }
                                    catch
                                    {
                                        serverLogs.AppendText("There was a problem sending the time.\n");
                                    }
                                }
                                catch
                                {
                                    serverLogs.AppendText("There was a problem sending the post.\n");
                                }
                            }
                            catch
                            {
                                serverLogs.AppendText("There was a problem sending the post ID.\n");
                            }

                        }
                        catch
                        {
                            serverLogs.AppendText("There was a problem sending the username.\n");
                        }

                    }
                    catch
                    {
                        serverLogs.AppendText("There was a problem with the GetBytes function.\n");
                    }
                }
            }
            serverLogs.AppendText("Showed all posts for " + username + ".\n");




        }
        private void deletepost(string postid,string username, Socket thisClient)
        {
            try {
                string allposts = File.ReadAllText(@"../../posts.txt");
                string pattern = @"\d\d\d\d[-]\d\d[-]\d\d[T]\d\d[:]\d\d[:]\d\d";
                string[] postback;
                Regex regex = new Regex(pattern);
                string[] splitted = regex.Split(allposts);
                MatchCollection matches = Regex.Matches(allposts, pattern);
                bool flag = true;
                int toskip = 0;
                List<string> filedata = new List<string>();
                for (int i = 1; i < splitted.Length; i++)
                {
                    int beforeid = splitted[i].IndexOf("/", 2);
                    int afterid = splitted[i].IndexOf("/", beforeid + 1);
                    string Name = splitted[i].Substring(2, beforeid - 2);
                    string pID = splitted[i].Substring(beforeid + 1, afterid - beforeid - 1);
                    string post = splitted[i].Substring(afterid + 1, splitted[i].Length - 4 - afterid);
                    Console.WriteLine("Username:" + username);
                    Console.WriteLine("Name:" + Name);
                    Console.WriteLine("pID:" + pID);
                    Console.WriteLine("post:" + post);
                    Console.WriteLine(splitted[i]);

                    if (username == Name && pID == postid)
                    {
                        flag = false;
                        toskip = i;
                        try
                        {
                            Byte[] buffer1 = Encoding.Default.GetBytes("SHOW_POSTSPost with ID " + postid + " is deleted successfully! \n");
                            try
                            {
                                //List<string> alllines = new List<string>(splitted);
                                //alllines.RemoveAt(i);
                                //splitted = alllines.ToArray();
                                //File.WriteAllLines(@"../../posts.txt", splitted);
                                serverLogs.AppendText("Post with id: " + postid + " is deleted.\n");
                                thisClient.Send(buffer1);
                                Byte[] response = new Byte[64];
                                thisClient.Receive(response);
                                string received = Encoding.Default.GetString(response);

                            }
                            catch
                            {
                                serverLogs.AppendText("There was a problem deleting a post with id: " + postid + ".\n");
                            }

                        }
                        catch
                        {
                            serverLogs.AppendText("There was a problem with the GetBytes function.\n");
                        }
                    }
                    else if (username != Name && pID == postid)
                    {
                        flag = false;

                        filedata.Add(matches[i - 1] + splitted[i]);
                        
                        Byte[] buffer2 = Encoding.Default.GetBytes("SHOW_POSTSPost with ID " + postid + " is not yours! \n");
                        serverLogs.AppendText("Post with id: " + postid + " is not " + username + "'s !\n");
                        thisClient.Send(buffer2);
                        Byte[] aresponse = new Byte[64];
                        thisClient.Receive(aresponse);

                    }
                    else
                    {
                        filedata.Add(matches[i - 1] + splitted[i]);
                    }
                }
                if (flag)
                {
                    try
                    {
                        Byte[] buffer2 = Encoding.Default.GetBytes("SHOW_POSTSPost with ID " + postid + " doesn't exist! \n");
                        serverLogs.AppendText("Post with id: " + postid + " doesn't exist! \n");
                        thisClient.Send(buffer2);
                        Byte[] aresponse = new Byte[64];
                        thisClient.Receive(aresponse);
                    }
                    catch
                    {
                        serverLogs.AppendText("Couldn't send post not found message");
                    }

                }
                else
                {
                    Console.WriteLine("In the overwrite function");
                    string[] toadd = filedata.ToArray();
                    Console.WriteLine(toadd);
                    File.WriteAllLines(@"../../posts.txt", toadd);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                Console.WriteLine(exc.StackTrace);

            }
            
        }
      
        

        private void postToLog(string username, object postID, string post)
        {
            DateTime currentDateTime = DateTime.Now;
            string DT = currentDateTime.ToString("s"); // 2021-11-20T16:54:52
            using (StreamWriter file = new StreamWriter("../../posts.txt", append: true))//append all posts to a file.
            {
                file.WriteLine(DT + " /" + username + "/" + postID.ToString() + "/" + post);
            }
            serverLogs.AppendText(username + " has sent a post:\n" + post + "\n");
        }

        private void allposts(Socket thisClient, string username)
        {
            string allposts = File.ReadAllText(@"../../posts.txt");
            string pattern = @"\d\d\d\d[-]\d\d[-]\d\d[T]\d\d[:]\d\d[:]\d\d";

            Regex regex = new Regex(pattern);
            string[] splitted = regex.Split(allposts);
            MatchCollection matches = Regex.Matches(allposts, pattern);
            for (int i = 1; i < splitted.Length; i++)
            {
                int beforeid = splitted[i].IndexOf("/", 2);
                int afterid = splitted[i].IndexOf("/", beforeid + 1);
                string Name = splitted[i].Substring(2, beforeid - 2);
                string pID = splitted[i].Substring(beforeid + 1, afterid - beforeid - 1);
                string post = splitted[i].Substring(afterid + 1, splitted[i].Length - 2 - afterid);
                if (username != Name)
                {
                    try
                    {
                        Byte[] buffer1 = Encoding.Default.GetBytes("SHOW_POSTSUsername: " + Name);
                        try
                        {
                            thisClient.Send(buffer1);
                            Byte[] response = new Byte[64];
                            thisClient.Receive(response);
                            string received = Encoding.Default.GetString(response);
                            Byte[] buffer2 = Encoding.Default.GetBytes("SHOW_POSTSPostID: " + pID);
                            try
                            {
                                thisClient.Send(buffer2);
                                Byte[] response2 = new Byte[64];
                                thisClient.Receive(response);
                                string received2 = Encoding.Default.GetString(response);
                                Byte[] buffer3 = Encoding.Default.GetBytes("SHOW_POSTSPost: " + post);
                                try
                                {
                                    thisClient.Send(buffer3);
                                    Byte[] response3 = new Byte[64];
                                    thisClient.Receive(response);
                                    string received3 = Encoding.Default.GetString(response);
                                    Byte[] buffer4 = Encoding.Default.GetBytes("SHOW_POSTSTime: " + matches[i - 1] + "\n");
                                    try
                                    {
                                        thisClient.Send(buffer4);
                                        Byte[] response4 = new Byte[64];
                                        thisClient.Receive(response);
                                        string received4 = Encoding.Default.GetString(response);
                                    }
                                    catch
                                    {
                                        serverLogs.AppendText("There was a problem sending the time.\n");
                                    }
                                }
                                catch
                                {
                                    serverLogs.AppendText("There was a problem sending the post.\n");
                                }
                            }
                            catch
                            {
                                serverLogs.AppendText("There was a problem sending the post ID.\n");
                            }

                        }
                        catch
                        {
                            serverLogs.AppendText("There was a problem sending the username.\n");
                        }

                    }
                    catch
                    {
                        serverLogs.AppendText("There was a problem with the GetBytes function.\n");
                    }
                }
            }
            serverLogs.AppendText("Showed all posts for " + username + ".\n");
        }
        private static int CountPost()
        {
            if (!File.Exists(@"../../posts.txt"))//if not generated before.
            {
                File.Create(@"../../posts.txt").Dispose();
            }

            string allPosts = File.ReadAllText(@"../../posts.txt");

            if (allPosts == "")
            {
                return 0;
            }
            //maybe also line by line can be tried.
            string pattern = @"\d\d\d\d[-]\d\d[-]\d\d[T]\d\d[:]\d\d[:]\d\d";

            Regex regex = new Regex(pattern);
            string[] splitted = regex.Split(allPosts);

            int beforeID = splitted[splitted.Length - 1].IndexOf("/", 2);
            int afterID = splitted[splitted.Length - 1].IndexOf("/", beforeID + 1);

            string pID = splitted[splitted.Length - 1].Substring(beforeID + 1, afterID - beforeID - 1);

            return Int32.Parse(pID);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            listening = false;
            terminating = true;
            Environment.Exit(0);
        }

      
    }
}
