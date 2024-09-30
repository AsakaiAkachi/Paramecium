namespace Paramecium.Forms
{
    partial class FormInspector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInspector));
            ComboBoxObjectType = new ComboBox();
            LabelObjectType = new Label();
            NumericUpDownObjectIndex = new NumericUpDown();
            LabelObjectIndex = new Label();
            ButtonInspect = new Button();
            RichTextBoxInspectResult = new RichTextBox();
            ButtonSave = new Button();
            ((System.ComponentModel.ISupportInitialize)NumericUpDownObjectIndex).BeginInit();
            SuspendLayout();
            // 
            // ComboBoxObjectType
            // 
            ComboBoxObjectType.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxObjectType.FormattingEnabled = true;
            ComboBoxObjectType.Items.AddRange(new object[] { resources.GetString("ComboBoxObjectType.Items"), resources.GetString("ComboBoxObjectType.Items1"), resources.GetString("ComboBoxObjectType.Items2") });
            resources.ApplyResources(ComboBoxObjectType, "ComboBoxObjectType");
            ComboBoxObjectType.Name = "ComboBoxObjectType";
            ComboBoxObjectType.SelectedIndexChanged += ComboBoxObjectType_SelectedIndexChanged;
            // 
            // LabelObjectType
            // 
            resources.ApplyResources(LabelObjectType, "LabelObjectType");
            LabelObjectType.Name = "LabelObjectType";
            // 
            // NumericUpDownObjectIndex
            // 
            resources.ApplyResources(NumericUpDownObjectIndex, "NumericUpDownObjectIndex");
            NumericUpDownObjectIndex.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            NumericUpDownObjectIndex.Name = "NumericUpDownObjectIndex";
            // 
            // LabelObjectIndex
            // 
            resources.ApplyResources(LabelObjectIndex, "LabelObjectIndex");
            LabelObjectIndex.Name = "LabelObjectIndex";
            // 
            // ButtonInspect
            // 
            resources.ApplyResources(ButtonInspect, "ButtonInspect");
            ButtonInspect.Name = "ButtonInspect";
            ButtonInspect.UseVisualStyleBackColor = true;
            ButtonInspect.Click += ButtonInspect_Click;
            // 
            // RichTextBoxInspectResult
            // 
            resources.ApplyResources(RichTextBoxInspectResult, "RichTextBoxInspectResult");
            RichTextBoxInspectResult.Name = "RichTextBoxInspectResult";
            // 
            // ButtonSave
            // 
            resources.ApplyResources(ButtonSave, "ButtonSave");
            ButtonSave.Name = "ButtonSave";
            ButtonSave.UseVisualStyleBackColor = true;
            ButtonSave.Click += ButtonSave_Click;
            // 
            // FormInspector
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ButtonSave);
            Controls.Add(RichTextBoxInspectResult);
            Controls.Add(ButtonInspect);
            Controls.Add(LabelObjectIndex);
            Controls.Add(NumericUpDownObjectIndex);
            Controls.Add(LabelObjectType);
            Controls.Add(ComboBoxObjectType);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FormInspector";
            Shown += FormInspector_Shown;
            ((System.ComponentModel.ISupportInitialize)NumericUpDownObjectIndex).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox ComboBoxObjectType;
        private Label LabelObjectType;
        private NumericUpDown NumericUpDownObjectIndex;
        private Label LabelObjectIndex;
        private Button ButtonInspect;
        private RichTextBox RichTextBoxInspectResult;
        private Button ButtonSave;
    }
}