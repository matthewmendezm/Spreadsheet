﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spreadsheet;

namespace Spreadsheet
{   

    public partial class ConnectionDialog : Form
    {
        Controller controller = new Controller();

        public ConnectionDialog()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            string host = textBoxHost.Text;
            int port = int.Parse(textBoxPort.Text);
            string userName = textBoxUserName.Text;
            string spreadsheetName = textBoxSpreadsheetName.Text;

            controller.Connect(host, userName, port, spreadsheetName);

            this.Close();
        }
    }
}
