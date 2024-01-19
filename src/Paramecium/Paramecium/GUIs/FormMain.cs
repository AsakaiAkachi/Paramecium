namespace Paramecium.GUIs
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private async void FormMain_Shown(object sender, EventArgs e)
        {
            while (true)
            {
                await Task.Delay(1);
            }
        }
    }
}
