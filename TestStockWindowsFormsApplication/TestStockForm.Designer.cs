namespace TestStockWindowsFormsApplication
{
    partial class TestStockForm
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
            this.TopPanel = new System.Windows.Forms.Panel();
            this.InfoListBox = new System.Windows.Forms.ListBox();
            this.PM930Button = new System.Windows.Forms.Button();
            this.PM1030Button = new System.Windows.Forms.Button();
            this.AM1400Button = new System.Windows.Forms.Button();
            this.AM1300Button = new System.Windows.Forms.Button();
            this.NextDayButton = new System.Windows.Forms.Button();
            this.PerDayButton = new System.Windows.Forms.Button();
            this.TestStockButton = new System.Windows.Forms.Button();
            this.DatecomboBox = new System.Windows.Forms.ComboBox();
            this.StartDatePicker = new System.Windows.Forms.DateTimePicker();
            this.ContentPanel = new System.Windows.Forms.Panel();
            this.StockToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.TopPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopPanel
            // 
            this.TopPanel.Controls.Add(this.InfoListBox);
            this.TopPanel.Controls.Add(this.PM930Button);
            this.TopPanel.Controls.Add(this.PM1030Button);
            this.TopPanel.Controls.Add(this.AM1400Button);
            this.TopPanel.Controls.Add(this.AM1300Button);
            this.TopPanel.Controls.Add(this.NextDayButton);
            this.TopPanel.Controls.Add(this.PerDayButton);
            this.TopPanel.Controls.Add(this.TestStockButton);
            this.TopPanel.Controls.Add(this.DatecomboBox);
            this.TopPanel.Controls.Add(this.StartDatePicker);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(895, 26);
            this.TopPanel.TabIndex = 0;
            // 
            // InfoListBox
            // 
            this.InfoListBox.FormattingEnabled = true;
            this.InfoListBox.Location = new System.Drawing.Point(669, 6);
            this.InfoListBox.Name = "InfoListBox";
            this.InfoListBox.Size = new System.Drawing.Size(214, 17);
            this.InfoListBox.TabIndex = 9;
            // 
            // PM930Button
            // 
            this.PM930Button.Location = new System.Drawing.Point(477, 3);
            this.PM930Button.Name = "PM930Button";
            this.PM930Button.Size = new System.Drawing.Size(42, 23);
            this.PM930Button.TabIndex = 8;
            this.PM930Button.Text = "9:30";
            this.PM930Button.UseVisualStyleBackColor = true;
            this.PM930Button.Click += new System.EventHandler(this.PM930Button_Click);
            // 
            // PM1030Button
            // 
            this.PM1030Button.Location = new System.Drawing.Point(525, 3);
            this.PM1030Button.Name = "PM1030Button";
            this.PM1030Button.Size = new System.Drawing.Size(42, 23);
            this.PM1030Button.TabIndex = 7;
            this.PM1030Button.Text = "10:30";
            this.PM1030Button.UseVisualStyleBackColor = true;
            this.PM1030Button.Click += new System.EventHandler(this.PM1030Button_Click);
            // 
            // AM1400Button
            // 
            this.AM1400Button.Location = new System.Drawing.Point(621, 3);
            this.AM1400Button.Name = "AM1400Button";
            this.AM1400Button.Size = new System.Drawing.Size(42, 23);
            this.AM1400Button.TabIndex = 6;
            this.AM1400Button.Text = "14:00";
            this.AM1400Button.UseVisualStyleBackColor = true;
            this.AM1400Button.Click += new System.EventHandler(this.AM1400Button_Click);
            // 
            // AM1300Button
            // 
            this.AM1300Button.Location = new System.Drawing.Point(573, 3);
            this.AM1300Button.Name = "AM1300Button";
            this.AM1300Button.Size = new System.Drawing.Size(42, 23);
            this.AM1300Button.TabIndex = 5;
            this.AM1300Button.Text = "13:00";
            this.AM1300Button.UseVisualStyleBackColor = true;
            this.AM1300Button.Click += new System.EventHandler(this.AM1300Button_Click);
            // 
            // NextDayButton
            // 
            this.NextDayButton.Location = new System.Drawing.Point(429, 3);
            this.NextDayButton.Name = "NextDayButton";
            this.NextDayButton.Size = new System.Drawing.Size(42, 23);
            this.NextDayButton.TabIndex = 4;
            this.NextDayButton.Text = ">>";
            this.NextDayButton.UseVisualStyleBackColor = true;
            this.NextDayButton.Click += new System.EventHandler(this.NextDayButton_Click);
            // 
            // PerDayButton
            // 
            this.PerDayButton.Location = new System.Drawing.Point(381, 1);
            this.PerDayButton.Name = "PerDayButton";
            this.PerDayButton.Size = new System.Drawing.Size(42, 23);
            this.PerDayButton.TabIndex = 3;
            this.PerDayButton.Text = "<<";
            this.PerDayButton.UseVisualStyleBackColor = true;
            this.PerDayButton.Click += new System.EventHandler(this.PerDayButton_Click);
            // 
            // TestStockButton
            // 
            this.TestStockButton.Location = new System.Drawing.Point(333, 1);
            this.TestStockButton.Name = "TestStockButton";
            this.TestStockButton.Size = new System.Drawing.Size(42, 23);
            this.TestStockButton.TabIndex = 2;
            this.TestStockButton.Text = "Run";
            this.TestStockButton.UseVisualStyleBackColor = true;
            this.TestStockButton.Click += new System.EventHandler(this.TestStockButton_Click);
            // 
            // DatecomboBox
            // 
            this.DatecomboBox.FormattingEnabled = true;
            this.DatecomboBox.Location = new System.Drawing.Point(206, 0);
            this.DatecomboBox.Name = "DatecomboBox";
            this.DatecomboBox.Size = new System.Drawing.Size(121, 21);
            this.DatecomboBox.TabIndex = 1;
            // 
            // StartDatePicker
            // 
            this.StartDatePicker.Location = new System.Drawing.Point(0, 0);
            this.StartDatePicker.Name = "StartDatePicker";
            this.StartDatePicker.Size = new System.Drawing.Size(200, 20);
            this.StartDatePicker.TabIndex = 0;
            this.StartDatePicker.ValueChanged += new System.EventHandler(this.StartDatePicker_ValueChanged);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContentPanel.Location = new System.Drawing.Point(0, 26);
            this.ContentPanel.Name = "ContentPanel";
            this.ContentPanel.Size = new System.Drawing.Size(895, 310);
            this.ContentPanel.TabIndex = 1;
            // 
            // TestStockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 336);
            this.Controls.Add(this.ContentPanel);
            this.Controls.Add(this.TopPanel);
            this.Name = "TestStockForm";
            this.Text = "TestStockForm";
            this.Load += new System.EventHandler(this.TestStockForm_Load);
            this.TopPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.ComboBox DatecomboBox;
        private System.Windows.Forms.DateTimePicker StartDatePicker;
        private System.Windows.Forms.Button TestStockButton;
        private System.Windows.Forms.Panel ContentPanel;
        private System.Windows.Forms.Button NextDayButton;
        private System.Windows.Forms.Button PerDayButton;
        private System.Windows.Forms.Button PM930Button;
        private System.Windows.Forms.Button PM1030Button;
        private System.Windows.Forms.Button AM1400Button;
        private System.Windows.Forms.Button AM1300Button;
        private System.Windows.Forms.ToolTip StockToolTip;
        private System.Windows.Forms.ListBox InfoListBox;
    }
}

