namespace Paramecium.Forms
{
    partial class FormAutosaveSettings
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
            label1 = new Label();
            CheckBoxAutosaveEnable = new CheckBox();
            label2 = new Label();
            NumUpDownAutosaveInterval = new NumericUpDown();
            panel1 = new Panel();
            panel2 = new Panel();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            panel3 = new Panel();
            ButtonCancel = new Button();
            ButtonApplySettings = new Button();
            ((System.ComponentModel.ISupportInitialize)NumUpDownAutosaveInterval).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 8);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 1;
            label1.Text = "Enable";
            // 
            // CheckBoxAutosaveEnable
            // 
            CheckBoxAutosaveEnable.AutoSize = true;
            CheckBoxAutosaveEnable.Location = new Point(227, 8);
            CheckBoxAutosaveEnable.Name = "CheckBoxAutosaveEnable";
            CheckBoxAutosaveEnable.Size = new Size(15, 14);
            CheckBoxAutosaveEnable.TabIndex = 2;
            CheckBoxAutosaveEnable.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 8);
            label2.Name = "label2";
            label2.Size = new Size(46, 15);
            label2.TabIndex = 3;
            label2.Text = "Interval";
            // 
            // NumUpDownAutosaveInterval
            // 
            NumUpDownAutosaveInterval.Location = new Point(100, 4);
            NumUpDownAutosaveInterval.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            NumUpDownAutosaveInterval.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            NumUpDownAutosaveInterval.Name = "NumUpDownAutosaveInterval";
            NumUpDownAutosaveInterval.Size = new Size(100, 23);
            NumUpDownAutosaveInterval.TabIndex = 4;
            NumUpDownAutosaveInterval.Value = new decimal(new int[] { 100000, 0, 0, 0 });
            // 
            // panel1
            // 
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(CheckBoxAutosaveEnable);
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(250, 31);
            panel1.TabIndex = 5;
            // 
            // panel2
            // 
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(NumUpDownAutosaveInterval);
            panel2.Location = new Point(0, 31);
            panel2.Name = "panel2";
            panel2.Size = new Size(250, 31);
            panel2.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(206, 8);
            label3.Name = "label3";
            label3.Size = new Size(35, 15);
            label3.TabIndex = 7;
            label3.Text = "Steps";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(90, 8);
            label4.Name = "label4";
            label4.Size = new Size(10, 15);
            label4.TabIndex = 8;
            label4.Text = ":";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(90, 8);
            label5.Name = "label5";
            label5.Size = new Size(10, 15);
            label5.TabIndex = 9;
            label5.Text = ":";
            // 
            // panel3
            // 
            panel3.Controls.Add(ButtonApplySettings);
            panel3.Controls.Add(ButtonCancel);
            panel3.Location = new Point(0, 62);
            panel3.Name = "panel3";
            panel3.Size = new Size(250, 31);
            panel3.TabIndex = 7;
            // 
            // ButtonCancel
            // 
            ButtonCancel.Location = new Point(146, 4);
            ButtonCancel.Margin = new Padding(0);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(100, 23);
            ButtonCancel.TabIndex = 0;
            ButtonCancel.Text = "Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            ButtonCancel.Click += ButtonCancel_Click;
            // 
            // ButtonApplySettings
            // 
            ButtonApplySettings.Location = new Point(42, 4);
            ButtonApplySettings.Margin = new Padding(0);
            ButtonApplySettings.Name = "ButtonApplySettings";
            ButtonApplySettings.Size = new Size(100, 23);
            ButtonApplySettings.TabIndex = 8;
            ButtonApplySettings.Text = "Apply Settings";
            ButtonApplySettings.UseVisualStyleBackColor = true;
            ButtonApplySettings.Click += ButtonApplySettings_Click;
            // 
            // FormAutosaveSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(250, 93);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormAutosaveSettings";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Autosave Settings";
            ((System.ComponentModel.ISupportInitialize)NumUpDownAutosaveInterval).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Label label1;
        private CheckBox CheckBoxAutosaveEnable;
        private Label label2;
        private NumericUpDown NumUpDownAutosaveInterval;
        private Panel panel1;
        private Panel panel2;
        private Label label4;
        private Label label3;
        private Label label5;
        private Panel panel3;
        private Button ButtonCancel;
        private Button ButtonApplySettings;
    }
}