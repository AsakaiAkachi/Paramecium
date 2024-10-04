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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutosaveSettings));
            CheckBoxAutosaveEnable = new CheckBox();
            NumericUpDownAutosaveInterval = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            ButtonCancel = new Button();
            ButtonApply = new Button();
            ((System.ComponentModel.ISupportInitialize)NumericUpDownAutosaveInterval).BeginInit();
            SuspendLayout();
            // 
            // CheckBoxAutosaveEnable
            // 
            CheckBoxAutosaveEnable.AutoSize = true;
            CheckBoxAutosaveEnable.Location = new Point(250, 15);
            CheckBoxAutosaveEnable.Name = "CheckBoxAutosaveEnable";
            CheckBoxAutosaveEnable.Size = new Size(15, 14);
            CheckBoxAutosaveEnable.TabIndex = 0;
            CheckBoxAutosaveEnable.UseVisualStyleBackColor = true;
            // 
            // NumericUpDownAutosaveInterval
            // 
            NumericUpDownAutosaveInterval.Location = new Point(115, 41);
            NumericUpDownAutosaveInterval.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            NumericUpDownAutosaveInterval.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            NumericUpDownAutosaveInterval.Name = "NumericUpDownAutosaveInterval";
            NumericUpDownAutosaveInterval.Size = new Size(150, 23);
            NumericUpDownAutosaveInterval.TabIndex = 1;
            NumericUpDownAutosaveInterval.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 14);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 2;
            label1.Text = "Enable";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 43);
            label2.Name = "label2";
            label2.Size = new Size(85, 15);
            label2.TabIndex = 3;
            label2.Text = "Interval (Steps)";
            // 
            // ButtonCancel
            // 
            ButtonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ButtonCancel.Location = new Point(166, 76);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(100, 23);
            ButtonCancel.TabIndex = 99;
            ButtonCancel.Text = "Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            ButtonCancel.Click += ButtonCancel_Click;
            // 
            // ButtonApply
            // 
            ButtonApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ButtonApply.Location = new Point(60, 76);
            ButtonApply.Name = "ButtonApply";
            ButtonApply.Size = new Size(100, 23);
            ButtonApply.TabIndex = 98;
            ButtonApply.Text = "Apply Settings";
            ButtonApply.UseVisualStyleBackColor = true;
            ButtonApply.Click += ButtonApply_Click;
            // 
            // FormAutosaveSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(278, 111);
            Controls.Add(ButtonCancel);
            Controls.Add(ButtonApply);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(NumericUpDownAutosaveInterval);
            Controls.Add(CheckBoxAutosaveEnable);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormAutosaveSettings";
            Text = "Autosave Settings";
            Shown += FormAutosaveSettings_Shown;
            ((System.ComponentModel.ISupportInitialize)NumericUpDownAutosaveInterval).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox CheckBoxAutosaveEnable;
        private NumericUpDown NumericUpDownAutosaveInterval;
        private Label label1;
        private Label label2;
        private Button ButtonCancel;
        private Button ButtonApply;
    }
}