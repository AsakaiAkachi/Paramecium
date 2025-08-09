namespace Paramecium.Forms.Controls
{
    partial class SoupSettingsItemCheckbox
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
            ResetButton = new Button();
            ItemNameLabel = new Label();
            InputCheckBox = new CheckBox();
            SuspendLayout();
            // 
            // ResetButton
            // 
            ResetButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ResetButton.Font = new Font("Segoe MDL2 Assets", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ResetButton.Location = new Point(373, 4);
            ResetButton.Name = "ResetButton";
            ResetButton.Size = new Size(23, 23);
            ResetButton.TabIndex = 5;
            ResetButton.Text = "";
            ResetButton.UseVisualStyleBackColor = true;
            ResetButton.Click += ResetButton_Click;
            // 
            // ItemNameLabel
            // 
            ItemNameLabel.AutoSize = true;
            ItemNameLabel.Location = new Point(8, 8);
            ItemNameLabel.Name = "ItemNameLabel";
            ItemNameLabel.Size = new Size(64, 15);
            ItemNameLabel.TabIndex = 3;
            ItemNameLabel.Text = "Item Name";
            // 
            // InputCheckBox
            // 
            InputCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            InputCheckBox.AutoSize = true;
            InputCheckBox.Location = new Point(355, 9);
            InputCheckBox.Margin = new Padding(3, 3, 0, 3);
            InputCheckBox.Name = "InputCheckBox";
            InputCheckBox.Size = new Size(15, 14);
            InputCheckBox.TabIndex = 6;
            InputCheckBox.UseVisualStyleBackColor = true;
            // 
            // SoupSettingsItemCheckbox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(InputCheckBox);
            Controls.Add(ResetButton);
            Controls.Add(ItemNameLabel);
            Name = "SoupSettingsItemCheckbox";
            Size = new Size(400, 31);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ResetButton;
        private Label ItemNameLabel;
        private CheckBox InputCheckBox;
    }
}
