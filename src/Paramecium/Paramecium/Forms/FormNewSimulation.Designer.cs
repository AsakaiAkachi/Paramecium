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
            ButtonOK = new Button();
            ButtonCancel = new Button();
            SoupParameterView = new DataGridView();
            ColumnSoupParameter = new DataGridViewTextBoxColumn();
            ColumnValue = new DataGridViewTextBoxColumn();
            label1 = new Label();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)SoupParameterView).BeginInit();
            SuspendLayout();
            // 
            // ButtonOK
            // 
            ButtonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ButtonOK.Location = new Point(12, 443);
            ButtonOK.Name = "ButtonOK";
            ButtonOK.Size = new Size(90, 30);
            ButtonOK.TabIndex = 32;
            ButtonOK.Text = "スープを作成";
            ButtonOK.UseVisualStyleBackColor = true;
            ButtonOK.Click += ButtonOK_Click;
            // 
            // ButtonCancel
            // 
            ButtonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ButtonCancel.Location = new Point(108, 443);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(90, 30);
            ButtonCancel.TabIndex = 33;
            ButtonCancel.Text = "キャンセル";
            ButtonCancel.UseVisualStyleBackColor = true;
            ButtonCancel.Click += ButtonCancel_Click;
            // 
            // SoupParameterView
            // 
            SoupParameterView.AllowUserToAddRows = false;
            SoupParameterView.AllowUserToDeleteRows = false;
            SoupParameterView.AllowUserToResizeColumns = false;
            SoupParameterView.AllowUserToResizeRows = false;
            SoupParameterView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            SoupParameterView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            SoupParameterView.ColumnHeadersVisible = false;
            SoupParameterView.Columns.AddRange(new DataGridViewColumn[] { ColumnSoupParameter, ColumnValue });
            SoupParameterView.Location = new Point(8, 33);
            SoupParameterView.MultiSelect = false;
            SoupParameterView.Name = "SoupParameterView";
            SoupParameterView.RowHeadersVisible = false;
            SoupParameterView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            SoupParameterView.Size = new Size(500, 378);
            SoupParameterView.TabIndex = 34;
            // 
            // ColumnSoupParameter
            // 
            ColumnSoupParameter.HeaderText = "パラメーター";
            ColumnSoupParameter.Name = "ColumnSoupParameter";
            ColumnSoupParameter.ReadOnly = true;
            // 
            // ColumnValue
            // 
            ColumnValue.HeaderText = "値";
            ColumnValue.Name = "ColumnValue";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 9);
            label1.Name = "label1";
            label1.Size = new Size(94, 15);
            label1.TabIndex = 35;
            label1.Text = "スープのパラメーター";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 420);
            label2.Name = "label2";
            label2.Size = new Size(342, 15);
            label2.TabIndex = 36;
            label2.Text = "上の表にパラメーターを入力した後、「スープを作成」ボタンを押してください";
            // 
            // FormNewSimulation
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(518, 485);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(SoupParameterView);
            Controls.Add(ButtonCancel);
            Controls.Add(ButtonOK);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormNewSimulation";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "スープを新規作成";
            ((System.ComponentModel.ISupportInitialize)SoupParameterView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button ButtonOK;
        private Button ButtonCancel;
        private DataGridView SoupParameterView;
        private DataGridViewTextBoxColumn ColumnSoupParameter;
        private DataGridViewTextBoxColumn ColumnValue;
        private Label label1;
        private Label label2;
    }
}