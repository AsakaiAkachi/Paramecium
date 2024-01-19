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
            StatusStrip_SoupStatus = new ToolStripStatusLabel();
            StatusStrip_TimeStep = new ToolStripStatusLabel();
            StatusStrip_Population = new ToolStripStatusLabel();
            StatusStrip_Generation = new ToolStripStatusLabel();
            StatusStrip_Born = new ToolStripStatusLabel();
            StatusStrip_Die = new ToolStripStatusLabel();
            StatusStrip_TotalBiomass = new ToolStripStatusLabel();
            StatusStrip_TPS = new ToolStripStatusLabel();
            StatusStrip_FPS = new ToolStripStatusLabel();
            MenuStrip = new MenuStrip();
            MenuStrip_File = new ToolStripMenuItem();
            MenuStrip_File_New = new ToolStripMenuItem();
            MenuStrip_File_Open = new ToolStripMenuItem();
            MenuStrip_File_Save = new ToolStripMenuItem();
            MenuStrip_File_SaveAs = new ToolStripMenuItem();
            MenuStrip_File_Separator1 = new ToolStripSeparator();
            MenuStrip_File_Exit = new ToolStripMenuItem();
            MenuStrip_Soup = new ToolStripMenuItem();
            MenuStrip_Soup_NewSoup = new ToolStripMenuItem();
            MenuStrip_Soup_EditSoupConstant = new ToolStripMenuItem();
            MenuStrip_Soup_Window = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)SimulationView).BeginInit();
            StatusStrip.SuspendLayout();
            MenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // SimulationView
            // 
            SimulationView.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SimulationView.BackColor = Color.Black;
            SimulationView.Location = new Point(0, 24);
            SimulationView.Name = "SimulationView";
            SimulationView.Size = new Size(900, 600);
            SimulationView.TabIndex = 0;
            SimulationView.TabStop = false;
            // 
            // StatusStrip
            // 
            StatusStrip.BackColor = Color.White;
            StatusStrip.Items.AddRange(new ToolStripItem[] { StatusStrip_SoupStatus, StatusStrip_TimeStep, StatusStrip_Population, StatusStrip_Generation, StatusStrip_Born, StatusStrip_Die, StatusStrip_TotalBiomass, StatusStrip_TPS, StatusStrip_FPS });
            StatusStrip.Location = new Point(0, 622);
            StatusStrip.Name = "StatusStrip";
            StatusStrip.Size = new Size(900, 24);
            StatusStrip.TabIndex = 1;
            StatusStrip.Text = "statusStrip1";
            // 
            // StatusStrip_SoupStatus
            // 
            StatusStrip_SoupStatus.Name = "StatusStrip_SoupStatus";
            StatusStrip_SoupStatus.Size = new Size(79, 19);
            StatusStrip_SoupStatus.Text = "Status : Pause";
            // 
            // StatusStrip_TimeStep
            // 
            StatusStrip_TimeStep.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatusStrip_TimeStep.Name = "StatusStrip_TimeStep";
            StatusStrip_TimeStep.Size = new Size(77, 19);
            StatusStrip_TimeStep.Text = "Time Step : 0";
            // 
            // StatusStrip_Population
            // 
            StatusStrip_Population.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatusStrip_Population.Name = "StatusStrip_Population";
            StatusStrip_Population.Size = new Size(84, 19);
            StatusStrip_Population.Text = "Population : 0";
            // 
            // StatusStrip_Generation
            // 
            StatusStrip_Generation.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatusStrip_Generation.Name = "StatusStrip_Generation";
            StatusStrip_Generation.Size = new Size(84, 19);
            StatusStrip_Generation.Text = "Generation : 0";
            // 
            // StatusStrip_Born
            // 
            StatusStrip_Born.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatusStrip_Born.Name = "StatusStrip_Born";
            StatusStrip_Born.Size = new Size(51, 19);
            StatusStrip_Born.Text = "Born : 0";
            // 
            // StatusStrip_Die
            // 
            StatusStrip_Die.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatusStrip_Die.Name = "StatusStrip_Die";
            StatusStrip_Die.Size = new Size(43, 19);
            StatusStrip_Die.Text = "Die : 0";
            // 
            // StatusStrip_TotalBiomass
            // 
            StatusStrip_TotalBiomass.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatusStrip_TotalBiomass.Name = "StatusStrip_TotalBiomass";
            StatusStrip_TotalBiomass.Size = new Size(97, 19);
            StatusStrip_TotalBiomass.Text = "Total Biomass : 0";
            // 
            // StatusStrip_TPS
            // 
            StatusStrip_TPS.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatusStrip_TPS.Name = "StatusStrip_TPS";
            StatusStrip_TPS.Size = new Size(45, 19);
            StatusStrip_TPS.Text = "TPS : 0";
            // 
            // StatusStrip_FPS
            // 
            StatusStrip_FPS.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatusStrip_FPS.Name = "StatusStrip_FPS";
            StatusStrip_FPS.Size = new Size(45, 19);
            StatusStrip_FPS.Text = "FPS : 0";
            // 
            // MenuStrip
            // 
            MenuStrip.BackColor = Color.White;
            MenuStrip.Items.AddRange(new ToolStripItem[] { MenuStrip_File, MenuStrip_Soup, MenuStrip_Soup_Window });
            MenuStrip.Location = new Point(0, 0);
            MenuStrip.Name = "MenuStrip";
            MenuStrip.Size = new Size(900, 24);
            MenuStrip.TabIndex = 2;
            MenuStrip.Text = "menuStrip1";
            // 
            // MenuStrip_File
            // 
            MenuStrip_File.DropDownItems.AddRange(new ToolStripItem[] { MenuStrip_File_New, MenuStrip_File_Open, MenuStrip_File_Save, MenuStrip_File_SaveAs, MenuStrip_File_Separator1, MenuStrip_File_Exit });
            MenuStrip_File.Name = "MenuStrip_File";
            MenuStrip_File.Size = new Size(37, 20);
            MenuStrip_File.Text = "File";
            // 
            // MenuStrip_File_New
            // 
            MenuStrip_File_New.Name = "MenuStrip_File_New";
            MenuStrip_File_New.Size = new Size(121, 22);
            MenuStrip_File_New.Text = "New...";
            // 
            // MenuStrip_File_Open
            // 
            MenuStrip_File_Open.Name = "MenuStrip_File_Open";
            MenuStrip_File_Open.Size = new Size(121, 22);
            MenuStrip_File_Open.Text = "Open...";
            // 
            // MenuStrip_File_Save
            // 
            MenuStrip_File_Save.Name = "MenuStrip_File_Save";
            MenuStrip_File_Save.Size = new Size(121, 22);
            MenuStrip_File_Save.Text = "Save";
            // 
            // MenuStrip_File_SaveAs
            // 
            MenuStrip_File_SaveAs.Name = "MenuStrip_File_SaveAs";
            MenuStrip_File_SaveAs.Size = new Size(121, 22);
            MenuStrip_File_SaveAs.Text = "Save as...";
            // 
            // MenuStrip_File_Separator1
            // 
            MenuStrip_File_Separator1.Name = "MenuStrip_File_Separator1";
            MenuStrip_File_Separator1.Size = new Size(118, 6);
            // 
            // MenuStrip_File_Exit
            // 
            MenuStrip_File_Exit.Name = "MenuStrip_File_Exit";
            MenuStrip_File_Exit.Size = new Size(121, 22);
            MenuStrip_File_Exit.Text = "Exit";
            // 
            // MenuStrip_Soup
            // 
            MenuStrip_Soup.DropDownItems.AddRange(new ToolStripItem[] { MenuStrip_Soup_NewSoup, MenuStrip_Soup_EditSoupConstant });
            MenuStrip_Soup.Name = "MenuStrip_Soup";
            MenuStrip_Soup.Size = new Size(46, 20);
            MenuStrip_Soup.Text = "Soup";
            // 
            // MenuStrip_Soup_NewSoup
            // 
            MenuStrip_Soup_NewSoup.Name = "MenuStrip_Soup_NewSoup";
            MenuStrip_Soup_NewSoup.Size = new Size(181, 22);
            MenuStrip_Soup_NewSoup.Text = "New Soup...";
            // 
            // MenuStrip_Soup_EditSoupConstant
            // 
            MenuStrip_Soup_EditSoupConstant.Name = "MenuStrip_Soup_EditSoupConstant";
            MenuStrip_Soup_EditSoupConstant.Size = new Size(181, 22);
            MenuStrip_Soup_EditSoupConstant.Text = "Edit soup constant...";
            // 
            // MenuStrip_Soup_Window
            // 
            MenuStrip_Soup_Window.Name = "MenuStrip_Soup_Window";
            MenuStrip_Soup_Window.Size = new Size(63, 20);
            MenuStrip_Soup_Window.Text = "Window";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 646);
            Controls.Add(StatusStrip);
            Controls.Add(MenuStrip);
            Controls.Add(SimulationView);
            MainMenuStrip = MenuStrip;
            Name = "FormMain";
            Text = "Paramecium";
            Shown += FormMain_Shown;
            ((System.ComponentModel.ISupportInitialize)SimulationView).EndInit();
            StatusStrip.ResumeLayout(false);
            StatusStrip.PerformLayout();
            MenuStrip.ResumeLayout(false);
            MenuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox SimulationView;
        private StatusStrip StatusStrip;
        private MenuStrip MenuStrip;
        private ToolStripMenuItem MenuStrip_File;
        private ToolStripMenuItem MenuStrip_File_Open;
        private ToolStripMenuItem MenuStrip_File_Save;
        private ToolStripMenuItem MenuStrip_File_SaveAs;
        private ToolStripSeparator MenuStrip_File_Separator1;
        private ToolStripMenuItem MenuStrip_File_Exit;
        private ToolStripMenuItem MenuStrip_File_New;
        private ToolStripMenuItem MenuStrip_Soup;
        private ToolStripMenuItem MenuStrip_Soup_NewSoup;
        private ToolStripMenuItem MenuStrip_Soup_EditSoupConstant;
        private ToolStripMenuItem MenuStrip_Soup_Window;
        private ToolStripStatusLabel StatusStrip_SoupStatus;
        private ToolStripStatusLabel StatusStrip_Population;
        private ToolStripStatusLabel StatusStrip_Generation;
        private ToolStripStatusLabel StatusStrip_TimeStep;
        private ToolStripStatusLabel StatusStrip_Born;
        private ToolStripStatusLabel StatusStrip_TotalBiomass;
        private ToolStripStatusLabel StatusStrip_Die;
        private ToolStripStatusLabel StatusStrip_TPS;
        private ToolStripStatusLabel StatusStrip_FPS;
    }
}
