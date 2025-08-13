using Paramecium.Engine;

namespace Paramecium.Forms
{
    public partial class FormAutosaveSettings : Form
    {
        public FormAutosaveSettings()
        {
            InitializeComponent();

            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                CheckBoxAutosaveEnable.Checked = soup.AutosaveEnabled;
                NumUpDownAutosaveInterval.Value = soup.AutosaveInterval;
            }
        }

        private void ButtonApplySettings_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                soup.AutosaveEnabled = CheckBoxAutosaveEnable.Checked;
                soup.AutosaveInterval = (int)NumUpDownAutosaveInterval.Value;
                soup.Modified = true;
            }

            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
