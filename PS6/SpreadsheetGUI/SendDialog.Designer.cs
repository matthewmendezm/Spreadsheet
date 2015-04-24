namespace SS
{
    partial class SendDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sendMessageTextbox = new System.Windows.Forms.TextBox();
            this.SendMessageButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sendMessageTextbox
            // 
            this.sendMessageTextbox.Location = new System.Drawing.Point(30, 32);
            this.sendMessageTextbox.Name = "sendMessageTextbox";
            this.sendMessageTextbox.Size = new System.Drawing.Size(286, 20);
            this.sendMessageTextbox.TabIndex = 0;
            // 
            // SendMessageButton
            // 
            this.SendMessageButton.Location = new System.Drawing.Point(116, 73);
            this.SendMessageButton.Name = "SendMessageButton";
            this.SendMessageButton.Size = new System.Drawing.Size(118, 23);
            this.SendMessageButton.TabIndex = 1;
            this.SendMessageButton.Text = "Send Message";
            this.SendMessageButton.UseVisualStyleBackColor = true;
            this.SendMessageButton.Click += new System.EventHandler(this.SendMessageButton_Click);
            // 
            // SendDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 108);
            this.Controls.Add(this.SendMessageButton);
            this.Controls.Add(this.sendMessageTextbox);
            this.Name = "SendDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SendDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox sendMessageTextbox;
        private System.Windows.Forms.Button SendMessageButton;
    }
}