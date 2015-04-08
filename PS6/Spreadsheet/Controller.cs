using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using CustomNetworking;

namespace Spreadsheet
{
    /// <summary>
    /// Provides a way for the spreadsheet client to communicate with the server
    /// </summary>
    public class Controller
    {
        // This is when we receive a message for the messager
        public event Action<String> IncomingMessageEvent;

        // The socket used to communicate with the server
        private StringSocket socket;

        public Controller()
        {
            socket = null;
        }

        /// <summary>
        /// Connect to the server at the given hostname and port and with the given name.
        /// </summary>
        /// <param name="hostname">IP addresses for game server</param>
        /// <param name="name">User's name</param>
        /// <param name="port">Connection port</param>
        public void Connect(string hostname, String name, int port, String sheetName)
        {            
            TcpClient client = new TcpClient(hostname, port);
            socket = new StringSocket(client.Client, ASCIIEncoding.Default);

            socket.BeginSend("connect " + name + " " + sheetName +  "\n", (e, p) => { }, null);

            socket.BeginReceive(LineReceived, null);
        }

        private void LineReceived(string s, Exception e, object payload)
        {
            String temp;

            if (IncomingMessageEvent != null)
            {
                temp = s.Trim();
                IncomingMessageEvent(temp);
            }

            socket.BeginReceive(LineReceived, null);
        }

        /// <summary>
        /// Sends a message from the client to the server.  The message is passed in from the GUI
        /// in the line parameter, gets a newline character appended to it, and is sent off.
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
                catch (Exception e)
                {
                    //server died
                    Disconnect();
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
        }
    }
}
