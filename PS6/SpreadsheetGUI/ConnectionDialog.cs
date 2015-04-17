using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;

namespace SS
{   

    public partial class ConnectionDialog : Form
    {
        private Controller controller;

        public ConnectionDialog(Controller controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            int port;
            string host = textBoxHost.Text;

            if (textBoxPort.Text == "")
                port = 2000;
            else    
                port = int.Parse(textBoxPort.Text);

            string userName = textBoxUserName.Text;
            string spreadsheetName = textBoxSpreadsheetName.Text;
            /*
            controller.Connect(host, "sysadmin", spreadsheetName, port);

            controller.SendMessage("register " + userName);

            controller.Disconnect();
            */ 

            controller.Connect(host, userName, spreadsheetName, port);

            this.Close();
        }
    }
}
