using Paramecium.Engine;

namespace Paramecium.Forms
{
    public partial class FormCreateNewSoup : Form
    {
        public SoupSettings Settings;

        public FormCreateNewSoup()
        {
            InitializeComponent();

            Settings = new SoupSettings();
            UpdateCurrentSoupSettings(Settings);
        }

        private void ButtonEditSoupSettings_Click(object sender, EventArgs e)
        {
            new FormSoupSettings(Settings, true).ShowDialog(this);
            UpdateCurrentSoupSettings(Settings);
        }

        private void ButtonCreateSoup_Click(object sender, EventArgs e)
        {
            if (Owner is not null && Owner.GetType() == typeof(FormMain))
            {
                ((FormMain)Owner).CameraPosition = new Double2d(Settings.SizeX / 2d, Settings.SizeY / 2d);
                ((FormMain)Owner).ZoomLevel = 0;
            }

            if (g_Soup is not null && g_Soup.Initialized)
            {
                g_Soup.SetSoupState(SoupState.Stop);
            }

            g_Soup = new Soup(Settings);
            g_Soup.InitializeSoup();
            g_Soup.StartSoupThread();

            g_FilePath = @$"{Path.GetDirectoryName(g_FilePath)}\New Soup.soup";

            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateCurrentSoupSettings(SoupSettings settings)
        {
            LabelInitialSeed.Text = $": {settings.InitialSeed}";
            LabelTotalElementAmount.Text = $": {settings.TotalElementAmount.ToString("0")}";
            LabelSoupSize.Text = $": {settings.SizeX} x {settings.SizeY}";
            LabelInitialPlantPopulation.Text = $": {settings.InitialPlantPopulation}";
            LabelInitialAnimalPopulation.Text = $": {settings.InitialAnimalPopulation}";
        }
    }
}
