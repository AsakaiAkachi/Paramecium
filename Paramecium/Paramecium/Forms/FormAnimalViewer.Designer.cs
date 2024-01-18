namespace Paramecium.Forms
{
    partial class FormAnimalViewer
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
            AnimalImage = new PictureBox();
            LabelSelectedParticleId = new Label();
            LabelItemPosition = new Label();
            LabelItemVelocity = new Label();
            LabelItemAngle = new Label();
            LabelItemSatiety = new Label();
            LabelItemColor = new Label();
            LabelPosition = new Label();
            LabelVelocity = new Label();
            LabelAngle = new Label();
            LabelColor = new Label();
            LabelSatiety = new Label();
            LabelGeneration = new Label();
            LabelItemGeneration = new Label();
            LabelAge = new Label();
            LabelItemAge = new Label();
            ((System.ComponentModel.ISupportInitialize)AnimalImage).BeginInit();
            SuspendLayout();
            // 
            // AnimalImage
            // 
            AnimalImage.BackColor = Color.Black;
            AnimalImage.BorderStyle = BorderStyle.Fixed3D;
            AnimalImage.Location = new Point(12, 12);
            AnimalImage.Name = "AnimalImage";
            AnimalImage.Size = new Size(128, 128);
            AnimalImage.TabIndex = 0;
            AnimalImage.TabStop = false;
            // 
            // LabelSelectedParticleId
            // 
            LabelSelectedParticleId.AutoSize = true;
            LabelSelectedParticleId.Location = new Point(146, 125);
            LabelSelectedParticleId.Name = "LabelSelectedParticleId";
            LabelSelectedParticleId.Size = new Size(68, 15);
            LabelSelectedParticleId.TabIndex = 1;
            LabelSelectedParticleId.Text = "#000000000";
            // 
            // LabelItemPosition
            // 
            LabelItemPosition.AutoSize = true;
            LabelItemPosition.Location = new Point(12, 152);
            LabelItemPosition.Name = "LabelItemPosition";
            LabelItemPosition.Size = new Size(50, 15);
            LabelItemPosition.TabIndex = 2;
            LabelItemPosition.Text = "Position";
            // 
            // LabelItemVelocity
            // 
            LabelItemVelocity.AutoSize = true;
            LabelItemVelocity.Location = new Point(12, 176);
            LabelItemVelocity.Name = "LabelItemVelocity";
            LabelItemVelocity.Size = new Size(48, 15);
            LabelItemVelocity.TabIndex = 3;
            LabelItemVelocity.Text = "Velocity";
            // 
            // LabelItemAngle
            // 
            LabelItemAngle.AutoSize = true;
            LabelItemAngle.Location = new Point(12, 200);
            LabelItemAngle.Name = "LabelItemAngle";
            LabelItemAngle.Size = new Size(38, 15);
            LabelItemAngle.TabIndex = 4;
            LabelItemAngle.Text = "Angle";
            // 
            // LabelItemSatiety
            // 
            LabelItemSatiety.AutoSize = true;
            LabelItemSatiety.Location = new Point(12, 248);
            LabelItemSatiety.Name = "LabelItemSatiety";
            LabelItemSatiety.Size = new Size(42, 15);
            LabelItemSatiety.TabIndex = 8;
            LabelItemSatiety.Text = "Satiety";
            // 
            // LabelItemColor
            // 
            LabelItemColor.AutoSize = true;
            LabelItemColor.Location = new Point(12, 224);
            LabelItemColor.Name = "LabelItemColor";
            LabelItemColor.Size = new Size(35, 15);
            LabelItemColor.TabIndex = 7;
            LabelItemColor.Text = "Color";
            // 
            // LabelPosition
            // 
            LabelPosition.AutoSize = true;
            LabelPosition.Location = new Point(90, 152);
            LabelPosition.Name = "LabelPosition";
            LabelPosition.Size = new Size(81, 15);
            LabelPosition.TabIndex = 9;
            LabelPosition.Text = ": (0.000, 0.000)";
            // 
            // LabelVelocity
            // 
            LabelVelocity.AutoSize = true;
            LabelVelocity.Location = new Point(90, 176);
            LabelVelocity.Name = "LabelVelocity";
            LabelVelocity.Size = new Size(81, 15);
            LabelVelocity.TabIndex = 10;
            LabelVelocity.Text = ": (0.000, 0.000)";
            // 
            // LabelAngle
            // 
            LabelAngle.AutoSize = true;
            LabelAngle.Location = new Point(90, 200);
            LabelAngle.Name = "LabelAngle";
            LabelAngle.Size = new Size(24, 15);
            LabelAngle.TabIndex = 11;
            LabelAngle.Text = ": 0°";
            // 
            // LabelColor
            // 
            LabelColor.AutoSize = true;
            LabelColor.Location = new Point(90, 224);
            LabelColor.Name = "LabelColor";
            LabelColor.Size = new Size(51, 15);
            LabelColor.TabIndex = 12;
            LabelColor.Text = ": (0, 0, 0)";
            // 
            // LabelSatiety
            // 
            LabelSatiety.AutoSize = true;
            LabelSatiety.Location = new Point(90, 248);
            LabelSatiety.Name = "LabelSatiety";
            LabelSatiety.Size = new Size(40, 15);
            LabelSatiety.TabIndex = 13;
            LabelSatiety.Text = ": 0.000";
            // 
            // LabelGeneration
            // 
            LabelGeneration.AutoSize = true;
            LabelGeneration.Location = new Point(90, 272);
            LabelGeneration.Name = "LabelGeneration";
            LabelGeneration.Size = new Size(19, 15);
            LabelGeneration.TabIndex = 15;
            LabelGeneration.Text = ": 0";
            // 
            // LabelItemGeneration
            // 
            LabelItemGeneration.AutoSize = true;
            LabelItemGeneration.Location = new Point(12, 272);
            LabelItemGeneration.Name = "LabelItemGeneration";
            LabelItemGeneration.Size = new Size(65, 15);
            LabelItemGeneration.TabIndex = 14;
            LabelItemGeneration.Text = "Generation";
            // 
            // LabelAge
            // 
            LabelAge.AutoSize = true;
            LabelAge.Location = new Point(90, 296);
            LabelAge.Name = "LabelAge";
            LabelAge.Size = new Size(40, 15);
            LabelAge.TabIndex = 17;
            LabelAge.Text = ": 0.000";
            // 
            // LabelItemAge
            // 
            LabelItemAge.AutoSize = true;
            LabelItemAge.Location = new Point(12, 296);
            LabelItemAge.Name = "LabelItemAge";
            LabelItemAge.Size = new Size(28, 15);
            LabelItemAge.TabIndex = 16;
            LabelItemAge.Text = "Age";
            // 
            // FormAnimalViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(278, 326);
            Controls.Add(LabelAge);
            Controls.Add(LabelItemAge);
            Controls.Add(LabelGeneration);
            Controls.Add(LabelItemGeneration);
            Controls.Add(LabelSatiety);
            Controls.Add(LabelColor);
            Controls.Add(LabelAngle);
            Controls.Add(LabelVelocity);
            Controls.Add(LabelPosition);
            Controls.Add(LabelItemSatiety);
            Controls.Add(LabelItemColor);
            Controls.Add(LabelItemAngle);
            Controls.Add(LabelItemVelocity);
            Controls.Add(LabelItemPosition);
            Controls.Add(LabelSelectedParticleId);
            Controls.Add(AnimalImage);
            Name = "FormAnimalViewer";
            Text = "生物ビュー";
            ((System.ComponentModel.ISupportInitialize)AnimalImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox AnimalImage;
        private Label LabelSelectedParticleId;
        private Label LabelItemPosition;
        private Label LabelItemVelocity;
        private Label LabelItemAngle;
        private Label LabelItemSatiety;
        private Label LabelItemColor;
        private Label LabelPosition;
        private Label LabelVelocity;
        private Label LabelAngle;
        private Label LabelColor;
        private Label LabelSatiety;
        private Label LabelGeneration;
        private Label LabelItemGeneration;
        private Label LabelAge;
        private Label LabelItemAge;
    }
}