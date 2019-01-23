namespace QnyDownloader
{
    partial class WebBrowserForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebBrowserForm));
            this.TopPanel = new System.Windows.Forms.Panel();
            this.AddressTextBox = new System.Windows.Forms.TextBox();
            this.GoButton = new System.Windows.Forms.Button();
            this.WebViewPanel = new System.Windows.Forms.Panel();
            this.TestWebBrowser = new System.Windows.Forms.WebBrowser();
            this.InfoPanel = new System.Windows.Forms.Panel();
            this.SyncTimer = new System.Windows.Forms.Timer(this.components);
            this.QnyNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TopPanel.SuspendLayout();
            this.WebViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopPanel
            // 
            this.TopPanel.Controls.Add(this.AddressTextBox);
            this.TopPanel.Controls.Add(this.GoButton);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(972, 28);
            this.TopPanel.TabIndex = 0;
            // 
            // AddressTextBox
            // 
            this.AddressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AddressTextBox.Location = new System.Drawing.Point(3, 2);
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.Size = new System.Drawing.Size(913, 20);
            this.AddressTextBox.TabIndex = 1;
            this.AddressTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AddressTextBox_KeyPress);
            // 
            // GoButton
            // 
            this.GoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GoButton.Location = new System.Drawing.Point(920, 0);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(49, 23);
            this.GoButton.TabIndex = 0;
            this.GoButton.Text = "Go";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.GoButton_Click);
            // 
            // WebViewPanel
            // 
            this.WebViewPanel.Controls.Add(this.TestWebBrowser);
            this.WebViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebViewPanel.Location = new System.Drawing.Point(0, 28);
            this.WebViewPanel.Name = "WebViewPanel";
            this.WebViewPanel.Size = new System.Drawing.Size(972, 439);
            this.WebViewPanel.TabIndex = 1;
            // 
            // TestWebBrowser
            // 
            this.TestWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TestWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.TestWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.TestWebBrowser.Name = "TestWebBrowser";
            this.TestWebBrowser.Size = new System.Drawing.Size(972, 439);
            this.TestWebBrowser.TabIndex = 0;
            this.TestWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.TestWebBrowser_DocumentCompleted);
            // 
            // InfoPanel
            // 
            this.InfoPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InfoPanel.Location = new System.Drawing.Point(0, 467);
            this.InfoPanel.Name = "InfoPanel";
            this.InfoPanel.Size = new System.Drawing.Size(972, 42);
            this.InfoPanel.TabIndex = 3;
            // 
            // SyncTimer
            // 
            this.SyncTimer.Tick += new System.EventHandler(this.SyncTimer_Tick);
            // 
            // QnyNotifyIcon
            // 
            this.QnyNotifyIcon.BalloonTipText = "亲农渔数据采集系统";
            this.QnyNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("QnyNotifyIcon.Icon")));
            this.QnyNotifyIcon.Text = "亲农渔";
            this.QnyNotifyIcon.Visible = true;
            this.QnyNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.QnyNotifyIcon_MouseDoubleClick);
            // 
            // WebBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 509);
            this.Controls.Add(this.WebViewPanel);
            this.Controls.Add(this.InfoPanel);
            this.Controls.Add(this.TopPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WebBrowserForm";
            this.Text = "上海亲农渔";
            this.Load += new System.EventHandler(this.WebBrowserForm_Load);
            this.Resize += new System.EventHandler(this.WebBrowserForm_Resize);
            this.TopPanel.ResumeLayout(false);
            this.TopPanel.PerformLayout();
            this.WebViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.TextBox AddressTextBox;
        private System.Windows.Forms.Button GoButton;
        private System.Windows.Forms.Panel WebViewPanel;
        private System.Windows.Forms.WebBrowser TestWebBrowser;
        private System.Windows.Forms.Panel InfoPanel;
        private System.Windows.Forms.Timer SyncTimer;
        private System.Windows.Forms.NotifyIcon QnyNotifyIcon;
    }
}