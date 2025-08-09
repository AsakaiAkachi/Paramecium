using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paramecium.Forms.Controls
{
    public partial class SoupSettingsItemCheckbox : UserControl
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
        public bool DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                InputCheckBox.Checked = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Checked
        {
            get => InputCheckBox.Checked;
            set => InputCheckBox.Checked = value;
        }

        private bool _defaultValue = false;

        public SoupSettingsItemCheckbox()
        {
            InitializeComponent();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            InputCheckBox.Checked = _defaultValue;
        }
    }
}
