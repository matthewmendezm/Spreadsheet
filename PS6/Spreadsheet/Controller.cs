using CustomNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SS
{
    /// <summary>
    /// Provides a way for the spreadsheet client to communicate with the server
    /// </summary>
    public class Controller
    {   
        /// <summary>
        /// this is a bool
        /// </summary>
        public bool connected = false;

        // The socket used to communicate with the server
        private StringSocket socket;

        // This is when we receive a message for the messenger
        /// <summary>
        /// message to update a cell has been received
        /// </summary>
        public event Action<String[]> IncomingCellEvent;

        /// <summary>
        /// some communication protocol error as occurred
        /// </summary>
        public event Action<String[]> IncomingErrorEvent;

        /// <summary>
        /// confirmation of connection to server
        /// </summary>
        public event Action<String[]> IncomingConnectionEvent;

        /// <summary>
        /// Used for informing the client that we have disconnected
        /// </summary>
        public event Action<String> IncomingDisconnectEvent;

        private string spreadsheetName;
        private string userName;

        /// <summary>
        /// Constructor
        /// </summary>
        public Controller()
        {
            socket = null;
        }

        /// <summary>
        /// Connect to the server at the given host name and port and with the given name.
        /// </summary>
        /// <param name="hostname">IP addresses for game server</param>
        /// <param name="name">User's name</param>
        /// <param name="sheetName">The name of the sheet</param>
        /// <param name="port">Connection port</param>
        public void Connect(string hostname, String name, String sheetName, int port = 2000)
        {
            //try
            //{
                TcpClient client = new TcpClient(hostname, port);
                socket = new StringSocket(client.Client, ASCIIEncoding.Default);
                socket.BeginSend("connect " + name + " " + sheetName + "\n", (e, p) => { socket.BeginReceive(LineReceived, null); }, null);
                spreadsheetName = sheetName;
                userName = name;
            //}
            //catch (Exception)
            //{

            //}
        }

        private void LineReceived(string s, Exception e, object payload)
        {
            String temp;
            char[] seperator = { ' ' };
            if (s == null)
            {
                //disconnected socket. do something
                //ServerEvent("The server has crashed");
                Disconnect();
                IncomingDisconnectEvent("");
                return;
            }

            //socket.BeginReceive(LineReceived, null);

            if (s.StartsWith("cell", true, null))
            {
                if (IncomingCellEvent != null)
                {
                    temp = s.Substring(5);
                    temp = temp.Trim();
                    String[] subString = new String[2];
                    String[] tempSubString = temp.Split(seperator, 2);

                    // if the content of the cell is null, set it to a null string instead
                    if (tempSubString.Length == 1)
                    {
                        subString[0] = tempSubString[0];
                        subString[1] = "";
                    }
                    else
                    {
                        subString[0] = tempSubString[0];
                        subString[1] = tempSubString[1];
                    }

                    IncomingCellEvent(subString);
                }
            }
            else if (s.StartsWith("connected", true, null))
            {
                if (IncomingConnectionEvent != null)
                {
                    temp = s.Substring(10);
                    temp = temp.Trim();
                    String[] subString = new String[3];
                    subString[0] = temp;
                    subString[1] = spreadsheetName;
                    subString[2] = userName;
                    IncomingConnectionEvent(subString);
                    connected = true;
                }
            }
            else if (s.StartsWith("error", true, null))
            {
                if (IncomingErrorEvent != null)
                {
                    temp = s.Substring(6);
                    temp = temp.Trim();
                    String[] subString = temp.Split(seperator, 2);
                    IncomingErrorEvent(subString);
                }
            }

            socket.BeginReceive(LineReceived, null);
        }

        /// <summary>
        /// Sends a message from the client to the server. The message is passed in from the GUI in the line parameter, gets
        /// a newline character appended to it, and is sent off.
        /// </summary>
        /// <param name="line">Message from GUI</param>
        public void SendMessage(String line)
        {
            if (socket != null)
            {
                try
                {
                    socket.BeginSend(line + "\n", (e, p) => { }, null);
                }
                catch (Exception)
                {
                    //server died
                    Disconnect();
                    IncomingDisconnectEvent("");
                    //ServerEvent("The server has crashed");
                }
            }
        }

        /// <summary>
        /// Helper method used to disconnect clients under any condition that ends the game
        /// </summary>
        public void Disconnect()
        {
            socket.Close();
            IncomingDisconnectEvent("");
            connected = false;
        }
    }
}