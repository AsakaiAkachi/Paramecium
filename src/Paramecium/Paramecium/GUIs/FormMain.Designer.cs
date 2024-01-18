namespace Paramecium.GUIs
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SimulationView = new PictureBox();
            StatusStrip = new StatusStrip();
            MenuStrip = new MenuStrip();
            ToolStrip = new ToolStrip();
            ((System.ComponentModel.ISupportInitialize)SimulationView).BeginInit();
            SuspendLayout();
            // 
            // SimulationView
            // 
            SimulationView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SimulationView.BackColor = Color.Black;
            SimulationView.Location = new Point(0, 49);
            SimulationView.Name = "SimulationView";
            SimulationView.Size = new Size(900, 600);
            SimulationView.TabIndex = 0;
            SimulationView.TabStop = false;
            // 
            // StatusStrip
            // 
            StatusStrip.BackColor = Color.White;
            StatusStrip.Location = new Point(0, 649);
            StatusStrip.Name = "StatusStrip";
            StatusStrip.Size = new Size(900, 22);
            StatusStrip.TabIndex = 1;
            StatusStrip.Text = "statusStrip1";
            // 
            // MenuStrip
            // 
            MenuStrip.BackColor = Color.White;
            MenuStrip.Location = new Point(0, 0);
            MenuStrip.Name = "MenuStrip";
            MenuStrip.Size = new Size(900, 24);
            MenuStrip.TabIndex = 2;
            MenuStrip.Text = "menuStrip1";
            // 
            // ToolStrip
            // 
            ToolStrip.BackColor = Color.White;
            ToolStrip.Location = new Point(0, 24);
            ToolStrip.Name = "ToolStrip";
            ToolStrip.Size = new Size(900, 25);
            ToolStrip.TabIndex = 3;
            ToolStrip.Text = "toolStrip1";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 671);
            Controls.Add(ToolStrip);
            Controls.Add(StatusStrip);
            Controls.Add(MenuStrip);
            Controls.Add(SimulationView);
            MainMenuStrip = MenuStrip;
            Name = "FormMain";
            Text = "Paramecium";
            ((System.ComponentModel.ISupportInitialize)SimulationView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox SimulationView;
        private StatusStrip StatusStrip;
        private MenuStrip MenuStrip;
        private ToolStrip ToolStrip;
    }
}
