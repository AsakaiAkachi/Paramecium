namespace Paramecium.Forms
{
    partial class FormAutosaveSetting
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
            checkboxEnableAutosave = new CheckBox();
            inputAutoSaveInterval = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            buttonApply = new Button();
            buttonCancel = new Button();
            labelCurrentAutoSaveInterval = new Label();
            ((System.ComponentModel.ISupportInitialize)inputAutoSaveInterval).BeginInit();
            SuspendLayout();
            // 
            // checkboxEnableAutosave
            // 
            checkboxEnableAutosave.AutoSize = true;
            checkboxEnableAutosave.Location = new Point(12, 12);
            checkboxEnableAutosave.Name = "checkboxEnableAutosave";
            checkboxEnableAutosave.Size = new Size(113, 19);
            checkboxEnableAutosave.TabIndex = 0;
            checkboxEnableAutosave.Text = "Enable Autosave";
            checkboxEnableAutosave.UseVisualStyleBackColor = true;
            // 
            // inputAutoSaveInterval
            // 
            inputAutoSaveInterval.Location = new Point(126, 41);
            inputAutoSaveInterval.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            inputAutoSaveInterval.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            inputAutoSaveInterval.Name = "inputAutoSaveInterval";
            inputAutoSaveInterval.Size = new Size(90, 23);
            inputAutoSaveInterval.TabIndex = 1;
            inputAutoSaveInterval.Value = new decimal(new int[] { 100000, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 43);
            label1.Name = "label1";
            label1.Size = new Size(108, 15);
            label1.TabIndex = 2;
            label1.Text = "Auto Save Interval :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(218, 43);
            label2.Name = "label2";
            label2.Size = new Size(34, 15);
            label2.TabIndex = 3;
            label2.Text = "steps";
            // 
            // buttonApply
            // 
            buttonApply.Location = new Point(12, 121);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(80, 24);
            buttonApply.TabIndex = 40;
            buttonApply.Text = "Apply";
            buttonApply.UseVisualStyleBackColor = true;
            buttonApply.Click += buttonApply_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(98, 121);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(80, 24);
            buttonCancel.TabIndex = 41;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // labelCurrentAutoSaveInterval
            // 
            labelCurrentAutoSaveInterval.AutoSize = true;
            labelCurrentAutoSaveInterval.Location = new Point(12, 76);
            labelCurrentAutoSaveInterval.Name = "labelCurrentAutoSaveInterval";
            labelCurrentAutoSaveInterval.Size = new Size(254, 15);
            labelCurrentAutoSaveInterval.TabIndex = 42;
            labelCurrentAutoSaveInterval.Text = "Current Auto Save Interval : Auto Save Disabled";
            // 
            // FormAutosaveSetting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(279, 157);
            Controls.Add(labelCurrentAutoSaveInterval);
            Controls.Add(buttonCancel);
            Controls.Add(buttonApply);
            Controls.Add(label1);
            Controls.Add(inputAutoSaveInterval);
            Controls.Add(checkboxEnableAutosave);
            Controls.Add(label2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormAutosaveSetting";
            ShowIcon = false;
            Text = "Auto Save Settings";
            FormClosing += FormAutosaveSetting_FormClosing;
            ((System.ComponentModel.ISupportInitialize)inputAutoSaveInterval).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkboxEnableAutosave;
        private NumericUpDown inputAutoSaveInterval;
        private Label label1;
        private Label label2;
        private Button buttonApply;
        private Button buttonCancel;
        private Label labelCurrentAutoSaveInterval;
    }
}