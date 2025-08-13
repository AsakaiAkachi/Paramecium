using System.ComponentModel;

namespace Paramecium.Forms.Controls
{
    public partial class SoupSettingsLabel : UserControl
    {
        [Browsable(true)]
        [Category("Soup Settings Label")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Label
        {
            get => LabelText.Text;
            set => LabelText.Text = value;
        }

        [Browsable(true)]
        [Category("Soup Settings Label")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackgroundColor
        {
            get => BackColor;
            set => BackColor = value;
        }

        public SoupSettingsLabel()
        {
            InitializeComponent();
        }
    }
}
