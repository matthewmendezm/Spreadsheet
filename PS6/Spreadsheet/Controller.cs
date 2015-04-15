using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using CustomNetworking;

namespace SS
{
    /// <summary>
    /// Provides a way for the spreadsheet client to communicate with the server
    /// </summary>
    public class Controller
    {
        // The socket used to communicate with the server
        private StringSocket socket;

        // This is when we receive a message for the messager
        public event Action<String[]> IncomingCellEvent;
        public event Action<String> IncomingErrorEvent;
        public event Action<String> IncomingConnectionEvent;        

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
        public void Connect(string hostname, String name, String sheetName, int port = 2000)
        {            
            TcpClient client = new TcpClient(hostname, port);
            socket = new StringSocket(client.Client, ASCIIEncoding.Default);
            socket.BeginSend("connect " + name + " " + sheetName +  "\n", (e, p) => { }, null);
            socket.BeginReceive(LineReceived, null);
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
                return;
            }

            socket.BeginReceive(LineReceived, null);

            if (s.StartsWith("cell", true, null))
            {
                if (IncomingCellEvent != null)
                {                    
                    temp = s.Substring(5);
                    temp = temp.Trim();
                    String[] subString = temp.Split(seperator, 2);
                    IncomingCellEvent(subString);
                }
            }
                
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
