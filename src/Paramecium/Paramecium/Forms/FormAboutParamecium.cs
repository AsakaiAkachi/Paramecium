namespace Paramecium.Forms
{
    public partial class FormAboutParamecium : Form
    {
        public FormAboutParamecium()
        {
            InitializeComponent();
        }

        private void FormAboutParamecium_Shown(object sender, EventArgs e)
        {
            AppNameAndVersionLabel.Text = $"{Globals.AppName} {Globals.AppVersion}";
        }
    }
}
