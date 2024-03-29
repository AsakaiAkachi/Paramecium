﻿namespace Paramecium.Forms
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
            BottomStat = new StatusStrip();
            StatSoupStatus = new ToolStripStatusLabel();
            StatTimeStep = new ToolStripStatusLabel();
            StatPopulation = new ToolStripStatusLabel();
            StatLatestGeneration = new ToolStripStatusLabel();
            StatTotalBornDie = new ToolStripStatusLabel();
            StatTps = new ToolStripStatusLabel();
            StatFps = new ToolStripStatusLabel();
            StatMemory = new ToolStripStatusLabel();
            TopMenu = new MenuStrip();
            TopMenu_File = new ToolStripMenuItem();
            TopMenu_File_Open = new ToolStripMenuItem();
            TopMenu_File_Save = new ToolStripMenuItem();
            TopMenu_File_SaveAs = new ToolStripMenuItem();
            TopMenu_File_Separator1 = new ToolStripSeparator();
            TopMenu_File_Exit = new ToolStripMenuItem();
            TopMenu_Window = new ToolStripMenuItem();
            TopMenu_Simulation = new ToolStripMenuItem();
            TopMenu_Simulation_NewSimulation = new ToolStripMenuItem();
            SimulationView = new PictureBox();
            TopMenu_Simulation_AutoSaveSettings = new ToolStripMenuItem();
            BottomStat.SuspendLayout();
            TopMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SimulationView).BeginInit();
            SuspendLayout();
            // 
            // BottomStat
            // 
            BottomStat.Items.AddRange(new ToolStripItem[] { StatSoupStatus, StatTimeStep, StatPopulation, StatLatestGeneration, StatTotalBornDie, StatTps, StatFps, StatMemory });
            BottomStat.Location = new Point(0, 622);
            BottomStat.Name = "BottomStat";
            BottomStat.Size = new Size(900, 24);
            BottomStat.TabIndex = 0;
            BottomStat.Text = "statusStrip1";
            // 
            // StatSoupStatus
            // 
            StatSoupStatus.Name = "StatSoupStatus";
            StatSoupStatus.Size = new Size(79, 19);
            StatSoupStatus.Text = "Status : Pause";
            // 
            // StatTimeStep
            // 
            StatTimeStep.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatTimeStep.Name = "StatTimeStep";
            StatTimeStep.Size = new Size(100, 19);
            StatTimeStep.Text = "Time Step : 0 (T0)";
            // 
            // StatPopulation
            // 
            StatPopulation.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatPopulation.Name = "StatPopulation";
            StatPopulation.Size = new Size(148, 19);
            StatPopulation.Text = "Population (P/A/T) : 0/0/0";
            // 
            // StatLatestGeneration
            // 
            StatLatestGeneration.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatLatestGeneration.Name = "StatLatestGeneration";
            StatLatestGeneration.Size = new Size(118, 19);
            StatLatestGeneration.Text = "Latest Generation : 0";
            // 
            // StatTotalBornDie
            // 
            StatTotalBornDie.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatTotalBornDie.Name = "StatTotalBornDie";
            StatTotalBornDie.Size = new Size(112, 19);
            StatTotalBornDie.Text = "Total Born/Die : 0/0";
            // 
            // StatTps
            // 
            StatTps.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatTps.Name = "StatTps";
            StatTps.Size = new Size(45, 19);
            StatTps.Text = "TPS : 0";
            // 
            // StatFps
            // 
            StatFps.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatFps.Name = "StatFps";
            StatFps.Size = new Size(45, 19);
            StatFps.Text = "FPS : 0";
            // 
            // StatMemory
            // 
            StatMemory.BorderSides = ToolStripStatusLabelBorderSides.Left;
            StatMemory.Name = "StatMemory";
            StatMemory.Size = new Size(112, 19);
            StatMemory.Text = "Memory : 0 Mbytes";
            // 
            // TopMenu
            // 
            TopMenu.Items.AddRange(new ToolStripItem[] { TopMenu_File, TopMenu_Window, TopMenu_Simulation });
            TopMenu.Location = new Point(0, 0);
            TopMenu.Name = "TopMenu";
            TopMenu.Size = new Size(900, 24);
            TopMenu.TabIndex = 1;
            TopMenu.Text = "menuStrip1";
            // 
            // TopMenu_File
            // 
            TopMenu_File.DropDownItems.AddRange(new ToolStripItem[] { TopMenu_File_Open, TopMenu_File_Save, TopMenu_File_SaveAs, TopMenu_File_Separator1, TopMenu_File_Exit });
            TopMenu_File.Name = "TopMenu_File";
            TopMenu_File.Size = new Size(37, 20);
            TopMenu_File.Text = "File";
            // 
            // TopMenu_File_Open
            // 
            TopMenu_File_Open.Name = "TopMenu_File_Open";
            TopMenu_File_Open.Size = new Size(137, 22);
            TopMenu_File_Open.Text = "Open...";
            TopMenu_File_Open.Click += TopMenu_File_Open_Click;
            // 
            // TopMenu_File_Save
            // 
            TopMenu_File_Save.Name = "TopMenu_File_Save";
            TopMenu_File_Save.ShortcutKeyDisplayString = "Crtl+S";
            TopMenu_File_Save.Size = new Size(137, 22);
            TopMenu_File_Save.Text = "Save";
            TopMenu_File_Save.Click += TopMenu_File_Save_Click;
            // 
            // TopMenu_File_SaveAs
            // 
            TopMenu_File_SaveAs.Name = "TopMenu_File_SaveAs";
            TopMenu_File_SaveAs.Size = new Size(137, 22);
            TopMenu_File_SaveAs.Text = "Save As...";
            TopMenu_File_SaveAs.Click += TopMenu_File_SaveAs_Click;
            // 
            // TopMenu_File_Separator1
            // 
            TopMenu_File_Separator1.Name = "TopMenu_File_Separator1";
            TopMenu_File_Separator1.Size = new Size(134, 6);
            // 
            // TopMenu_File_Exit
            // 
            TopMenu_File_Exit.Name = "TopMenu_File_Exit";
            TopMenu_File_Exit.Size = new Size(137, 22);
            TopMenu_File_Exit.Text = "Exit";
            TopMenu_File_Exit.Click += TopMenu_File_Exit_Click;
            // 
            // TopMenu_Window
            // 
            TopMenu_Window.Name = "TopMenu_Window";
            TopMenu_Window.Size = new Size(63, 20);
            TopMenu_Window.Text = "Window";
            // 
            // TopMenu_Simulation
            // 
            TopMenu_Simulation.DropDownItems.AddRange(new ToolStripItem[] { TopMenu_Simulation_NewSimulation, TopMenu_Simulation_AutoSaveSettings });
            TopMenu_Simulation.Name = "TopMenu_Simulation";
            TopMenu_Simulation.Size = new Size(46, 20);
            TopMenu_Simulation.Text = "Soup";
            // 
            // TopMenu_Simulation_NewSimulation
            // 
            TopMenu_Simulation_NewSimulation.Name = "TopMenu_Simulation_NewSimulation";
            TopMenu_Simulation_NewSimulation.Size = new Size(182, 22);
            TopMenu_Simulation_NewSimulation.Text = "Create a New Soup...";
            TopMenu_Simulation_NewSimulation.Click += TopMenu_Simulation_NewSimulation_Click;
            // 
            // SimulationView
            // 
            SimulationView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SimulationView.BackColor = Color.Black;
            SimulationView.Location = new Point(0, 24);
            SimulationView.Name = "SimulationView";
            SimulationView.Size = new Size(900, 600);
            SimulationView.TabIndex = 2;
            SimulationView.TabStop = false;
            SimulationView.MouseDown += SimulationView_MouseDown;
            SimulationView.MouseMove += SimulationView_MouseMove;
            SimulationView.MouseUp += SimulationView_MouseUp;
            // 
            // TopMenu_Simulation_AutoSaveSettings
            // 
            TopMenu_Simulation_AutoSaveSettings.Name = "TopMenu_Simulation_AutoSaveSettings";
            TopMenu_Simulation_AutoSaveSettings.Size = new Size(182, 22);
            TopMenu_Simulation_AutoSaveSettings.Text = "Auto Save Settings...";
            TopMenu_Simulation_AutoSaveSettings.Click += TopMenu_Simulation_AutoSaveSettings_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 646);
            Controls.Add(SimulationView);
            Controls.Add(BottomStat);
            Controls.Add(TopMenu);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = TopMenu;
            Name = "FormMain";
            Text = "Paramecium";
            Shown += FormMain_Shown;
            KeyDown += FormMain_KeyDown;
            BottomStat.ResumeLayout(false);
            BottomStat.PerformLayout();
            TopMenu.ResumeLayout(false);
            TopMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SimulationView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip BottomStat;
        private MenuStrip TopMenu;
        private ToolStripMenuItem TopMenu_Window;
        private PictureBox SimulationView;
        private ToolStripStatusLabel StatTps;
        private ToolStripStatusLabel StatFps;
        private ToolStripStatusLabel StatTimeStep;
        private ToolStripStatusLabel StatMemory;
        private ToolStripStatusLabel StatPopulation;
        private ToolStripMenuItem TopMenu_File;
        private ToolStripMenuItem TopMenu_File_Open;
        private ToolStripMenuItem TopMenu_File_Save;
        private ToolStripMenuItem TopMenu_File_SaveAs;
        private ToolStripSeparator TopMenu_File_Separator1;
        private ToolStripMenuItem TopMenu_File_Exit;
        private ToolStripStatusLabel StatSoupStatus;
        private ToolStripMenuItem TopMenu_Simulation;
        private ToolStripMenuItem TopMenu_Simulation_NewSimulation;
        private ToolStripStatusLabel StatTotalBornDie;
        private ToolStripStatusLabel StatLatestGeneration;
        private ToolStripMenuItem TopMenu_Simulation_AutoSaveSettings;
    }
}