namespace Paramecium.Forms
{
    partial class FormCreateNewSoup
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
            groupBox1 = new GroupBox();
            checkboxEnableWall = new CheckBox();
            inputNoiseOctave = new NumericUpDown();
            label10 = new Label();
            inputNoiseScale = new NumericUpDown();
            label9 = new Label();
            inputWallThickness = new NumericUpDown();
            label8 = new Label();
            buttonNoiseXYZRandomize = new Button();
            label5 = new Label();
            inputNoiseZ = new NumericUpDown();
            inputNoiseX = new NumericUpDown();
            label7 = new Label();
            label6 = new Label();
            inputNoiseY = new NumericUpDown();
            inputSoupHeight = new NumericUpDown();
            inputSoupWidth = new NumericUpDown();
            label3 = new Label();
            label2 = new Label();
            inputSeed = new NumericUpDown();
            label1 = new Label();
            label4 = new Label();
            inputTotalElementAmount = new NumericUpDown();
            inputCellSizeMultiplier = new NumericUpDown();
            label11 = new Label();
            buttonSeedRandomize = new Button();
            inputPlantForkCost = new NumericUpDown();
            label12 = new Label();
            inputAnimalForkCost = new NumericUpDown();
            label13 = new Label();
            inputPlantElementCollectionRange = new NumericUpDown();
            label14 = new Label();
            inputInitialAnimalCount = new NumericUpDown();
            label15 = new Label();
            inputHatchingTime = new NumericUpDown();
            label16 = new Label();
            inputMutationRate = new NumericUpDown();
            label17 = new Label();
            inputAnimalSurvivalCost = new NumericUpDown();
            label18 = new Label();
            inputAnimalAccDecCost = new NumericUpDown();
            label19 = new Label();
            buttonCreateSoup = new Button();
            buttonCancel = new Button();
            label20 = new Label();
            textboxSoupName = new TextBox();
            label21 = new Label();
            inputAnimalColorCognateRange = new NumericUpDown();
            label22 = new Label();
            inputAnimalColorMutationRange = new NumericUpDown();
            label23 = new Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)inputNoiseOctave).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputNoiseScale).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputWallThickness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputNoiseZ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputNoiseX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputNoiseY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputSoupHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputSoupWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputSeed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputTotalElementAmount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputCellSizeMultiplier).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputPlantForkCost).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputAnimalForkCost).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputPlantElementCollectionRange).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputInitialAnimalCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputHatchingTime).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputMutationRate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputAnimalSurvivalCost).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputAnimalAccDecCost).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputAnimalColorCognateRange).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inputAnimalColorMutationRange).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkboxEnableWall);
            groupBox1.Controls.Add(inputNoiseOctave);
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(inputNoiseScale);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(inputWallThickness);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(buttonNoiseXYZRandomize);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(inputNoiseZ);
            groupBox1.Controls.Add(inputNoiseX);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(inputNoiseY);
            groupBox1.Location = new Point(12, 96);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(358, 142);
            groupBox1.TabIndex = 13;
            groupBox1.TabStop = false;
            groupBox1.Text = "Wall Generation Settings";
            // 
            // checkboxEnableWall
            // 
            checkboxEnableWall.AutoSize = true;
            checkboxEnableWall.Checked = true;
            checkboxEnableWall.CheckState = CheckState.Checked;
            checkboxEnableWall.Location = new Point(202, 114);
            checkboxEnableWall.Name = "checkboxEnableWall";
            checkboxEnableWall.Size = new Size(87, 19);
            checkboxEnableWall.TabIndex = 20;
            checkboxEnableWall.Text = "Enable Wall";
            checkboxEnableWall.UseVisualStyleBackColor = true;
            // 
            // inputNoiseOctave
            // 
            inputNoiseOctave.Location = new Point(265, 51);
            inputNoiseOctave.Maximum = new decimal(new int[] { 16, 0, 0, 0 });
            inputNoiseOctave.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            inputNoiseOctave.Name = "inputNoiseOctave";
            inputNoiseOctave.Size = new Size(80, 23);
            inputNoiseOctave.TabIndex = 19;
            inputNoiseOctave.TextAlign = HorizontalAlignment.Center;
            inputNoiseOctave.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(169, 53);
            label10.Name = "label10";
            label10.Size = new Size(83, 15);
            label10.TabIndex = 18;
            label10.Text = "Noise Octave :";
            // 
            // inputNoiseScale
            // 
            inputNoiseScale.DecimalPlaces = 3;
            inputNoiseScale.Location = new Point(265, 22);
            inputNoiseScale.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            inputNoiseScale.Name = "inputNoiseScale";
            inputNoiseScale.Size = new Size(80, 23);
            inputNoiseScale.TabIndex = 17;
            inputNoiseScale.TextAlign = HorizontalAlignment.Center;
            inputNoiseScale.Value = new decimal(new int[] { 3, 0, 0, 131072 });
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(169, 24);
            label9.Name = "label9";
            label9.Size = new Size(73, 15);
            label9.TabIndex = 16;
            label9.Text = "Noise Scale :";
            // 
            // inputWallThickness
            // 
            inputWallThickness.DecimalPlaces = 3;
            inputWallThickness.Location = new Point(265, 80);
            inputWallThickness.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            inputWallThickness.Name = "inputWallThickness";
            inputWallThickness.Size = new Size(80, 23);
            inputWallThickness.TabIndex = 15;
            inputWallThickness.TextAlign = HorizontalAlignment.Center;
            inputWallThickness.Value = new decimal(new int[] { 1, 0, 0, 131072 });
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(169, 82);
            label8.Name = "label8";
            label8.Size = new Size(90, 15);
            label8.TabIndex = 14;
            label8.Text = "Wall Thickness :";
            // 
            // buttonNoiseXYZRandomize
            // 
            buttonNoiseXYZRandomize.Location = new Point(38, 109);
            buttonNoiseXYZRandomize.Name = "buttonNoiseXYZRandomize";
            buttonNoiseXYZRandomize.Size = new Size(80, 24);
            buttonNoiseXYZRandomize.TabIndex = 13;
            buttonNoiseXYZRandomize.Text = "Randomize";
            buttonNoiseXYZRandomize.UseVisualStyleBackColor = true;
            buttonNoiseXYZRandomize.Click += buttonNoiseXYZRandomize_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(9, 24);
            label5.Name = "label5";
            label5.Size = new Size(53, 15);
            label5.TabIndex = 7;
            label5.Text = "Noise X :";
            // 
            // inputNoiseZ
            // 
            inputNoiseZ.DecimalPlaces = 6;
            inputNoiseZ.Location = new Point(68, 80);
            inputNoiseZ.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            inputNoiseZ.Name = "inputNoiseZ";
            inputNoiseZ.Size = new Size(80, 23);
            inputNoiseZ.TabIndex = 12;
            inputNoiseZ.TextAlign = HorizontalAlignment.Center;
            inputNoiseZ.Value = new decimal(new int[] { 256, 0, 0, 0 });
            // 
            // inputNoiseX
            // 
            inputNoiseX.DecimalPlaces = 6;
            inputNoiseX.Location = new Point(68, 22);
            inputNoiseX.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            inputNoiseX.Name = "inputNoiseX";
            inputNoiseX.Size = new Size(80, 23);
            inputNoiseX.TabIndex = 8;
            inputNoiseX.TextAlign = HorizontalAlignment.Center;
            inputNoiseX.Value = new decimal(new int[] { 256, 0, 0, 0 });
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(9, 82);
            label7.Name = "label7";
            label7.Size = new Size(53, 15);
            label7.TabIndex = 11;
            label7.Text = "Noise Z :";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(9, 53);
            label6.Name = "label6";
            label6.Size = new Size(53, 15);
            label6.TabIndex = 9;
            label6.Text = "Noise Y :";
            // 
            // inputNoiseY
            // 
            inputNoiseY.DecimalPlaces = 6;
            inputNoiseY.Location = new Point(68, 51);
            inputNoiseY.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            inputNoiseY.Name = "inputNoiseY";
            inputNoiseY.Size = new Size(80, 23);
            inputNoiseY.TabIndex = 10;
            inputNoiseY.TextAlign = HorizontalAlignment.Center;
            inputNoiseY.Value = new decimal(new int[] { 256, 0, 0, 0 });
            // 
            // inputSoupHeight
            // 
            inputSoupHeight.Location = new Point(250, 67);
            inputSoupHeight.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
            inputSoupHeight.Minimum = new decimal(new int[] { 32, 0, 0, 0 });
            inputSoupHeight.Name = "inputSoupHeight";
            inputSoupHeight.Size = new Size(120, 23);
            inputSoupHeight.TabIndex = 5;
            inputSoupHeight.TextAlign = HorizontalAlignment.Center;
            inputSoupHeight.Value = new decimal(new int[] { 160, 0, 0, 0 });
            // 
            // inputSoupWidth
            // 
            inputSoupWidth.Location = new Point(63, 67);
            inputSoupWidth.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
            inputSoupWidth.Minimum = new decimal(new int[] { 32, 0, 0, 0 });
            inputSoupWidth.Name = "inputSoupWidth";
            inputSoupWidth.Size = new Size(120, 23);
            inputSoupWidth.TabIndex = 4;
            inputSoupWidth.TextAlign = HorizontalAlignment.Center;
            inputSoupWidth.Value = new decimal(new int[] { 256, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(195, 69);
            label3.Name = "label3";
            label3.Size = new Size(49, 15);
            label3.TabIndex = 3;
            label3.Text = "Height :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 69);
            label2.Name = "label2";
            label2.Size = new Size(45, 15);
            label2.TabIndex = 2;
            label2.Text = "Width :";
            // 
            // inputSeed
            // 
            inputSeed.Location = new Point(134, 38);
            inputSeed.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            inputSeed.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            inputSeed.Name = "inputSeed";
            inputSeed.Size = new Size(150, 23);
            inputSeed.TabIndex = 1;
            inputSeed.TextAlign = HorizontalAlignment.Center;
            inputSeed.Value = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 40);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 0;
            label1.Text = "Seed :";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(376, 11);
            label4.Name = "label4";
            label4.Size = new Size(132, 15);
            label4.TabIndex = 14;
            label4.Text = "Total Element Amount  :";
            // 
            // inputTotalElementAmount
            // 
            inputTotalElementAmount.DecimalPlaces = 3;
            inputTotalElementAmount.Location = new Point(584, 9);
            inputTotalElementAmount.Maximum = new decimal(new int[] { 16777216, 0, 0, 0 });
            inputTotalElementAmount.Name = "inputTotalElementAmount";
            inputTotalElementAmount.Size = new Size(150, 23);
            inputTotalElementAmount.TabIndex = 20;
            inputTotalElementAmount.TextAlign = HorizontalAlignment.Center;
            inputTotalElementAmount.Value = new decimal(new int[] { 163840, 0, 0, 0 });
            // 
            // inputCellSizeMultiplier
            // 
            inputCellSizeMultiplier.DecimalPlaces = 3;
            inputCellSizeMultiplier.Location = new Point(584, 38);
            inputCellSizeMultiplier.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            inputCellSizeMultiplier.Name = "inputCellSizeMultiplier";
            inputCellSizeMultiplier.Size = new Size(150, 23);
            inputCellSizeMultiplier.TabIndex = 22;
            inputCellSizeMultiplier.TextAlign = HorizontalAlignment.Center;
            inputCellSizeMultiplier.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(376, 40);
            label11.Name = "label11";
            label11.Size = new Size(112, 15);
            label11.TabIndex = 21;
            label11.Text = "Cell Size Multiplier  :";
            // 
            // buttonSeedRandomize
            // 
            buttonSeedRandomize.Location = new Point(290, 37);
            buttonSeedRandomize.Name = "buttonSeedRandomize";
            buttonSeedRandomize.Size = new Size(80, 24);
            buttonSeedRandomize.TabIndex = 20;
            buttonSeedRandomize.Text = "Randomize";
            buttonSeedRandomize.UseVisualStyleBackColor = true;
            buttonSeedRandomize.Click += buttonSeedRandomize_Click;
            // 
            // inputPlantForkCost
            // 
            inputPlantForkCost.DecimalPlaces = 3;
            inputPlantForkCost.Location = new Point(584, 67);
            inputPlantForkCost.Name = "inputPlantForkCost";
            inputPlantForkCost.Size = new Size(150, 23);
            inputPlantForkCost.TabIndex = 24;
            inputPlantForkCost.TextAlign = HorizontalAlignment.Center;
            inputPlantForkCost.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(376, 69);
            label12.Name = "label12";
            label12.Size = new Size(95, 15);
            label12.TabIndex = 23;
            label12.Text = "Plant Fork Cost  :";
            // 
            // inputAnimalForkCost
            // 
            inputAnimalForkCost.DecimalPlaces = 3;
            inputAnimalForkCost.Location = new Point(584, 96);
            inputAnimalForkCost.Name = "inputAnimalForkCost";
            inputAnimalForkCost.Size = new Size(150, 23);
            inputAnimalForkCost.TabIndex = 26;
            inputAnimalForkCost.TextAlign = HorizontalAlignment.Center;
            inputAnimalForkCost.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(376, 98);
            label13.Name = "label13";
            label13.Size = new Size(105, 15);
            label13.TabIndex = 25;
            label13.Text = "Animal Fork Cost  :";
            // 
            // inputPlantElementCollectionRange
            // 
            inputPlantElementCollectionRange.Location = new Point(584, 125);
            inputPlantElementCollectionRange.Maximum = new decimal(new int[] { 15, 0, 0, 0 });
            inputPlantElementCollectionRange.Name = "inputPlantElementCollectionRange";
            inputPlantElementCollectionRange.Size = new Size(150, 23);
            inputPlantElementCollectionRange.TabIndex = 28;
            inputPlantElementCollectionRange.TextAlign = HorizontalAlignment.Center;
            inputPlantElementCollectionRange.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(376, 127);
            label14.Name = "label14";
            label14.Size = new Size(180, 15);
            label14.TabIndex = 27;
            label14.Text = "Plant Element Collection Range  :";
            // 
            // inputInitialAnimalCount
            // 
            inputInitialAnimalCount.Location = new Point(584, 154);
            inputInitialAnimalCount.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
            inputInitialAnimalCount.Name = "inputInitialAnimalCount";
            inputInitialAnimalCount.Size = new Size(150, 23);
            inputInitialAnimalCount.TabIndex = 30;
            inputInitialAnimalCount.TextAlign = HorizontalAlignment.Center;
            inputInitialAnimalCount.Value = new decimal(new int[] { 1600, 0, 0, 0 });
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(376, 156);
            label15.Name = "label15";
            label15.Size = new Size(117, 15);
            label15.TabIndex = 29;
            label15.Text = "Initial Animal Count :";
            // 
            // inputHatchingTime
            // 
            inputHatchingTime.Location = new Point(584, 183);
            inputHatchingTime.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            inputHatchingTime.Name = "inputHatchingTime";
            inputHatchingTime.Size = new Size(150, 23);
            inputHatchingTime.TabIndex = 32;
            inputHatchingTime.TextAlign = HorizontalAlignment.Center;
            inputHatchingTime.Value = new decimal(new int[] { 300, 0, 0, 0 });
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(376, 185);
            label16.Name = "label16";
            label16.Size = new Size(90, 15);
            label16.TabIndex = 31;
            label16.Text = "Hatching Time :";
            // 
            // inputMutationRate
            // 
            inputMutationRate.DecimalPlaces = 6;
            inputMutationRate.Location = new Point(584, 212);
            inputMutationRate.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            inputMutationRate.Name = "inputMutationRate";
            inputMutationRate.Size = new Size(150, 23);
            inputMutationRate.TabIndex = 34;
            inputMutationRate.TextAlign = HorizontalAlignment.Center;
            inputMutationRate.Value = new decimal(new int[] { 15625, 0, 0, 393216 });
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(376, 214);
            label17.Name = "label17";
            label17.Size = new Size(88, 15);
            label17.TabIndex = 33;
            label17.Text = "Mutation Rate :";
            // 
            // inputAnimalSurvivalCost
            // 
            inputAnimalSurvivalCost.DecimalPlaces = 6;
            inputAnimalSurvivalCost.Location = new Point(584, 241);
            inputAnimalSurvivalCost.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            inputAnimalSurvivalCost.Name = "inputAnimalSurvivalCost";
            inputAnimalSurvivalCost.Size = new Size(150, 23);
            inputAnimalSurvivalCost.TabIndex = 36;
            inputAnimalSurvivalCost.TextAlign = HorizontalAlignment.Center;
            inputAnimalSurvivalCost.Value = new decimal(new int[] { 25, 0, 0, 196608 });
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(376, 243);
            label18.Name = "label18";
            label18.Size = new Size(120, 15);
            label18.TabIndex = 35;
            label18.Text = "Animal Survival Cost :";
            // 
            // inputAnimalAccDecCost
            // 
            inputAnimalAccDecCost.DecimalPlaces = 6;
            inputAnimalAccDecCost.Location = new Point(584, 270);
            inputAnimalAccDecCost.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            inputAnimalAccDecCost.Name = "inputAnimalAccDecCost";
            inputAnimalAccDecCost.Size = new Size(150, 23);
            inputAnimalAccDecCost.TabIndex = 38;
            inputAnimalAccDecCost.TextAlign = HorizontalAlignment.Center;
            inputAnimalAccDecCost.Value = new decimal(new int[] { 75, 0, 0, 196608 });
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(376, 272);
            label19.Name = "label19";
            label19.Size = new Size(130, 15);
            label19.TabIndex = 37;
            label19.Text = "Animal Acc./Dec. Cost :";
            // 
            // buttonCreateSoup
            // 
            buttonCreateSoup.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonCreateSoup.Location = new Point(14, 324);
            buttonCreateSoup.Name = "buttonCreateSoup";
            buttonCreateSoup.Size = new Size(80, 24);
            buttonCreateSoup.TabIndex = 39;
            buttonCreateSoup.Text = "Create Soup";
            buttonCreateSoup.UseVisualStyleBackColor = true;
            buttonCreateSoup.Click += buttonCreateSoup_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonCancel.Location = new Point(100, 324);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(80, 24);
            buttonCancel.TabIndex = 40;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(12, 12);
            label20.Name = "label20";
            label20.Size = new Size(74, 15);
            label20.TabIndex = 41;
            label20.Text = "Soup Name :";
            // 
            // textboxSoupName
            // 
            textboxSoupName.Location = new Point(92, 9);
            textboxSoupName.Name = "textboxSoupName";
            textboxSoupName.Size = new Size(236, 23);
            textboxSoupName.TabIndex = 42;
            textboxSoupName.Text = "Untitled";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(334, 12);
            label21.Name = "label21";
            label21.Size = new Size(36, 15);
            label21.TabIndex = 43;
            label21.Text = ".soup";
            // 
            // inputAnimalColorCognateRange
            // 
            inputAnimalColorCognateRange.DecimalPlaces = 6;
            inputAnimalColorCognateRange.Location = new Point(584, 328);
            inputAnimalColorCognateRange.Maximum = new decimal(new int[] { 441, 0, 0, 0 });
            inputAnimalColorCognateRange.Name = "inputAnimalColorCognateRange";
            inputAnimalColorCognateRange.Size = new Size(150, 23);
            inputAnimalColorCognateRange.TabIndex = 47;
            inputAnimalColorCognateRange.TextAlign = HorizontalAlignment.Center;
            inputAnimalColorCognateRange.Value = new decimal(new int[] { 175, 0, 0, 196608 });
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(376, 330);
            label22.Name = "label22";
            label22.Size = new Size(164, 15);
            label22.TabIndex = 46;
            label22.Text = "Animal Color Cognate Range :";
            // 
            // inputAnimalColorMutationRange
            // 
            inputAnimalColorMutationRange.DecimalPlaces = 6;
            inputAnimalColorMutationRange.Location = new Point(584, 299);
            inputAnimalColorMutationRange.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            inputAnimalColorMutationRange.Name = "inputAnimalColorMutationRange";
            inputAnimalColorMutationRange.Size = new Size(150, 23);
            inputAnimalColorMutationRange.TabIndex = 45;
            inputAnimalColorMutationRange.TextAlign = HorizontalAlignment.Center;
            inputAnimalColorMutationRange.Value = new decimal(new int[] { 875, 0, 0, 262144 });
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(376, 301);
            label23.Name = "label23";
            label23.Size = new Size(169, 15);
            label23.TabIndex = 44;
            label23.Text = "Animal Color Mutation Range :";
            // 
            // FormCreateNewSoup
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(743, 360);
            Controls.Add(inputAnimalColorCognateRange);
            Controls.Add(label22);
            Controls.Add(inputAnimalColorMutationRange);
            Controls.Add(label23);
            Controls.Add(textboxSoupName);
            Controls.Add(label20);
            Controls.Add(buttonCancel);
            Controls.Add(buttonCreateSoup);
            Controls.Add(inputAnimalAccDecCost);
            Controls.Add(label19);
            Controls.Add(inputAnimalSurvivalCost);
            Controls.Add(label18);
            Controls.Add(inputMutationRate);
            Controls.Add(label17);
            Controls.Add(inputHatchingTime);
            Controls.Add(label16);
            Controls.Add(inputInitialAnimalCount);
            Controls.Add(label15);
            Controls.Add(inputPlantElementCollectionRange);
            Controls.Add(label14);
            Controls.Add(inputAnimalForkCost);
            Controls.Add(label13);
            Controls.Add(inputPlantForkCost);
            Controls.Add(label12);
            Controls.Add(buttonSeedRandomize);
            Controls.Add(inputCellSizeMultiplier);
            Controls.Add(label11);
            Controls.Add(inputTotalElementAmount);
            Controls.Add(label4);
            Controls.Add(groupBox1);
            Controls.Add(label1);
            Controls.Add(inputSoupHeight);
            Controls.Add(inputSeed);
            Controls.Add(inputSoupWidth);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(label21);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormCreateNewSoup";
            ShowIcon = false;
            Text = "Creating a New Soup";
            FormClosing += FormCreateNewSoup_FormClosing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)inputNoiseOctave).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputNoiseScale).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputWallThickness).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputNoiseZ).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputNoiseX).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputNoiseY).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputSoupHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputSoupWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputSeed).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputTotalElementAmount).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputCellSizeMultiplier).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputPlantForkCost).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputAnimalForkCost).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputPlantElementCollectionRange).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputInitialAnimalCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputHatchingTime).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputMutationRate).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputAnimalSurvivalCost).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputAnimalAccDecCost).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputAnimalColorCognateRange).EndInit();
            ((System.ComponentModel.ISupportInitialize)inputAnimalColorMutationRange).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private NumericUpDown inputSeed;
        private Label label1;
        private NumericUpDown inputSoupHeight;
        private NumericUpDown inputSoupWidth;
        private Label label3;
        private Label label2;
        private NumericUpDown inputNoiseZ;
        private Label label7;
        private NumericUpDown inputNoiseY;
        private Label label6;
        private NumericUpDown inputNoiseX;
        private Label label5;
        private GroupBox groupBox1;
        private Button buttonNoiseXYZRandomize;
        private NumericUpDown inputWallThickness;
        private Label label8;
        private NumericUpDown inputNoiseOctave;
        private Label label10;
        private NumericUpDown inputNoiseScale;
        private Label label9;
        private Label label4;
        private NumericUpDown inputTotalElementAmount;
        private NumericUpDown inputCellSizeMultiplier;
        private Label label11;
        private Button buttonSeedRandomize;
        private NumericUpDown inputPlantForkCost;
        private Label label12;
        private NumericUpDown inputAnimalForkCost;
        private Label label13;
        private NumericUpDown inputPlantElementCollectionRange;
        private Label label14;
        private NumericUpDown inputInitialAnimalCount;
        private Label label15;
        private CheckBox checkboxEnableWall;
        private NumericUpDown inputHatchingTime;
        private Label label16;
        private NumericUpDown inputMutationRate;
        private Label label17;
        private NumericUpDown inputAnimalSurvivalCost;
        private Label label18;
        private NumericUpDown inputAnimalAccDecCost;
        private Label label19;
        private Button buttonCreateSoup;
        private Button buttonCancel;
        private Label label20;
        private TextBox textboxSoupName;
        private Label label21;
        private NumericUpDown inputAnimalColorCognateRange;
        private Label label22;
        private NumericUpDown inputAnimalColorMutationRange;
        private Label label23;
    }
}