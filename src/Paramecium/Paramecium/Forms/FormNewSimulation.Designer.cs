namespace Paramecium.Forms
{
    partial class FormNewSimulation
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
            SoupSizeX = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            SoupSizeY = new NumericUpDown();
            label3 = new Label();
            label4 = new Label();
            label6 = new Label();
            WallPerlinNoiseX = new NumericUpDown();
            WallPerlinNoiseY = new NumericUpDown();
            label7 = new Label();
            WallPerlinNoiseZ = new NumericUpDown();
            label5 = new Label();
            WallPerlinNoiseScale = new NumericUpDown();
            label8 = new Label();
            WallPerlinNoiseOctave = new NumericUpDown();
            label9 = new Label();
            WallThickness = new NumericUpDown();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            ParticleCountLimit = new NumericUpDown();
            label13 = new Label();
            label14 = new Label();
            TotalBiomassAmount = new NumericUpDown();
            label15 = new Label();
            label16 = new Label();
            InitialAnimalCount = new NumericUpDown();
            WallPerlinNoisePositionRandomize = new CheckBox();
            ButtonOK = new Button();
            ButtonCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)SoupSizeX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SoupSizeY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)WallPerlinNoiseX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)WallPerlinNoiseY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)WallPerlinNoiseZ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)WallPerlinNoiseScale).BeginInit();
            ((System.ComponentModel.ISupportInitialize)WallPerlinNoiseOctave).BeginInit();
            ((System.ComponentModel.ISupportInitialize)WallThickness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ParticleCountLimit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TotalBiomassAmount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)InitialAnimalCount).BeginInit();
            SuspendLayout();
            // 
            // SoupSizeX
            // 
            SoupSizeX.Location = new Point(223, 30);
            SoupSizeX.Maximum = new decimal(new int[] { 2048, 0, 0, 0 });
            SoupSizeX.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            SoupSizeX.Name = "SoupSizeX";
            SoupSizeX.Size = new Size(48, 23);
            SoupSizeX.TabIndex = 0;
            SoupSizeX.Value = new decimal(new int[] { 512, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(192, 32);
            label1.Name = "label1";
            label1.Size = new Size(25, 15);
            label1.TabIndex = 1;
            label1.Text = ": 横";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(277, 32);
            label2.Name = "label2";
            label2.Size = new Size(28, 15);
            label2.TabIndex = 2;
            label2.Text = "x 縦";
            // 
            // SoupSizeY
            // 
            SoupSizeY.Location = new Point(311, 30);
            SoupSizeY.Maximum = new decimal(new int[] { 2048, 0, 0, 0 });
            SoupSizeY.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            SoupSizeY.Name = "SoupSizeY";
            SoupSizeY.Size = new Size(48, 23);
            SoupSizeY.TabIndex = 3;
            SoupSizeY.Value = new decimal(new int[] { 256, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(24, 32);
            label3.Name = "label3";
            label3.Size = new Size(60, 15);
            label3.TabIndex = 4;
            label3.Text = "スープサイズ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(24, 71);
            label4.Name = "label4";
            label4.Size = new Size(103, 15);
            label4.TabIndex = 9;
            label4.Text = "壁生成パーリンノイズ";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(192, 71);
            label6.Name = "label6";
            label6.Size = new Size(20, 15);
            label6.TabIndex = 6;
            label6.Text = ": X";
            // 
            // WallPerlinNoiseX
            // 
            WallPerlinNoiseX.DecimalPlaces = 4;
            WallPerlinNoiseX.Location = new Point(218, 69);
            WallPerlinNoiseX.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            WallPerlinNoiseX.Minimum = new decimal(new int[] { 256, 0, 0, int.MinValue });
            WallPerlinNoiseX.Name = "WallPerlinNoiseX";
            WallPerlinNoiseX.Size = new Size(65, 23);
            WallPerlinNoiseX.TabIndex = 5;
            // 
            // WallPerlinNoiseY
            // 
            WallPerlinNoiseY.DecimalPlaces = 4;
            WallPerlinNoiseY.Location = new Point(309, 69);
            WallPerlinNoiseY.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            WallPerlinNoiseY.Minimum = new decimal(new int[] { 256, 0, 0, int.MinValue });
            WallPerlinNoiseY.Name = "WallPerlinNoiseY";
            WallPerlinNoiseY.Size = new Size(65, 23);
            WallPerlinNoiseY.TabIndex = 13;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(289, 71);
            label7.Name = "label7";
            label7.Size = new Size(14, 15);
            label7.TabIndex = 12;
            label7.Text = "Y";
            // 
            // WallPerlinNoiseZ
            // 
            WallPerlinNoiseZ.DecimalPlaces = 4;
            WallPerlinNoiseZ.Location = new Point(400, 69);
            WallPerlinNoiseZ.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            WallPerlinNoiseZ.Minimum = new decimal(new int[] { 256, 0, 0, int.MinValue });
            WallPerlinNoiseZ.Name = "WallPerlinNoiseZ";
            WallPerlinNoiseZ.Size = new Size(65, 23);
            WallPerlinNoiseZ.TabIndex = 15;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(380, 71);
            label5.Name = "label5";
            label5.Size = new Size(14, 15);
            label5.TabIndex = 14;
            label5.Text = "Z";
            // 
            // WallPerlinNoiseScale
            // 
            WallPerlinNoiseScale.DecimalPlaces = 4;
            WallPerlinNoiseScale.Location = new Point(245, 98);
            WallPerlinNoiseScale.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            WallPerlinNoiseScale.Name = "WallPerlinNoiseScale";
            WallPerlinNoiseScale.Size = new Size(48, 23);
            WallPerlinNoiseScale.TabIndex = 17;
            WallPerlinNoiseScale.Value = new decimal(new int[] { 3, 0, 0, 131072 });
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(196, 100);
            label8.Name = "label8";
            label8.Size = new Size(43, 15);
            label8.TabIndex = 16;
            label8.Text = "スケール";
            // 
            // WallPerlinNoiseOctave
            // 
            WallPerlinNoiseOctave.Location = new Point(356, 98);
            WallPerlinNoiseOctave.Maximum = new decimal(new int[] { 16, 0, 0, 0 });
            WallPerlinNoiseOctave.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            WallPerlinNoiseOctave.Name = "WallPerlinNoiseOctave";
            WallPerlinNoiseOctave.Size = new Size(48, 23);
            WallPerlinNoiseOctave.TabIndex = 19;
            WallPerlinNoiseOctave.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(299, 100);
            label9.Name = "label9";
            label9.Size = new Size(51, 15);
            label9.TabIndex = 18;
            label9.Text = "オクターブ";
            // 
            // WallThickness
            // 
            WallThickness.DecimalPlaces = 5;
            WallThickness.Location = new Point(254, 127);
            WallThickness.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            WallThickness.Name = "WallThickness";
            WallThickness.Size = new Size(60, 23);
            WallThickness.TabIndex = 21;
            WallThickness.Value = new decimal(new int[] { 85, 0, 0, 262144 });
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(196, 129);
            label10.Name = "label10";
            label10.Size = new Size(52, 15);
            label10.TabIndex = 20;
            label10.Text = "壁の厚み";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(24, 168);
            label11.Name = "label11";
            label11.Size = new Size(96, 15);
            label11.TabIndex = 24;
            label11.Text = "パーティクル数上限";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(192, 168);
            label12.Name = "label12";
            label12.Size = new Size(13, 15);
            label12.TabIndex = 23;
            label12.Text = ": ";
            // 
            // ParticleCountLimit
            // 
            ParticleCountLimit.Location = new Point(211, 166);
            ParticleCountLimit.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            ParticleCountLimit.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            ParticleCountLimit.Name = "ParticleCountLimit";
            ParticleCountLimit.Size = new Size(85, 23);
            ParticleCountLimit.TabIndex = 22;
            ParticleCountLimit.Value = new decimal(new int[] { 65536, 0, 0, 0 });
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(24, 207);
            label13.Name = "label13";
            label13.Size = new Size(77, 15);
            label13.TabIndex = 27;
            label13.Text = "バイオマス総量";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(192, 207);
            label14.Name = "label14";
            label14.Size = new Size(13, 15);
            label14.TabIndex = 26;
            label14.Text = ": ";
            // 
            // TotalBiomassAmount
            // 
            TotalBiomassAmount.DecimalPlaces = 3;
            TotalBiomassAmount.Location = new Point(211, 205);
            TotalBiomassAmount.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            TotalBiomassAmount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            TotalBiomassAmount.Name = "TotalBiomassAmount";
            TotalBiomassAmount.Size = new Size(85, 23);
            TotalBiomassAmount.TabIndex = 25;
            TotalBiomassAmount.Value = new decimal(new int[] { 262144, 0, 0, 0 });
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(24, 246);
            label15.Name = "label15";
            label15.Size = new Size(67, 15);
            label15.TabIndex = 30;
            label15.Text = "初期動物数";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(192, 246);
            label16.Name = "label16";
            label16.Size = new Size(13, 15);
            label16.TabIndex = 29;
            label16.Text = ": ";
            // 
            // InitialAnimalCount
            // 
            InitialAnimalCount.Location = new Point(211, 244);
            InitialAnimalCount.Maximum = new decimal(new int[] { 2048, 0, 0, 0 });
            InitialAnimalCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            InitialAnimalCount.Name = "InitialAnimalCount";
            InitialAnimalCount.Size = new Size(48, 23);
            InitialAnimalCount.TabIndex = 28;
            InitialAnimalCount.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // WallPerlinNoisePositionRandomize
            // 
            WallPerlinNoisePositionRandomize.AutoSize = true;
            WallPerlinNoisePositionRandomize.Checked = true;
            WallPerlinNoisePositionRandomize.CheckState = CheckState.Checked;
            WallPerlinNoisePositionRandomize.Location = new Point(329, 128);
            WallPerlinNoisePositionRandomize.Name = "WallPerlinNoisePositionRandomize";
            WallPerlinNoisePositionRandomize.Size = new Size(104, 19);
            WallPerlinNoisePositionRandomize.TabIndex = 31;
            WallPerlinNoisePositionRandomize.Text = "座標をランダム化";
            // 
            // ButtonOK
            // 
            ButtonOK.Location = new Point(127, 298);
            ButtonOK.Name = "ButtonOK";
            ButtonOK.Size = new Size(90, 30);
            ButtonOK.TabIndex = 32;
            ButtonOK.Text = "スープを作成";
            ButtonOK.UseVisualStyleBackColor = true;
            ButtonOK.Click += ButtonOK_Click;
            // 
            // ButtonCancel
            // 
            ButtonCancel.Location = new Point(284, 298);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(90, 30);
            ButtonCancel.TabIndex = 33;
            ButtonCancel.Text = "キャンセル";
            ButtonCancel.UseVisualStyleBackColor = true;
            ButtonCancel.Click += ButtonCancel_Click;
            // 
            // FormNewSimulation
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(503, 354);
            Controls.Add(ButtonCancel);
            Controls.Add(ButtonOK);
            Controls.Add(WallPerlinNoisePositionRandomize);
            Controls.Add(label15);
            Controls.Add(label16);
            Controls.Add(InitialAnimalCount);
            Controls.Add(label13);
            Controls.Add(label14);
            Controls.Add(TotalBiomassAmount);
            Controls.Add(label11);
            Controls.Add(label12);
            Controls.Add(ParticleCountLimit);
            Controls.Add(WallThickness);
            Controls.Add(label10);
            Controls.Add(WallPerlinNoiseOctave);
            Controls.Add(label9);
            Controls.Add(WallPerlinNoiseScale);
            Controls.Add(label8);
            Controls.Add(WallPerlinNoiseZ);
            Controls.Add(label5);
            Controls.Add(WallPerlinNoiseY);
            Controls.Add(label7);
            Controls.Add(label4);
            Controls.Add(label6);
            Controls.Add(WallPerlinNoiseX);
            Controls.Add(label3);
            Controls.Add(SoupSizeY);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(SoupSizeX);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormNewSimulation";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "スープを新規作成";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize)SoupSizeX).EndInit();
            ((System.ComponentModel.ISupportInitialize)SoupSizeY).EndInit();
            ((System.ComponentModel.ISupportInitialize)WallPerlinNoiseX).EndInit();
            ((System.ComponentModel.ISupportInitialize)WallPerlinNoiseY).EndInit();
            ((System.ComponentModel.ISupportInitialize)WallPerlinNoiseZ).EndInit();
            ((System.ComponentModel.ISupportInitialize)WallPerlinNoiseScale).EndInit();
            ((System.ComponentModel.ISupportInitialize)WallPerlinNoiseOctave).EndInit();
            ((System.ComponentModel.ISupportInitialize)WallThickness).EndInit();
            ((System.ComponentModel.ISupportInitialize)ParticleCountLimit).EndInit();
            ((System.ComponentModel.ISupportInitialize)TotalBiomassAmount).EndInit();
            ((System.ComponentModel.ISupportInitialize)InitialAnimalCount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown SoupSizeX;
        private Label label1;
        private Label label2;
        private NumericUpDown SoupSizeY;
        private Label label3;
        private Label label4;
        private Label label6;
        private NumericUpDown WallPerlinNoiseX;
        private NumericUpDown WallPerlinNoiseY;
        private Label label7;
        private NumericUpDown WallPerlinNoiseZ;
        private Label label5;
        private NumericUpDown WallPerlinNoiseScale;
        private Label label8;
        private NumericUpDown WallPerlinNoiseOctave;
        private Label label9;
        private NumericUpDown WallThickness;
        private Label label10;
        private Label label11;
        private Label label12;
        private NumericUpDown ParticleCountLimit;
        private Label label13;
        private Label label14;
        private NumericUpDown TotalBiomassAmount;
        private Label label15;
        private Label label16;
        private NumericUpDown InitialAnimalCount;
        private CheckBox WallPerlinNoisePositionRandomize;
        private Button ButtonOK;
        private Button ButtonCancel;
    }
}