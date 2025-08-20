namespace Paramecium.Forms
{
    partial class FormAboutParamecium
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAboutParamecium));
            AppNameAndVersionLabel = new Label();
            CopyrightLabel = new Label();
            LicenseText = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // AppNameAndVersionLabel
            // 
            AppNameAndVersionLabel.AutoSize = true;
            AppNameAndVersionLabel.Location = new Point(8, 8);
            AppNameAndVersionLabel.Name = "AppNameAndVersionLabel";
            AppNameAndVersionLabel.Size = new Size(139, 15);
            AppNameAndVersionLabel.TabIndex = 0;
            AppNameAndVersionLabel.Text = "(AppName) (AppVersion)";
            // 
            // CopyrightLabel
            // 
            CopyrightLabel.AutoSize = true;
            CopyrightLabel.Location = new Point(8, 23);
            CopyrightLabel.Name = "CopyrightLabel";
            CopyrightLabel.Size = new Size(150, 15);
            CopyrightLabel.TabIndex = 1;
            CopyrightLabel.Text = "© 2024-2025 Asakai Akachi";
            // 
            // LicenseText
            // 
            LicenseText.AutoSize = true;
            LicenseText.Location = new Point(8, 53);
            LicenseText.Name = "LicenseText";
            LicenseText.Size = new Size(498, 315);
            LicenseText.TabIndex = 2;
            LicenseText.Text = resources.GetString("LicenseText.Text");
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(410, 8);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(96, 96);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // FormAboutParamecium
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(514, 374);
            Controls.Add(pictureBox1);
            Controls.Add(LicenseText);
            Controls.Add(CopyrightLabel);
            Controls.Add(AppNameAndVersionLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormAboutParamecium";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "About Paramecium";
            Shown += FormAboutParamecium_Shown;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label AppNameAndVersionLabel;
        private Label CopyrightLabel;
        private Label LicenseText;
        private PictureBox pictureBox1;
    }
}