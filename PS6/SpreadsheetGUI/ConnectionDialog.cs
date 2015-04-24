using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            labelConnectionError.Text = "";
            int port;
            string host = textBoxHost.Text;

            if (textBoxPort.Text == "")
                port = 2000;
            else
                port = int.Parse(textBoxPort.Text);

            string userName = textBoxUserName.Text;
            string spreadsheetName = textBoxSpreadsheetName.Text;

            if (userName == "" || spreadsheetName == "")
            {
                labelConnectionError.Text = "Must provide user name and spreadsheet name";
                return;
            }

            if (userName.Contains(" ") || spreadsheetName.Contains(" "))
            {
                labelConnectionError.Text = "User name and spreadsheet name may not contain spaces";
                return;
            }

            try
            {
                controller.Connect(host, "sysadmin", spreadsheetName, port);
            }
            catch (Exception)
            {
                // print an error message
                labelConnectionError.Text = "Cannot connect to server";
                controller.connected = false;
                return;
            }

            System.Threading.Thread.Sleep(100);

            controller.SendMessage("register " + userName);

            System.Threading.Thread.Sleep(100);

            controller.Disconnect();

            System.Threading.Thread.Sleep(100);

            controller.Connect(host, userName, spreadsheetName, port);

            this.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    buttonConnect_Click(new Object(), new EventArgs());
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}