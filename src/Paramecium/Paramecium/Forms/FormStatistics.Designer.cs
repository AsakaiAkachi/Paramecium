namespace Paramecium.Forms
{
    partial class FormStatistics
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
            panel1 = new Panel();
            ComboBoxTimeSpan = new ComboBox();
            LabelTimeSpan = new Label();
            ComboBoxDataType = new ComboBox();
            LabelDataType = new Label();
            panel2 = new Panel();
            LabelYMaxValue = new Label();
            LabelYMinValue = new Label();
            LabelXMaxValue = new Label();
            LabelXMinValue = new Label();
            GraphView = new PictureBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)GraphView).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(ComboBoxTimeSpan);
            panel1.Controls.Add(LabelTimeSpan);
            panel1.Controls.Add(ComboBoxDataType);
            panel1.Controls.Add(LabelDataType);
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1095, 60);
            panel1.TabIndex = 0;
            // 
            // ComboBoxTimeSpan
            // 
            ComboBoxTimeSpan.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxTimeSpan.FormattingEnabled = true;
            ComboBoxTimeSpan.Items.AddRange(new object[] { "10k Steps", "20k Steps", "40k Steps", "80k Steps", "160k Steps", "320k Steps", "640k Steps", "1280k Steps", "2560k Steps", "5120k Steps", "10240k Steps" });
            ComboBoxTimeSpan.Location = new Point(425, 18);
            ComboBoxTimeSpan.Name = "ComboBoxTimeSpan";
            ComboBoxTimeSpan.Size = new Size(150, 23);
            ComboBoxTimeSpan.TabIndex = 3;
            // 
            // LabelTimeSpan
            // 
            LabelTimeSpan.AutoSize = true;
            LabelTimeSpan.Location = new Point(350, 22);
            LabelTimeSpan.Name = "LabelTimeSpan";
            LabelTimeSpan.Size = new Size(67, 15);
            LabelTimeSpan.TabIndex = 2;
            LabelTimeSpan.Text = "Time Span :";
            // 
            // ComboBoxDataType
            // 
            ComboBoxDataType.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxDataType.FormattingEnabled = true;
            ComboBoxDataType.Items.AddRange(new object[] { "Total Mutation Count Distribution", "Generation Distribution", "Age Distribution", "Offspring Count Distribution", "Velocity Magnitude Distribution", "Angular Velocity Distribution", "Element Amount Distribution", "Element In/Outflow Distribution", "Reproduction Progress Distribution", "Reproduction Rate Distribution", "Brain Eat Output Distribution", "Brain Attack Output Distribution", "Animal Population Change Over Time", "Latest Generation Change Over Time", "Max Total Mutation Count Change Over Time" });
            ComboBoxDataType.Location = new Point(60, 18);
            ComboBoxDataType.Name = "ComboBoxDataType";
            ComboBoxDataType.Size = new Size(250, 23);
            ComboBoxDataType.TabIndex = 1;
            // 
            // LabelDataType
            // 
            LabelDataType.AutoSize = true;
            LabelDataType.Location = new Point(15, 22);
            LabelDataType.Name = "LabelDataType";
            LabelDataType.Size = new Size(37, 15);
            LabelDataType.TabIndex = 0;
            LabelDataType.Text = "Data :";
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.BackColor = Color.Black;
            panel2.Controls.Add(LabelYMaxValue);
            panel2.Controls.Add(LabelYMinValue);
            panel2.Controls.Add(LabelXMaxValue);
            panel2.Controls.Add(LabelXMinValue);
            panel2.Controls.Add(GraphView);
            panel2.Location = new Point(0, 60);
            panel2.Name = "panel2";
            panel2.Size = new Size(1095, 549);
            panel2.TabIndex = 1;
            // 
            // LabelYMaxValue
            // 
            LabelYMaxValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LabelYMaxValue.AutoSize = true;
            LabelYMaxValue.ForeColor = Color.White;
            LabelYMaxValue.Location = new Point(1022, 15);
            LabelYMaxValue.Name = "LabelYMaxValue";
            LabelYMaxValue.Size = new Size(58, 15);
            LabelYMaxValue.TabIndex = 4;
            LabelYMaxValue.Text = "16.000000";
            // 
            // LabelYMinValue
            // 
            LabelYMinValue.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            LabelYMinValue.AutoSize = true;
            LabelYMinValue.ForeColor = Color.White;
            LabelYMinValue.Location = new Point(1022, 501);
            LabelYMinValue.Name = "LabelYMinValue";
            LabelYMinValue.Size = new Size(52, 15);
            LabelYMinValue.TabIndex = 3;
            LabelYMinValue.Text = "0.000000";
            // 
            // LabelXMaxValue
            // 
            LabelXMaxValue.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            LabelXMaxValue.AutoSize = true;
            LabelXMaxValue.ForeColor = Color.White;
            LabelXMaxValue.Location = new Point(1003, 519);
            LabelXMaxValue.Name = "LabelXMaxValue";
            LabelXMaxValue.Size = new Size(31, 15);
            LabelXMaxValue.TabIndex = 2;
            LabelXMaxValue.Text = "1000";
            // 
            // LabelXMinValue
            // 
            LabelXMinValue.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LabelXMinValue.AutoSize = true;
            LabelXMinValue.ForeColor = Color.White;
            LabelXMinValue.Location = new Point(15, 519);
            LabelXMinValue.Name = "LabelXMinValue";
            LabelXMinValue.Size = new Size(13, 15);
            LabelXMinValue.TabIndex = 1;
            LabelXMinValue.Text = "0";
            // 
            // GraphView
            // 
            GraphView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GraphView.Location = new Point(15, 15);
            GraphView.Name = "GraphView";
            GraphView.Size = new Size(1001, 501);
            GraphView.TabIndex = 0;
            GraphView.TabStop = false;
            // 
            // FormStatistics
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1095, 609);
            Controls.Add(panel2);
            Controls.Add(panel1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormStatistics";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Statistics";
            Shown += FormStatistics_Shown;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)GraphView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private PictureBox GraphView;
        private Label LabelXMaxValue;
        private Label LabelXMinValue;
        private Label LabelYMaxValue;
        private Label LabelYMinValue;
        private Label LabelDataType;
        private ComboBox ComboBoxDataType;
        private ComboBox ComboBoxTimeSpan;
        private Label LabelTimeSpan;
    }
}