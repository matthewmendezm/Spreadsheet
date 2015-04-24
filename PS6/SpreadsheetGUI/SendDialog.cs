using System;
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