using System.ComponentModel;

namespace Paramecium.Forms.Controls
{
    public partial class SoupSettingsItemNumUpDown : UserControl
    {
        [Browsable(true)]
        [Category("Soup Settings Item Num Up Down")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ItemName
        {
            get => ItemNameLabel.Text;
            set
            {
                ItemNameLabel.Text = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Soup Settings Item Num Up Down")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal DefaultValue
        {
            get => InputNumUpDown.Value;
            set
            {
                InputNumUpDown.Value = value;
                _defaultValue = InputNumUpDown.Value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Soup Settings Item Num Up Down")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int DecimalPlaces
        {
            get => InputNumUpDown.DecimalPlaces;
            set
            {
                InputNumUpDown.DecimalPlaces = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Soup Settings Item Num Up Down")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Increment
        {
            get => InputNumUpDown.Increment;
            set
            {
                InputNumUpDown.Increment = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Soup Settings Item Num Up Down")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Maximum
        {
            get => InputNumUpDown.Maximum;
            set
            {
                InputNumUpDown.Maximum = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Soup Settings Item Num Up Down")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Minimum
        {
            get => InputNumUpDown.Minimum;
            set
            {
                InputNumUpDown.Minimum = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Soup Settings Item Num Up Down")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Editable
        {
            get => _editable;
            set
            {
                _editable = value;

                ItemNameLabel.Enabled = value;
                InputNumUpDown.Enabled = value;
                ResetButton.Enabled = value;

                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int InputValueInt
        {
            get => (int)InputNumUpDown.Value;
            set
            {
                InputNumUpDown.Value = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double InputValueDouble
        {
            get => (double)InputNumUpDown.Value;
            set
            {
                InputNumUpDown.Value = (decimal)value;
                Invalidate();
            }
        }

        private decimal _defaultValue = 0;
        private bool _editable = true;

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
