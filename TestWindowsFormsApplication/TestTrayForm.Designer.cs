namespace TestWindowsFormsApplication
{
    partial class TestTrayForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestTrayForm));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MaxWindowsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseWindowsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddressTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.GoButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.TestWebBrowser = new System.Windows.Forms.WebBrowser();
            this.contextMenuStripTray.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStripTray;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "notifyIcon";
            this.notifyIcon.Visible = true;
            this.notifyIcon.Click += new System.EventHandler(this.notifyIcon_Click);
            // 
            // contextMenuStripTray
            // 
            this.contextMenuStripTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MaxWindowsMenuItem,
            this.CloseWindowsMenuItem});
            this.contextMenuStripTray.Name = "contextMenuStripTray";
            this.contextMenuStripTray.Size = new System.Drawing.Size(104, 48);
            // 
            // MaxWindowsMenuItem
            // 
            this.MaxWindowsMenuItem.Name = "MaxWindowsMenuItem";
            this.MaxWindowsMenuItem.Size = new System.Drawing.Size(103, 22);
            this.MaxWindowsMenuItem.Text = "Max";
            this.MaxWindowsMenuItem.ToolTipText = "Max the window";
            this.MaxWindowsMenuItem.Click += new System.EventHandler(this.MaxWindowsMenuItem_Click);
            // 
            // CloseWindowsMenuItem
            // 
            this.CloseWindowsMenuItem.Name = "CloseWindowsMenuItem";
            this.CloseWindowsMenuItem.Size = new System.Drawing.Size(103, 22);
            this.CloseWindowsMenuItem.Text = "Close";
            this.CloseWindowsMenuItem.ToolTipText = "Close the application";
            this.CloseWindowsMenuItem.Click += new System.EventHandler(this.CloseWindowsMenuItem_Click);
            // 
            // AddressTextBox
            // 
            this.AddressTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddressTextBox.Location = new System.Drawing.Point(0, 0);
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.Size = new System.Drawing.Size(574, 20);
            this.AddressTextBox.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.AddressTextBox);
            this.panel1.Controls.Add(this.GoButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(649, 26);
            this.panel1.TabIndex = 3;
            // 
            // GoButton
            // 
            this.GoButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.GoButton.Location = new System.Drawing.Point(574, 0);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(75, 26);
            this.GoButton.TabIndex = 2;
            this.GoButton.Text = "Go";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.GoButton_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.TestWebBrowser);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(649, 417);
            this.panel2.TabIndex = 4;
            // 
            // TestWebBrowser
            // 
            this.TestWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TestWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.TestWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.TestWebBrowser.Name = "TestWebBrowser";
            this.TestWebBrowser.Size = new System.Drawing.Size(649, 417);
            this.TestWebBrowser.TabIndex = 0;
            this.TestWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.TestWebBrowser_DocumentCompleted);
            // 
            // TestTrayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 443);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "TestTrayForm";
            this.Text = "TestTray";
            this.Load += new System.EventHandler(this.TestTrayForm_Load);
            this.SizeChanged += new System.EventHandler(this.TestTrayForm_SizeChanged);
            this.contextMenuStripTray.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTray;
        private System.Windows.Forms.ToolStripMenuItem MaxWindowsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CloseWindowsMenuItem;
        private System.Windows.Forms.TextBox AddressTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button GoButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.WebBrowser TestWebBrowser;
    }
}

