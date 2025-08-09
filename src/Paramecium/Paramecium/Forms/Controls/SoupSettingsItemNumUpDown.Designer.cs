namespace Paramecium.Forms.Controls
{
    partial class SoupSettingsItemNumUpDown
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            ItemNameLabel = new Label();
            InputNumUpDown = new NumericUpDown();
            ResetButton = new Button();
            ((System.ComponentModel.ISupportInitialize)InputNumUpDown).BeginInit();
            SuspendLayout();
            // 
            // ItemNameLabel
            // 
            ItemNameLabel.AutoSize = true;
            ItemNameLabel.Location = new Point(8, 8);
            ItemNameLabel.Name = "ItemNameLabel";
            ItemNameLabel.Size = new Size(64, 15);
            ItemNameLabel.TabIndex = 0;
            ItemNameLabel.Text = "Item Name";
            // 
            // InputNumUpDown
            // 
            InputNumUpDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            InputNumUpDown.DecimalPlaces = 20;
            InputNumUpDown.Location = new Point(220, 4);
            InputNumUpDown.Margin = new Padding(3, 3, 0, 3);
            InputNumUpDown.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            InputNumUpDown.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            InputNumUpDown.Name = "InputNumUpDown";
            InputNumUpDown.Size = new Size(150, 23);
            InputNumUpDown.TabIndex = 1;
            // 
            // ResetButton
            // 
            ResetButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ResetButton.Font = new Font("Segoe MDL2 Assets", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ResetButton.Location = new Point(373, 4);
            ResetButton.Name = "ResetButton";
            ResetButton.Size = new Size(23, 23);
            ResetButton.TabIndex = 2;
            ResetButton.Text = "";
            ResetButton.UseVisualStyleBackColor = true;
            ResetButton.Click += ResetButton_Click;
            // 
            // SoupSettingsItemNumUpDown
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ResetButton);
            Controls.Add(InputNumUpDown);
            Controls.Add(ItemNameLabel);
            Name = "SoupSettingsItemNumUpDown";
            Size = new Size(400, 31);
            ((System.ComponentModel.ISupportInitialize)InputNumUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label ItemNameLabel;
        private NumericUpDown InputNumUpDown;
        private Button ResetButton;
    }
}
