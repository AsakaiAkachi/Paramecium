using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paramecium.Forms
{
    public partial class FormAutosaveSettings : Form
    {
        public FormAutosaveSettings()
        {
            InitializeComponent();
        }

        private void FormAutosaveSettings_Shown(object sender, EventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            CheckBoxAutosaveEnable.Checked = g_Soup.AutosaveEnabled;
            NumericUpDownAutosaveInterval.Value = g_Soup.AutosaveInterval;
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            if (g_Soup is null || !g_Soup.Initialized) return;

            g_Soup.AutosaveEnabled = CheckBoxAutosaveEnable.Checked;
            g_Soup.AutosaveInterval = (int)NumericUpDownAutosaveInterval.Value;
            g_Soup.PrevSaveTime = g_Soup.ElapsedTimeSteps;

            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
