namespace Paramecium.Forms
{
    partial class FormObjectEditor
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
            panel2 = new Panel();
            LabelTargetObject = new Label();
            RichTextBoxEditingObjectRawJson = new RichTextBox();
            panel3 = new Panel();
            ButtonSave = new Button();
            ButtonReload = new Button();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.Controls.Add(LabelTargetObject);
            panel2.Controls.Add(RichTextBoxEditingObjectRawJson);
            panel2.Location = new Point(0, 0);
            panel2.Margin = new Padding(0);
            panel2.Name = "panel2";
            panel2.Size = new Size(530, 660);
            panel2.TabIndex = 1;
            // 
            // LabelTargetObject
            // 
            LabelTargetObject.AutoSize = true;
            LabelTargetObject.Location = new Point(15, 15);
            LabelTargetObject.Name = "LabelTargetObject";
            LabelTargetObject.Size = new Size(167, 15);
            LabelTargetObject.TabIndex = 1;
            LabelTargetObject.Text = "Currently Edited Object : None";
            // 
            // RichTextBoxEditingObjectRawJson
            // 
            RichTextBoxEditingObjectRawJson.Font = new Font("MS UI Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            RichTextBoxEditingObjectRawJson.Location = new Point(15, 45);
            RichTextBoxEditingObjectRawJson.Name = "RichTextBoxEditingObjectRawJson";
            RichTextBoxEditingObjectRawJson.Size = new Size(500, 600);
            RichTextBoxEditingObjectRawJson.TabIndex = 0;
            RichTextBoxEditingObjectRawJson.Text = "";
            // 
            // panel3
            // 
            panel3.Controls.Add(ButtonReload);
            panel3.Controls.Add(ButtonSave);
            panel3.Location = new Point(0, 660);
            panel3.Name = "panel3";
            panel3.Size = new Size(530, 60);
            panel3.TabIndex = 2;
            // 
            // ButtonSave
            // 
            ButtonSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ButtonSave.Location = new Point(395, 15);
            ButtonSave.Margin = new Padding(0);
            ButtonSave.Name = "ButtonSave";
            ButtonSave.Size = new Size(120, 30);
            ButtonSave.TabIndex = 2;
            ButtonSave.Text = "Save";
            ButtonSave.UseVisualStyleBackColor = true;
            ButtonSave.Click += ButtonSave_Click;
            // 
            // ButtonReload
            // 
            ButtonReload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ButtonReload.Location = new Point(260, 15);
            ButtonReload.Margin = new Padding(0);
            ButtonReload.Name = "ButtonReload";
            ButtonReload.Size = new Size(120, 30);
            ButtonReload.TabIndex = 3;
            ButtonReload.Text = "Reload";
            ButtonReload.UseVisualStyleBackColor = true;
            ButtonReload.Click += ButtonReload_Click;
            // 
            // FormObjectEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(530, 720);
            Controls.Add(panel3);
            Controls.Add(panel2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormObjectEditor";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Object Editor";
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Panel panel2;
        private RichTextBox RichTextBoxEditingObjectRawJson;
        private Panel panel3;
        private Button ButtonSave;
        private Label LabelTargetObject;
        private Button ButtonReload;
    }
}