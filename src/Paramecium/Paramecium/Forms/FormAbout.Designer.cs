namespace Paramecium.Forms
{
    partial class FormAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
            LabelAppNameAndVersion = new Label();
            LabelCopyright = new Label();
            LabelLicense = new Label();
            GroupBoxLicense = new GroupBox();
            ButtonOK = new Button();
            GroupBoxLicense.SuspendLayout();
            SuspendLayout();
            // 
            // LabelAppNameAndVersion
            // 
            LabelAppNameAndVersion.AutoSize = true;
            LabelAppNameAndVersion.Location = new Point(22, 19);
            LabelAppNameAndVersion.Name = "LabelAppNameAndVersion";
            LabelAppNameAndVersion.Size = new Size(138, 15);
            LabelAppNameAndVersion.TabIndex = 0;
            LabelAppNameAndVersion.Text = "Paramecium Version x.y.z";
            // 
            // LabelCopyright
            // 
            LabelCopyright.AutoSize = true;
            LabelCopyright.Location = new Point(22, 44);
            LabelCopyright.Name = "LabelCopyright";
            LabelCopyright.Size = new Size(176, 15);
            LabelCopyright.TabIndex = 1;
            LabelCopyright.Text = "Copyright (c) 2024 AsakaiAkachi";
            // 
            // LabelLicense
            // 
            LabelLicense.AutoSize = true;
            LabelLicense.Location = new Point(6, 19);
            LabelLicense.Name = "LabelLicense";
            LabelLicense.Size = new Size(498, 315);
            LabelLicense.TabIndex = 2;
            LabelLicense.Text = resources.GetString("LabelLicense.Text");
            // 
            // GroupBoxLicense
            // 
            GroupBoxLicense.Controls.Add(LabelLicense);
            GroupBoxLicense.Location = new Point(12, 82);
            GroupBoxLicense.Name = "GroupBoxLicense";
            GroupBoxLicense.Size = new Size(510, 341);
            GroupBoxLicense.TabIndex = 3;
            GroupBoxLicense.TabStop = false;
            GroupBoxLicense.Text = "License";
            // 
            // ButtonOK
            // 
            ButtonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ButtonOK.Location = new Point(422, 434);
            ButtonOK.Name = "ButtonOK";
            ButtonOK.Size = new Size(100, 23);
            ButtonOK.TabIndex = 4;
            ButtonOK.Text = "OK";
            ButtonOK.UseVisualStyleBackColor = true;
            ButtonOK.Click += ButtonOK_Click;
            // 
            // FormAbout
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(534, 469);
            Controls.Add(ButtonOK);
            Controls.Add(GroupBoxLicense);
            Controls.Add(LabelCopyright);
            Controls.Add(LabelAppNameAndVersion);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormAbout";
            Text = "About Paramecium";
            GroupBoxLicense.ResumeLayout(false);
            GroupBoxLicense.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LabelAppNameAndVersion;
        private Label LabelCopyright;
        private Label LabelLicense;
        private GroupBox GroupBoxLicense;
        private Button ButtonOK;
    }
}