namespace Paramecium.Forms.Controls
{
    partial class SoupSettingsLabel
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
            LabelText = new Label();
            SuspendLayout();
            // 
            // LabelText
            // 
            LabelText.AutoSize = true;
            LabelText.Location = new Point(8, 8);
            LabelText.Name = "LabelText";
            LabelText.Size = new Size(59, 15);
            LabelText.TabIndex = 0;
            LabelText.Text = "Label Text";
            // 
            // SoupSettingsLabel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(LabelText);
            Name = "SoupSettingsLabel";
            Size = new Size(400, 31);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LabelText;
    }
}
