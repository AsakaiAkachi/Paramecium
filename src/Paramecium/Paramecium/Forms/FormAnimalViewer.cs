using Paramecium.Simulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Paramecium.Forms
{
    public partial class FormAnimalViewer : Form
    {
        public int TargetIndex;
        public int TargetId;

        public FormAnimalViewer(int targetIndex, int targetId)
        {
            InitializeComponent();

            TargetIndex = targetIndex;
            TargetId = targetId;
        }

        public void Update()
        {
            if (Global.SoupInstance.Particles[TargetIndex] is not null)
            {
                if (Global.SoupInstance.Particles[TargetIndex].Id == TargetId && Global.SoupInstance.Particles[TargetIndex].Type == ParticleType.Animal)
                {
                    Particle Target = Global.SoupInstance.Particles[TargetIndex];

                    LabelItemPosition.Visible = true;
                    LabelItemVelocity.Visible = true;
                    LabelItemAngle.Visible = true;
                    LabelItemColor.Visible = true;
                    LabelItemSatiety.Visible = true;
                    LabelItemGeneration.Visible = true;
                    LabelItemAge.Visible = true;
                    LabelPosition.Visible = true;
                    LabelVelocity.Visible = true;
                    LabelAngle.Visible = true;
                    LabelColor.Visible = true;
                    LabelSatiety.Visible = true;
                    LabelGeneration.Visible = true;
                    LabelAge.Visible = true;

                    LabelSelectedParticleId.Text = $"#{TargetId}";
                    LabelPosition.Text = $": ({Target.Position.X}, {Target.Position.Y})";
                    LabelVelocity.Text = $": ({Target.Velocity.X}, {Target.Velocity.Y})";
                    LabelAngle.Text = $": {Target.Angle}°";
                    LabelColor.Text = $": {Target.Color.Red}, {Target.Color.Green}, {Target.Color.Blue}";
                    LabelSatiety.Text = $": {Target.Satiety}";
                    LabelGeneration.Text = $": {Target.Generation}";
                    LabelAge.Text = $": {Target.Age}";
                }
            }
            else
            {
                LabelSelectedParticleId.Text = "<Unselected>";

                LabelItemPosition.Visible = false;
                LabelItemVelocity.Visible = false;
                LabelItemAngle.Visible = false;
                LabelItemColor.Visible = false;
                LabelItemSatiety.Visible = false;
                LabelItemGeneration.Visible = false;
                LabelItemAge.Visible = false;
                LabelPosition.Visible = false;
                LabelVelocity.Visible = false;
                LabelAngle.Visible = false;
                LabelColor.Visible = false;
                LabelSatiety.Visible = false;
                LabelGeneration.Visible = false;
                LabelAge.Visible = false;
            }
        }
    }
}
