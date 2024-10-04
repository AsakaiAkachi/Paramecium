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
            TopMenu_Soup_EditSoupSettings = new ToolStripMenuItem();
            TopMenu_Help = new ToolStripMenuItem();
            TopMenu_Help_AboutParamecium = new ToolStripMenuItem();
            BottomStat = new StatusStrip();
            BottomStat_SoupState = new ToolStripStatusLabel();
            BottomStat_ElapsedTimeSteps = new ToolStripStatusLabel();
            BottomStat_Population = new ToolStripStatusLabel();
            BottomStat_LatestGeneration = new ToolStripStatusLabel();
            BottomStat_TotalBornDieCount = new ToolStripStatusLabel();
            BottomStat_Tps = new ToolStripStatusLabel();
            BottomStat_Fps = new ToolStripStatusLabel();
            SoupView = new PictureBox();
            SaveFileDialog_SaveSoup = new SaveFileDialog();
            OpenFileDialog_LoadSoup = new OpenFileDialog();
            TopMenuBar_Soup_EditAutosaveSettings = new ToolStripMenuItem();
            TopMenu.SuspendLayout();
            BottomStat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SoupView).BeginInit();
            SuspendLayout();
            // 
            // TopMenu
            // 
            TopMenu.Items.AddRange(new ToolStripItem[] { TopMenu_File, TopMenu_Soup, TopMenu_Help });
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
            TopMenu_File_New.Size = new Size(180, 22);
            TopMenu_File_New.Text = "New";
            TopMenu_File_New.Click += TopMenu_File_New_Click;
            // 
            // TopMenu_File_Open
            // 
            TopMenu_File_Open.Name = "TopMenu_File_Open";
            TopMenu_File_Open.Size = new Size(180, 22);
            TopMenu_File_Open.Text = "Open...";
            TopMenu_File_Open.Click += TopMenu_File_Open_Click;
            // 
            // TopMenu_File_Save
            // 
            TopMenu_File_Save.Name = "TopMenu_File_Save";
            TopMenu_File_Save.Size = new Size(180, 22);
            TopMenu_File_Save.Text = "Save";
            TopMenu_File_Save.Click += TopMenu_File_Save_Click;
            // 
            // TopMenu_File_SaveAs
            // 
            TopMenu_File_SaveAs.Name = "TopMenu_File_SaveAs";
            TopMenu_File_SaveAs.Size = new Size(180, 22);
            TopMenu_File_SaveAs.Text = "Save As...";
            TopMenu_File_SaveAs.Click += TopMenu_File_SaveAs_Click;
            // 
            // TopMenu_File_Separator1
            // 
            TopMenu_File_Separator1.Name = "TopMenu_File_Separator1";
            TopMenu_File_Separator1.Size = new Size(177, 6);
            // 
            // TopMenu_File_Exit
            // 
            TopMenu_File_Exit.Name = "TopMenu_File_Exit";
            TopMenu_File_Exit.Size = new Size(180, 22);
            TopMenu_File_Exit.Text = "Exit";
            TopMenu_File_Exit.Click += TopMenu_File_Exit_Click;
            // 
            // TopMenu_Soup
            // 
            TopMenu_Soup.DropDownItems.AddRange(new ToolStripItem[] { TopMenu_Soup_EditSoupSettings, TopMenuBar_Soup_EditAutosaveSettings });
            TopMenu_Soup.Name = "TopMenu_Soup";
            TopMenu_Soup.Size = new Size(46, 20);
            TopMenu_Soup.Text = "Soup";
            // 
            // TopMenu_Soup_EditSoupSettings
            // 
            TopMenu_Soup_EditSoupSettings.Name = "TopMenu_Soup_EditSoupSettings";
            TopMenu_Soup_EditSoupSettings.Size = new Size(180, 22);
            TopMenu_Soup_EditSoupSettings.Text = "Edit Soup Settings...";
            TopMenu_Soup_EditSoupSettings.Click += TopMenu_Soup_EditSoupSettings_Click;
            // 
            // TopMenu_Help
            // 
            TopMenu_Help.DropDownItems.AddRange(new ToolStripItem[] { TopMenu_Help_AboutParamecium });
            TopMenu_Help.Name = "TopMenu_Help";
            TopMenu_Help.Size = new Size(44, 20);
            TopMenu_Help.Text = "Help";
            // 
            // TopMenu_Help_AboutParamecium
            // 
            TopMenu_Help_AboutParamecium.Name = "TopMenu_Help_AboutParamecium";
            TopMenu_Help_AboutParamecium.Size = new Size(175, 22);
            TopMenu_Help_AboutParamecium.Text = "About Paramecium";
            TopMenu_Help_AboutParamecium.Click += TopMenu_Help_AboutParamecium_Click;
            // 
            // BottomStat
            // 
            BottomStat.Items.AddRange(new ToolStripItem[] { BottomStat_SoupState, BottomStat_ElapsedTimeSteps, BottomStat_Population, BottomStat_LatestGeneration, BottomStat_TotalBornDieCount, BottomStat_Tps, BottomStat_Fps });
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
            // BottomStat_Population
            // 
            BottomStat_Population.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_Population.Name = "BottomStat_Population";
            BottomStat_Population.Size = new Size(148, 19);
            BottomStat_Population.Text = "Population (P/A/T) : 0/0/0";
            // 
            // BottomStat_LatestGeneration
            // 
            BottomStat_LatestGeneration.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_LatestGeneration.Name = "BottomStat_LatestGeneration";
            BottomStat_LatestGeneration.Size = new Size(84, 19);
            BottomStat_LatestGeneration.Text = "Generation : 0";
            // 
            // BottomStat_TotalBornDieCount
            // 
            BottomStat_TotalBornDieCount.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_TotalBornDieCount.Name = "BottomStat_TotalBornDieCount";
            BottomStat_TotalBornDieCount.Size = new Size(112, 19);
            BottomStat_TotalBornDieCount.Text = "Total Born/Die : 0/0";
            // 
            // BottomStat_Tps
            // 
            BottomStat_Tps.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_Tps.Name = "BottomStat_Tps";
            BottomStat_Tps.Size = new Size(54, 19);
            BottomStat_Tps.Text = "TPS : 0.0";
            // 
            // BottomStat_Fps
            // 
            BottomStat_Fps.BorderSides = ToolStripStatusLabelBorderSides.Left;
            BottomStat_Fps.Name = "BottomStat_Fps";
            BottomStat_Fps.Size = new Size(45, 19);
            BottomStat_Fps.Text = "FPS : 0";
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
            // SaveFileDialog_SaveSoup
            // 
            SaveFileDialog_SaveSoup.Filter = "Paramecium Soup File|*.soup|All Files|*.*";
            SaveFileDialog_SaveSoup.Title = "Save As";
            // 
            // OpenFileDialog_LoadSoup
            // 
            OpenFileDialog_LoadSoup.Filter = "Paramecium Soup File|*.soup|All Files|*.*";
            OpenFileDialog_LoadSoup.Title = "Open";
            // 
            // TopMenuBar_Soup_EditAutosaveSettings
            // 
            TopMenuBar_Soup_EditAutosaveSettings.Name = "TopMenuBar_Soup_EditAutosaveSettings";
            TopMenuBar_Soup_EditAutosaveSettings.Size = new Size(180, 22);
            TopMenuBar_Soup_EditAutosaveSettings.Text = "Autosave Settings...";
            TopMenuBar_Soup_EditAutosaveSettings.Click += TopMenuBar_Soup_EditAutosaveSettings_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 846);
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
        private ToolStripStatusLabel BottomStat_ElapsedTimeSteps;
        private ToolStripStatusLabel BottomStat_Tps;
        private ToolStripStatusLabel BottomStat_Fps;
        private ToolStripStatusLabel BottomStat_Population;
        private ToolStripStatusLabel BottomStat_TotalBornDieCount;
        private ToolStripMenuItem TopMenu_File;
        private ToolStripMenuItem TopMenu_File_New;
        private ToolStripMenuItem TopMenu_File_Open;
        private ToolStripMenuItem TopMenu_File_Save;
        private ToolStripMenuItem TopMenu_File_SaveAs;
        private ToolStripSeparator TopMenu_File_Separator1;
        private ToolStripMenuItem TopMenu_File_Exit;
        private SaveFileDialog SaveFileDialog_SaveSoup;
        private OpenFileDialog OpenFileDialog_LoadSoup;
        private ToolStripStatusLabel BottomStat_LatestGeneration;
        private ToolStripMenuItem TopMenu_Soup;
        private ToolStripMenuItem TopMenu_Soup_EditSoupSettings;
        private ToolStripMenuItem TopMenu_Help;
        private ToolStripMenuItem TopMenu_Help_AboutParamecium;
        private ToolStripMenuItem TopMenuBar_Soup_EditAutosaveSettings;
    }
}