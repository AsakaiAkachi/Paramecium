using Paramecium.Engine;

namespace Paramecium.Forms
{
    // オートセーブの設定をするためのForm
    public partial class FormAutosaveSettings : Form
    {
        public FormAutosaveSettings()
        {
            InitializeComponent();
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

        private void FormAutosaveSettings_Shown(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                CheckBoxAutosaveEnable.Checked = soup.AutosaveEnabled;
                NumUpDownAutosaveInterval.Value = soup.AutosaveInterval;
            }
        }
    }
}
