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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            TopMenu = new MenuStrip();
            TopMenu_File = new ToolStripMenuItem();
            TopMenu_File_New = new ToolStripMenuItem();
            TopMenu_File_Open = new ToolStripMenuItem();
            TopMenu_File_Save = new ToolStripMenuItem();
            TopMenu_File_SaveAs = new ToolStripMenuItem();
            TopMenu_File_Separator1 = new ToolStripSeparator();
            TopMenu_File_Exit = new ToolStripMenuItem();
            TopMenu_Soup = new ToolStripMenuItem();
            TopMenu_Soup_RunPause = new ToolStripMenuItem();
            TopMenu_Soup_StepRun = new ToolStripMenuItem();
            TopMenu_Soup_ThreadCountInc = new ToolStripMenuItem();
            TopMenu_Soup_ThreadCountDec = new ToolStripMenuItem();
            TopMenu_Soup_Separator1 = new ToolStripSeparator();
            TopMenu_Soup_EditSoupSettings = new ToolStripMenuItem();
            TopMenu_View = new ToolStripMenuItem();
            TopMenu_View_ZoomIn = new ToolStripMenuItem();
            TopMenu_View_ZoomOut = new ToolStripMenuItem();
            TopMenu_View_Reset = new ToolStripMenuItem();
            TopMenu_View_Separator1 = new ToolStripSeparator();
            TopMenu_View_TrackingSelectedCell = new ToolStripMenuItem();
            TopMenu_View_TrackingRandomAnimal = new ToolStripMenuItem();
            TopMenu_View_AutoSelectTrackedCells = new ToolStripMenuItem();
            BottomStat = new StatusStrip();
            BottomStat_SoupState = new ToolStripStatusLabel();
            BottomStat_TimeSteps = new ToolStripStatusLabel();
            BottomStat_Population = new ToolStripStatusLabel();
            BottomStat_Generation = new ToolStripStatusLabel();
            BottomStat_TotalBornDie = new ToolStripStatusLabel();
            BottomStat_Tps = new ToolStripStatusLabel();
            BottomStat_Fps = new ToolStripStatusLabel();
            SoupView = new PictureBox();
            LoadSoupDialog = new OpenFileDialog();
            SaveSoupDialog = new SaveFileDialog();
            TopMenu.SuspendLayout();
            BottomStat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SoupView).BeginInit();
            SuspendLayout();
            // 
            // TopMenu
            // 
            TopMenu.BackColor = Color.White;
            TopMenu.Items.AddRange(new ToolStripItem[] { TopMenu_File, TopMenu_Soup, TopMenu_View });
            TopMenu.Location = new Point(0, 0);
            TopMenu.Name = "TopMenu";
            TopMenu.Size = new Size(1200, 24);
            TopMenu.TabIndex = 0;
            TopMenu.Text = "menuStrip1";
            // 
            // TopMenu_File
            // 
            TopMenu_File.DropDownItems.AddRange(new ToolStripItem[] { TopMenu_File_New, TopMenu_File_Open, TopMenu_File_Save, TopMenu_File_SaveAs, TopMenu_File_Separator1, TopMenu_File_Exit });
            TopMenu_File.Name = "TopMenu_File";
            TopMenu_File.Size = new Size(37, 20);
            TopMenu_File.Text = "File";
            // 
            // TopMenu_File_New
            // 
            TopMenu_File_New.Name = "TopMenu_File_New";
            TopMenu_File_New.ShortcutKeyDisplayString = "Ctrl+N";
            TopMenu_File_New.Size = new Size(194, 22);
            TopMenu_File_New.Text = "New";
            TopMenu_File_New.Click += TopMenu_File_New_Click;
            // 
            // TopMenu_File_Open
            // 
            TopMenu_File_Open.Name = "TopMenu_File_Open";
            TopMenu_File_Open.ShortcutKeyDisplayString = "Ctrl+O";
            TopMenu_File_Open.Size = new Size(194, 22);
            TopMenu_File_Open.Text = "Open...";
            TopMenu_File_Open.Click += TopMenu_File_Open_Click;
            // 
            // TopMenu_File_Save
            // 
            TopMenu_File_Save.Enabled = false;
            TopMenu_File_Save.Name = "TopMenu_File_Save";
            TopMenu_File_Save.ShortcutKeyDisplayString = "Ctrl+S";
            TopMenu_File_Save.Size = new Size(194, 22);
            TopMenu_File_Save.Text = "Save";
            TopMenu_File_Save.Click += TopMenu_File_Save_Click;
            // 
            // TopMenu_File_SaveAs
            // 
            TopMenu_File_SaveAs.Enabled = false;
            TopMenu_File_SaveAs.Name = "TopMenu_File_SaveAs";
            TopMenu_File_SaveAs.ShortcutKeyDisplayString = "Ctrl+Shift+S";
            TopMenu_File_SaveAs.Size = new Size(194, 22);
            TopMenu_File_SaveAs.Text = "Save As...";
            TopMenu_File_SaveAs.Click += TopMenu_File_SaveAs_Click;
            // 
            // TopMenu_File_Separator1
            // 
            TopMenu_File_Separator1.Name = "TopMenu_File_Separator1";
            TopMenu_File_Separator1.Size = new Size(191, 6);
            // 
            // TopMenu_File_Exit
            // 
            TopMenu_File_Exit.Name = "TopMenu_File_Exit";
            TopMenu_File_Exit.Size = new Size(194, 22);
            TopMenu_File_Exit.Text = "Exit";
            TopMenu_File_Exit.Click += TopMenu_File_Exit_Click;
            // 
            // TopMenu_Soup
            // 
            TopMenu_Soup.DropDownItems.AddRange(new ToolStripItem[] { TopMenu_Soup_RunPause, TopMenu_Soup_StepRun, TopMenu_Soup_ThreadCountInc, TopMenu_Soup_ThreadCountDec, TopMenu_Soup_Separator1, TopMenu_Soup_EditSoupSettings });
            TopMenu_Soup.Enabled = false;
            TopMenu_Soup.Name = "TopMenu_Soup";
            TopMenu_Soup.Size = new Size(46, 20);
            TopMenu_Soup.Text = "Soup";
            // 
            // TopMenu_Soup_RunPause
            // 
            TopMenu_Soup_RunPause.Name = "TopMenu_Soup_RunPause";
            TopMenu_Soup_RunPause.ShortcutKeyDisplayString = "Space";
            TopMenu_Soup_RunPause.Size = new Size(256, 22);
            TopMenu_Soup_RunPause.Text = "Run/Pause";
            TopMenu_Soup_RunPause.Click += TopMenu_Soup_RunPause_Click;
            // 
            // TopMenu_Soup_StepRun
            // 
            TopMenu_Soup_StepRun.Name = "TopMenu_Soup_StepRun";
            TopMenu_Soup_StepRun.ShortcutKeyDisplayString = "Question";
            TopMenu_Soup_StepRun.Size = new Size(256, 22);
            TopMenu_Soup_StepRun.Text = "Step Run";
            TopMenu_Soup_StepRun.Click += TopMenu_Soup_StepRun_Click;
            // 
            // TopMenu_Soup_ThreadCountInc
            // 
            TopMenu_Soup_ThreadCountInc.Name = "TopMenu_Soup_ThreadCountInc";
            TopMenu_Soup_ThreadCountInc.ShortcutKeyDisplayString = "Period";
            TopMenu_Soup_ThreadCountInc.Size = new Size(256, 22);
            TopMenu_Soup_ThreadCountInc.Text = "Inc. Num of Threads by 1";
            TopMenu_Soup_ThreadCountInc.Click += TopMenu_Soup_ThreadCountInc_Click;
            // 
            // TopMenu_Soup_ThreadCountDec
            // 
            TopMenu_Soup_ThreadCountDec.Name = "TopMenu_Soup_ThreadCountDec";
            TopMenu_Soup_ThreadCountDec.ShortcutKeyDisplayString = "Comma";
            TopMenu_Soup_ThreadCountDec.Size = new Size(256, 22);
            TopMenu_Soup_ThreadCountDec.Text = "Dec. Num of Threads by 1";
            TopMenu_Soup_ThreadCountDec.Click += TopMenu_Soup_ThreadCountDec_Click;
            // 
            // TopMenu_Soup_Separator1
            // 
            TopMenu_Soup_Separator1.Name = "TopMenu_Soup_Separator1";
            TopMenu_Soup_Separator1.Size = new Size(253, 6);
            // 
            // TopMenu_Soup_EditSoupSettings
            // 
            TopMenu_Soup_EditSoupSettings.Name = "TopMenu_Soup_EditSoupSettings";
            TopMenu_Soup_EditSoupSettings.Size = new Size(256, 22);
            TopMenu_Soup_EditSoupSettings.Text = "Edit Soup Settings";
            TopMenu_Soup_EditSoupSettings.Click += TopMenu_Soup_EditSoupSettings_Click;
            // 
            // TopMenu_View
            // 
            TopMenu_View.DropDownItems.AddRange(new ToolStripItem[] { TopMenu_View_ZoomIn, TopMenu_View_ZoomOut, TopMenu_View_Reset, TopMenu_View_Separator1, TopMenu_View_TrackingSelectedCell, TopMenu_View_TrackingRandomAnimal, TopMenu_View_AutoSelectTrackedCells });
            TopMenu_View.Enabled = false;
            TopMenu_View.Name = "TopMenu_View";
            TopMenu_View.Size = new Size(44, 20);
            TopMenu_View.Text = "View";
            // 
            // TopMenu_View_ZoomIn
            // 
            TopMenu_View_ZoomIn.Name = "TopMenu_View_ZoomIn";
            TopMenu_View_ZoomIn.ShortcutKeyDisplayString = "Ctrl+LClick";
            TopMenu_View_ZoomIn.Size = new Size(219, 22);
            TopMenu_View_ZoomIn.Text = "Zoom In";
            TopMenu_View_ZoomIn.Click += TopMenu_View_ZoomIn_Click;
            // 
            // TopMenu_View_ZoomOut
            // 
            TopMenu_View_ZoomOut.Name = "TopMenu_View_ZoomOut";
            TopMenu_View_ZoomOut.ShortcutKeyDisplayString = "Ctrl+RClick";
            TopMenu_View_ZoomOut.Size = new Size(219, 22);
            TopMenu_View_ZoomOut.Text = "Zoom Out";
            TopMenu_View_ZoomOut.Click += TopMenu_View_ZoomOut_Click;
            // 
            // TopMenu_View_Reset
            // 
            TopMenu_View_Reset.Name = "TopMenu_View_Reset";
            TopMenu_View_Reset.ShortcutKeyDisplayString = "C";
            TopMenu_View_Reset.Size = new Size(219, 22);
            TopMenu_View_Reset.Text = "Reset Position and Zoom";
            TopMenu_View_Reset.Click += TopMenu_View_Reset_Click;
            // 
            // TopMenu_View_Separator1
            // 
            TopMenu_View_Separator1.Name = "TopMenu_View_Separator1";
            TopMenu_View_Separator1.Size = new Size(216, 6);
            // 
            // TopMenu_View_TrackingSelectedCell
            // 
            TopMenu_View_TrackingSelectedCell.Name = "TopMenu_View_TrackingSelectedCell";
            TopMenu_View_TrackingSelectedCell.ShortcutKeyDisplayString = "T";
            TopMenu_View_TrackingSelectedCell.Size = new Size(219, 22);
            TopMenu_View_TrackingSelectedCell.Text = "Tracking Selected Cell";
            TopMenu_View_TrackingSelectedCell.Click += TopMenu_View_TrackingSelectedCell_Click;
            // 
            // TopMenu_View_TrackingRandomAnimal
            // 
            TopMenu_View_TrackingRandomAnimal.Name = "TopMenu_View_TrackingRandomAnimal";
            TopMenu_View_TrackingRandomAnimal.ShortcutKeyDisplayString = "R";
            TopMenu_View_TrackingRandomAnimal.Size = new Size(219, 22);
            TopMenu_View_TrackingRandomAnimal.Text = "Tracking Random Animal";
            TopMenu_View_TrackingRandomAnimal.Click += TopMenu_View_TrackingRandomAnimal_Click;
            // 
            // TopMenu_View_AutoSelectTrackedCells
            // 
            TopMenu_View_AutoSelectTrackedCells.Name = "TopMenu_View_AutoSelectTrackedCells";
            TopMenu_View_AutoSelectTrackedCells.Size = new Size(219, 22);
            TopMenu_View_AutoSelectTrackedCells.Text = "Auto Select Tracked Cells";
            TopMenu_View_AutoSelectTrackedCells.Click += TopMenu_View_AutoSelectTrackedCells_Click;
            // 
            // BottomStat
            // 
            BottomStat.BackColor = Color.White;
            BottomStat.Items.AddRange(new ToolStripItem[] { BottomStat_SoupState, BottomStat_TimeSteps, BottomStat_Population, BottomStat_Generation, BottomStat_TotalBornDie, BottomStat_Tps, BottomStat_Fps });
            BottomStat.Location = new Point(0, 824);
            BottomStat.Name = "BottomStat";
            BottomStat.Size = new Size(1200, 24);
            BottomStat.TabIndex = 1;
            BottomStat.Text = "statusStrip1";
            // 
            // BottomStat_SoupState
            // 
            BottomStat_SoupState.Name = "BottomStat_SoupState";
            BottomStat_SoupState.Size = new Size(92, 19);
            BottomStat_SoupState.Text = "Status : Stopped";
            // 
            // BottomStat_TimeSteps
            // 
            BottomStat_TimeSteps.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_TimeSteps.BorderStyle = Border3DStyle.Raised;
            BottomStat_TimeSteps.Name = "BottomStat_TimeSteps";
            BottomStat_TimeSteps.Size = new Size(100, 19);
            BottomStat_TimeSteps.Text = "Time Step : 0 (T1)";
            // 
            // BottomStat_Population
            // 
            BottomStat_Population.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_Population.BorderStyle = Border3DStyle.Raised;
            BottomStat_Population.Name = "BottomStat_Population";
            BottomStat_Population.Size = new Size(148, 19);
            BottomStat_Population.Text = "Population (P/A/T) : 0/0/0";
            // 
            // BottomStat_Generation
            // 
            BottomStat_Generation.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_Generation.BorderStyle = Border3DStyle.Raised;
            BottomStat_Generation.Name = "BottomStat_Generation";
            BottomStat_Generation.Size = new Size(84, 19);
            BottomStat_Generation.Text = "Generation : 0";
            // 
            // BottomStat_TotalBornDie
            // 
            BottomStat_TotalBornDie.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_TotalBornDie.BorderStyle = Border3DStyle.Raised;
            BottomStat_TotalBornDie.Name = "BottomStat_TotalBornDie";
            BottomStat_TotalBornDie.Size = new Size(112, 19);
            BottomStat_TotalBornDie.Text = "Total Born/Die : 0/0";
            // 
            // BottomStat_Tps
            // 
            BottomStat_Tps.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_Tps.BorderStyle = Border3DStyle.Raised;
            BottomStat_Tps.Name = "BottomStat_Tps";
            BottomStat_Tps.Size = new Size(113, 19);
            BottomStat_Tps.Text = "TPS : 0.0 (0.0000ms)";
            // 
            // BottomStat_Fps
            // 
            BottomStat_Fps.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_Fps.BorderStyle = Border3DStyle.Raised;
            BottomStat_Fps.Name = "BottomStat_Fps";
            BottomStat_Fps.Size = new Size(113, 19);
            BottomStat_Fps.Text = "FPS : 0.0 (0.0000ms)";
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
            // LoadSoupDialog
            // 
            LoadSoupDialog.Filter = "Paramecium Soup File|*.soup|All FIles|*.*";
            // 
            // SaveSoupDialog
            // 
            SaveSoupDialog.FileName = "untitled.soup";
            SaveSoupDialog.Filter = "Paramecium Soup File|*.soup|All FIles|*.*";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1200, 848);
            Controls.Add(BottomStat);
            Controls.Add(TopMenu);
            Controls.Add(SoupView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = TopMenu;
            Name = "FormMain";
            Text = "Paramecium";
            FormClosing += FormMain_FormClosing;
            Shown += FormMain_Shown;
            KeyDown += FormMain_KeyDown;
            TopMenu.ResumeLayout(false);
            TopMenu.PerformLayout();
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
        private ToolStripStatusLabel BottomStat_TimeSteps;
        private ToolStripStatusLabel BottomStat_Population;
        private ToolStripStatusLabel BottomStat_Generation;
        private ToolStripStatusLabel BottomStat_TotalBornDie;
        private ToolStripStatusLabel BottomStat_Tps;
        private ToolStripStatusLabel BottomStat_Fps;
        private ToolStripMenuItem TopMenu_File;
        private ToolStripMenuItem TopMenu_File_New;
        private ToolStripMenuItem TopMenu_File_Open;
        private ToolStripMenuItem TopMenu_File_Save;
        private ToolStripMenuItem TopMenu_File_SaveAs;
        private ToolStripSeparator TopMenu_File_Separator1;
        private ToolStripMenuItem TopMenu_File_Exit;
        private ToolStripMenuItem TopMenu_Soup;
        private ToolStripMenuItem TopMenu_Soup_EditSoupSettings;
        private ToolStripSeparator TopMenu_Soup_Separator1;
        private ToolStripMenuItem TopMenu_Soup_RunPause;
        private ToolStripMenuItem TopMenu_Soup_StepRun;
        private ToolStripMenuItem TopMenu_Soup_ThreadCountInc;
        private ToolStripMenuItem TopMenu_Soup_ThreadCountDec;
        private ToolStripMenuItem TopMenu_View;
        private ToolStripMenuItem TopMenu_View_ZoomIn;
        private ToolStripMenuItem TopMenu_View_ZoomOut;
        private ToolStripMenuItem TopMenu_View_Reset;
        private OpenFileDialog LoadSoupDialog;
        private SaveFileDialog SaveSoupDialog;
        private ToolStripSeparator TopMenu_View_Separator1;
        private ToolStripMenuItem TopMenu_View_TrackingSelectedCell;
        private ToolStripMenuItem TopMenu_View_TrackingRandomAnimal;
        private ToolStripMenuItem TopMenu_View_AutoSelectTrackedCells;
    }
}