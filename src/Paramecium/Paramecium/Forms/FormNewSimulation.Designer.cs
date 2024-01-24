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
            ((System.ComponentModel.ISupportInitialize)SoupParameterView).BeginInit();
            SuspendLayout();
            // 
            // ButtonOK
            // 
            ButtonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ButtonOK.Location = new Point(12, 423);
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
            ButtonCancel.Location = new Point(108, 423);
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
            SoupParameterView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SoupParameterView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            SoupParameterView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            SoupParameterView.Columns.AddRange(new DataGridViewColumn[] { ColumnSoupParameter, ColumnValue });
            SoupParameterView.Location = new Point(12, 12);
            SoupParameterView.Name = "SoupParameterView";
            SoupParameterView.RowHeadersVisible = false;
            SoupParameterView.Size = new Size(500, 400);
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
            // FormNewSimulation
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 465);
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
        }

        #endregion
        private Button ButtonOK;
        private Button ButtonCancel;
        private DataGridView SoupParameterView;
        private DataGridViewTextBoxColumn ColumnSoupParameter;
        private DataGridViewTextBoxColumn ColumnValue;
    }
}