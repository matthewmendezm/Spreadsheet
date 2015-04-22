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
            int port;
            string host = textBoxHost.Text;

            if (textBoxPort.Text == "")
                port = 2000;
            else
                port = int.Parse(textBoxPort.Text);

            string userName = textBoxUserName.Text;
            string spreadsheetName = textBoxSpreadsheetName.Text;

            controller.Connect(host, "sysadmin", "", port);

            System.Threading.Thread.Sleep(100);

            controller.SendMessage("register " + userName);

            System.Threading.Thread.Sleep(100);

            controller.Disconnect();

            System.Threading.Thread.Sleep(100);

            controller.Connect(host, userName, spreadsheetName, port);

            this.Close();
        }

        private void textBoxHost_MouseDown(object sender, MouseEventArgs e)
        {
            defaultTextClick(textBoxHost);
        }

        private bool defaultTextClick(TextBox box)
        {
            if (box.ForeColor != SystemColors.WindowText)
            {
                box.ForeColor = SystemColors.WindowText;
                return true;
            }

            return false;
        }

        private void textBoxPort_MouseDown(object sender, MouseEventArgs e)
        {
            defaultTextClick(textBoxPort);
        }

        private void textBoxUserName_MouseDown(object sender, MouseEventArgs e)
        {
            defaultTextClick(textBoxUserName);
        }

        private void textBoxSpreadsheetName_MouseDown(object sender, MouseEventArgs e)
        {
            defaultTextClick(textBoxSpreadsheetName);
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