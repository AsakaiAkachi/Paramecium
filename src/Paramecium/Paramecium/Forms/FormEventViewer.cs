using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paramecium.Forms
{
    public partial class FormEventViewer : Form
    {
        FormMain RootForm;

        public FormEventViewer(FormMain parentForm)
        {
            RootForm = parentForm;

            InitializeComponent();
        }

        private void FormEventViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainLoopBreak = true;
            RootForm.FormEventViewer_Closed();
        }

        bool MainLoopBreak = false;
        private async void FormEventViewer_Shown(object sender, EventArgs e)
        {
            /**
            while (true)
            {
                if (MainLoopBreak) break;

                richTextBox1.AppendText(System.Environment.NewLine + System.DateTime.Now.ToString());

                await Task.Delay(1);
            }
            **/
        }
    }
}
