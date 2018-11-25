namespace TestWebBrowserWindowsFormsApplication
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
            this.UrlTextBox = new System.Windows.Forms.TextBox();
            this.GoButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.WebBrowserPanel = new System.Windows.Forms.Panel();
            this.ContentTextBox = new System.Windows.Forms.TextBox();
            this.MessageLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UrlTextBox
            // 
            this.UrlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UrlTextBox.Location = new System.Drawing.Point(0, 6);
            this.UrlTextBox.Name = "UrlTextBox";
            this.UrlTextBox.Size = new System.Drawing.Size(834, 20);
            this.UrlTextBox.TabIndex = 0;
            this.UrlTextBox.Text = "https://vss.baobaoaichi.cn/login.html";
            // 
            // GoButton
            // 
            this.GoButton.Location = new System.Drawing.Point(846, 3);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(75, 23);
            this.GoButton.TabIndex = 1;
            this.GoButton.Text = "Go";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.GoButton_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.UrlTextBox);
            this.panel1.Controls.Add(this.GoButton);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(924, 32);
            this.panel1.TabIndex = 2;
            // 
            // WebBrowserPanel
            // 
            this.WebBrowserPanel.Location = new System.Drawing.Point(0, 32);
            this.WebBrowserPanel.Name = "WebBrowserPanel";
            this.WebBrowserPanel.Size = new System.Drawing.Size(585, 496);
            this.WebBrowserPanel.TabIndex = 3;
            // 
            // ContentTextBox
            // 
            this.ContentTextBox.Location = new System.Drawing.Point(591, 61);
            this.ContentTextBox.Multiline = true;
            this.ContentTextBox.Name = "ContentTextBox";
            this.ContentTextBox.Size = new System.Drawing.Size(330, 467);
            this.ContentTextBox.TabIndex = 4;
            // 
            // MessageLabel
            // 
            this.MessageLabel.AutoSize = true;
            this.MessageLabel.Location = new System.Drawing.Point(588, 32);
            this.MessageLabel.Name = "MessageLabel";
            this.MessageLabel.Size = new System.Drawing.Size(0, 13);
            this.MessageLabel.TabIndex = 5;
            // 
            // WebBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 522);
            this.Controls.Add(this.MessageLabel);
            this.Controls.Add(this.ContentTextBox);
            this.Controls.Add(this.WebBrowserPanel);
            this.Controls.Add(this.panel1);
            this.Name = "WebBrowserForm";
            this.Text = "WebBrowserForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebBrowserForm_FormClosing);
            this.Load += new System.EventHandler(this.WebBrowserForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UrlTextBox;
        private System.Windows.Forms.Button GoButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel WebBrowserPanel;
        private System.Windows.Forms.TextBox ContentTextBox;
        private System.Windows.Forms.Label MessageLabel;
    }
}

