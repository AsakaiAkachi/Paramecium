using System.ComponentModel;

namespace Paramecium.Forms.Controls
{
    public partial class SoupSettingsItemNumUpDown : UserControl
    {
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ItemName
        {
            get => ItemNameLabel.Text;
            set => ItemNameLabel.Text = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal DefaultValue
        {
            get => InputNumUpDown.Value;
            set
            {
                InputNumUpDown.Value = value;
                _defaultValue = InputNumUpDown.Value;
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int DecimalPlaces
        {
            get => InputNumUpDown.DecimalPlaces;
            set => InputNumUpDown.DecimalPlaces = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Increment
        {
            get => InputNumUpDown.Increment;
            set => InputNumUpDown.Increment = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Maximum
        {
            get => InputNumUpDown.Maximum;
            set => InputNumUpDown.Maximum = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Minimum
        {
            get => InputNumUpDown.Minimum;
            set => InputNumUpDown.Minimum = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int InputValueInt
        {
            get => (int)InputNumUpDown.Value;
            set => InputNumUpDown.Value = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double InputValueDouble
        {
            get => (double)InputNumUpDown.Value;
            set => InputNumUpDown.Value = (decimal)value;
        }

        private decimal _defaultValue = 0;

        public SoupSettingsItemNumUpDown()
        {
            InitializeComponent();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            InputNumUpDown.Value = _defaultValue;
        }
    }
}
