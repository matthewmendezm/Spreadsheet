namespace SS
{
    partial class ConnectionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionDialog));
            this.textBoxHost = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSpreadsheetName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxHost
            // 
            this.textBoxHost.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.textBoxHost.Location = new System.Drawing.Point(164, 15);
            this.textBoxHost.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.Size = new System.Drawing.Size(287, 22);
            this.textBoxHost.TabIndex = 0;
            this.textBoxHost.Text = "lab1-1.eng.utah.edu";
            this.textBoxHost.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxHost_MouseDown);
            // 
            // textBoxPort
            // 
            this.textBoxPort.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.textBoxPort.Location = new System.Drawing.Point(164, 47);
            this.textBoxPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(287, 22);
            this.textBoxPort.TabIndex = 1;
            this.textBoxPort.Text = "2112";
            this.textBoxPort.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxPort_MouseDown);
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.textBoxUserName.Location = new System.Drawing.Point(164, 79);
            this.textBoxUserName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(287, 22);
            this.textBoxUserName.TabIndex = 2;
            this.textBoxUserName.Text = "sysadmin";
            this.textBoxUserName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxUserName_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Host";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 54);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 86);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "User Name";
            // 
            // textBoxSpreadsheetName
            // 
            this.textBoxSpreadsheetName.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.textBoxSpreadsheetName.Location = new System.Drawing.Point(164, 112);
            this.textBoxSpreadsheetName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxSpreadsheetName.Name = "textBoxSpreadsheetName";
            this.textBoxSpreadsheetName.Size = new System.Drawing.Size(287, 22);
            this.textBoxSpreadsheetName.TabIndex = 6;
            this.textBoxSpreadsheetName.Text = "MangoSheet";
            this.textBoxSpreadsheetName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBoxSpreadsheetName_MouseDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 119);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(130, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Spreadsheet Name";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(164, 153);
            this.buttonConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(167, 39);
            this.buttonConnect.TabIndex = 8;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // ConnectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 207);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxSpreadsheetName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxUserName);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.textBoxHost);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ConnectionDialog";
            this.Text = "Connect to a Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxHost;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSpreadsheetName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonConnect;
    }
}