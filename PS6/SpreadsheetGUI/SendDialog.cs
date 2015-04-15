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
    public partial class SendDialog : Form
    {
        private Controller controller;

        public SendDialog(Controller controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        private void SendMessageButton_Click(object sender, EventArgs e)
        {
            string message = sendMessageTextbox.Text;           

            controller.SendMessage(message);           

            this.Close();
        }
    }
}
