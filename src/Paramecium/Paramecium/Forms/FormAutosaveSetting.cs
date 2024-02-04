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
    public partial class FormAutosaveSetting : Form
    {
        public FormAutosaveSetting()
        {
            InitializeComponent();
        }

        private void FormAutosaveSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (g_Soup is not null)
            {
                g_Soup.AutoSave = checkboxEnableAutosave.Checked;
                g_Soup.AutoSaveInterval = (int)inputAutoSaveInterval.Value;
                UpdateCurrentAutoSaveIntervalText();
            }
            Hide();
        }

        public void UpdateCurrentAutoSaveIntervalText()
        {
            if (g_Soup is not null)
            {
                if (!g_Soup.AutoSave)
                {
                    labelCurrentAutoSaveInterval.Text = "Current Auto Save Interval : Auto Save Disabled";
                }
                else
                {
                    labelCurrentAutoSaveInterval.Text = $"Current Auto Save Interval : {g_Soup.AutoSaveInterval} steps";
                }
            }
        }
    }
}
