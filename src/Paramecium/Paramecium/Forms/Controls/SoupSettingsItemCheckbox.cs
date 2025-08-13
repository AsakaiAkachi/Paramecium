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
        [Category("Soup Settings Item Checkbox")]
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
        [Category("Soup Settings Item Checkbox")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                InputCheckBox.Checked = value;
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
                InputCheckBox.Enabled = value;
                ResetButton.Enabled = value;

                Invalidate();
            }
        }

        [Browsable(false)]
        [Category("Soup Settings Item Num Up Down")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Checked
        {
            get => InputCheckBox.Checked;
            set
            {
                InputCheckBox.Checked = value;
                Invalidate();
            }
        }

        private bool _defaultValue = false;
        private bool _editable = true;

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
