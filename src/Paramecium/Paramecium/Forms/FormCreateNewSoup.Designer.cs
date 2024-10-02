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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCreateNewSoup));
            ButtonEditSoupSettings = new Button();
            ButtonCreateSoup = new Button();
            ButtonCancel = new Button();
            GroupBoxCurrentSoupSettings = new GroupBox();
            LabelInitialAnimalPopulation = new Label();
            LabelInitialPlantPopulation = new Label();
            LabelTotalElementAmount = new Label();
            LabelSoupSize = new Label();
            LabelInitialSeed = new Label();
            label10 = new Label();
            label5 = new Label();
            label6 = new Label();
            label3 = new Label();
            label1 = new Label();
            GroupBoxCurrentSoupSettings.SuspendLayout();
            SuspendLayout();
            // 
            // ButtonEditSoupSettings
            // 
            ButtonEditSoupSettings.Location = new Point(85, 165);
            ButtonEditSoupSettings.Name = "ButtonEditSoupSettings";
            ButtonEditSoupSettings.Size = new Size(150, 23);
            ButtonEditSoupSettings.TabIndex = 0;
            ButtonEditSoupSettings.Text = "View More and Edit";
            ButtonEditSoupSettings.UseVisualStyleBackColor = true;
            ButtonEditSoupSettings.Click += ButtonEditSoupSettings_Click;
            // 
            // ButtonCreateSoup
            // 
            ButtonCreateSoup.Location = new Point(134, 222);
            ButtonCreateSoup.Name = "ButtonCreateSoup";
            ButtonCreateSoup.Size = new Size(100, 23);
            ButtonCreateSoup.TabIndex = 1;
            ButtonCreateSoup.Text = "Create Soup";
            ButtonCreateSoup.UseVisualStyleBackColor = true;
            ButtonCreateSoup.Click += ButtonCreateSoup_Click;
            // 
            // ButtonCancel
            // 
            ButtonCancel.Location = new Point(240, 222);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(100, 23);
            ButtonCancel.TabIndex = 2;
            ButtonCancel.Text = "Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            ButtonCancel.Click += ButtonCancel_Click;
            // 
            // GroupBoxCurrentSoupSettings
            // 
            GroupBoxCurrentSoupSettings.Controls.Add(LabelInitialAnimalPopulation);
            GroupBoxCurrentSoupSettings.Controls.Add(LabelInitialPlantPopulation);
            GroupBoxCurrentSoupSettings.Controls.Add(LabelTotalElementAmount);
            GroupBoxCurrentSoupSettings.Controls.Add(LabelSoupSize);
            GroupBoxCurrentSoupSettings.Controls.Add(LabelInitialSeed);
            GroupBoxCurrentSoupSettings.Controls.Add(label10);
            GroupBoxCurrentSoupSettings.Controls.Add(label5);
            GroupBoxCurrentSoupSettings.Controls.Add(label6);
            GroupBoxCurrentSoupSettings.Controls.Add(ButtonEditSoupSettings);
            GroupBoxCurrentSoupSettings.Controls.Add(label3);
            GroupBoxCurrentSoupSettings.Controls.Add(label1);
            GroupBoxCurrentSoupSettings.Location = new Point(12, 12);
            GroupBoxCurrentSoupSettings.Name = "GroupBoxCurrentSoupSettings";
            GroupBoxCurrentSoupSettings.Size = new Size(320, 204);
            GroupBoxCurrentSoupSettings.TabIndex = 3;
            GroupBoxCurrentSoupSettings.TabStop = false;
            GroupBoxCurrentSoupSettings.Text = "Current Soup Settings";
            // 
            // LabelInitialAnimalPopulation
            // 
            LabelInitialAnimalPopulation.AutoSize = true;
            LabelInitialAnimalPopulation.Location = new Point(216, 129);
            LabelInitialAnimalPopulation.Name = "LabelInitialAnimalPopulation";
            LabelInitialAnimalPopulation.Size = new Size(31, 15);
            LabelInitialAnimalPopulation.TabIndex = 14;
            LabelInitialAnimalPopulation.Text = ": 256";
            // 
            // LabelInitialPlantPopulation
            // 
            LabelInitialPlantPopulation.AutoSize = true;
            LabelInitialPlantPopulation.Location = new Point(216, 104);
            LabelInitialPlantPopulation.Name = "LabelInitialPlantPopulation";
            LabelInitialPlantPopulation.Size = new Size(37, 15);
            LabelInitialPlantPopulation.TabIndex = 13;
            LabelInitialPlantPopulation.Text = ": 1024";
            // 
            // LabelTotalElementAmount
            // 
            LabelTotalElementAmount.AutoSize = true;
            LabelTotalElementAmount.Location = new Point(216, 79);
            LabelTotalElementAmount.Name = "LabelTotalElementAmount";
            LabelTotalElementAmount.Size = new Size(43, 15);
            LabelTotalElementAmount.TabIndex = 12;
            LabelTotalElementAmount.Text = ": 32768";
            // 
            // LabelSoupSize
            // 
            LabelSoupSize.AutoSize = true;
            LabelSoupSize.Location = new Point(216, 54);
            LabelSoupSize.Name = "LabelSoupSize";
            LabelSoupSize.Size = new Size(61, 15);
            LabelSoupSize.TabIndex = 11;
            LabelSoupSize.Text = ": 128 x 128";
            // 
            // LabelInitialSeed
            // 
            LabelInitialSeed.AutoSize = true;
            LabelInitialSeed.Location = new Point(216, 29);
            LabelInitialSeed.Name = "LabelInitialSeed";
            LabelInitialSeed.Size = new Size(19, 15);
            LabelInitialSeed.TabIndex = 10;
            LabelInitialSeed.Text = ": 0";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(16, 129);
            label10.Name = "label10";
            label10.Size = new Size(137, 15);
            label10.TabIndex = 9;
            label10.Text = "Animal Initial Population";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(16, 104);
            label5.Name = "label5";
            label5.Size = new Size(127, 15);
            label5.TabIndex = 6;
            label5.Text = "Plant Initial Population";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(16, 79);
            label6.Name = "label6";
            label6.Size = new Size(123, 15);
            label6.TabIndex = 5;
            label6.Text = "Total Element Amount";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(16, 54);
            label3.Name = "label3";
            label3.Size = new Size(57, 15);
            label3.TabIndex = 2;
            label3.Text = "Soup Size";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 29);
            label1.Name = "label1";
            label1.Size = new Size(64, 15);
            label1.TabIndex = 0;
            label1.Text = "Initial Seed";
            // 
            // FormCreateNewSoup
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(352, 255);
            Controls.Add(GroupBoxCurrentSoupSettings);
            Controls.Add(ButtonCancel);
            Controls.Add(ButtonCreateSoup);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormCreateNewSoup";
            Text = "Create New Soup";
            GroupBoxCurrentSoupSettings.ResumeLayout(false);
            GroupBoxCurrentSoupSettings.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button ButtonEditSoupSettings;
        private Button ButtonCreateSoup;
        private Button ButtonCancel;
        private GroupBox GroupBoxCurrentSoupSettings;
        private Label label1;
        private Label label3;
        private Label label10;
        private Label label5;
        private Label label6;
        private Label LabelInitialAnimalPopulation;
        private Label LabelInitialPlantPopulation;
        private Label LabelTotalElementAmount;
        private Label LabelSoupSize;
        private Label LabelInitialSeed;
    }
}