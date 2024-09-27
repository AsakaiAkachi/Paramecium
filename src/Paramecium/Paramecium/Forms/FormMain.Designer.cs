namespace Paramecium.Forms
{
    partial class FormMain
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
            TopMenu = new MenuStrip();
            BottomStat = new StatusStrip();
            BottomStat_SoupState = new ToolStripStatusLabel();
            BottomStat_ElapsedTimeSteps = new ToolStripStatusLabel();
            BottomStat_Tps = new ToolStripStatusLabel();
            SoupView = new PictureBox();
            BottomStat_Fps = new ToolStripStatusLabel();
            BottomStat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SoupView).BeginInit();
            SuspendLayout();
            // 
            // TopMenu
            // 
            TopMenu.Location = new Point(0, 0);
            TopMenu.Name = "TopMenu";
            TopMenu.Size = new Size(1200, 24);
            TopMenu.TabIndex = 0;
            TopMenu.Text = "menuStrip1";
            // 
            // BottomStat
            // 
            BottomStat.Items.AddRange(new ToolStripItem[] { BottomStat_SoupState, BottomStat_ElapsedTimeSteps, BottomStat_Tps, BottomStat_Fps });
            BottomStat.Location = new Point(0, 822);
            BottomStat.Name = "BottomStat";
            BottomStat.Size = new Size(1200, 24);
            BottomStat.TabIndex = 1;
            BottomStat.Text = "statusStrip1";
            // 
            // BottomStat_SoupState
            // 
            BottomStat_SoupState.Name = "BottomStat_SoupState";
            BottomStat_SoupState.Size = new Size(79, 19);
            BottomStat_SoupState.Text = "Status : Pause";
            // 
            // BottomStat_ElapsedTimeSteps
            // 
            BottomStat_ElapsedTimeSteps.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_ElapsedTimeSteps.Name = "BottomStat_ElapsedTimeSteps";
            BottomStat_ElapsedTimeSteps.Size = new Size(100, 19);
            BottomStat_ElapsedTimeSteps.Text = "Time Step : 0 (T1)";
            // 
            // BottomStat_Tps
            // 
            BottomStat_Tps.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_Tps.Name = "BottomStat_Tps";
            BottomStat_Tps.Size = new Size(54, 19);
            BottomStat_Tps.Text = "TPS : 0.0";
            // 
            // SoupView
            // 
            SoupView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SoupView.BackColor = Color.Black;
            SoupView.Location = new Point(0, 24);
            SoupView.Name = "SoupView";
            SoupView.Size = new Size(1200, 800);
            SoupView.TabIndex = 2;
            SoupView.TabStop = false;
            SoupView.MouseDown += SoupView_MouseDown;
            SoupView.MouseMove += SoupView_MouseMove;
            SoupView.MouseUp += SoupView_MouseUp;
            // 
            // BottomStat_Fps
            // 
            BottomStat_Fps.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_Fps.Name = "BottomStat_Fps";
            BottomStat_Fps.Size = new Size(45, 19);
            BottomStat_Fps.Text = "FPS : 0";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 846);
            Controls.Add(BottomStat);
            Controls.Add(TopMenu);
            Controls.Add(SoupView);
            MainMenuStrip = TopMenu;
            Name = "FormMain";
            Text = "Paramecium";
            Shown += FormMain_Shown;
            KeyDown += FormMain_KeyDown;
            BottomStat.ResumeLayout(false);
            BottomStat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SoupView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip TopMenu;
        private StatusStrip BottomStat;
        private PictureBox SoupView;
        private ToolStripStatusLabel BottomStat_SoupState;
        private ToolStripStatusLabel BottomStat_ElapsedTimeSteps;
        private ToolStripStatusLabel BottomStat_Tps;
        private ToolStripStatusLabel BottomStat_Fps;
    }
}