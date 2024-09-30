﻿using Paramecium.Engine;
using System.Text.Json;

namespace Paramecium.Forms
{
    public partial class FormInspector : Form
    {
        public FormInspector()
        {
            InitializeComponent();

            ComboBoxObjectType.SelectedIndex = 0;
            RichTextBoxInspectResult.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
        }

        public void Inspect(int objectType, int index)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            try
            {
                if (objectType == 0)
                {
                    RichTextBoxInspectResult.Text = JsonSerializer.Serialize(g_Soup.Tiles[index], options);
                    ComboBoxObjectType.SelectedIndex = 0;
                }
                else if (objectType == 1)
                {
                    RichTextBoxInspectResult.Text = JsonSerializer.Serialize(g_Soup.Plants[index], options);
                    ComboBoxObjectType.SelectedIndex = 1;
                }
                else if (objectType == 2)
                {
                    RichTextBoxInspectResult.Text = JsonSerializer.Serialize(g_Soup.Animals[index], options);
                    ComboBoxObjectType.SelectedIndex = 2;
                }
                NumericUpDownObjectIndex.Value = index;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                MessageBox.Show(
                    $"Could not load object.\r\nWrong index or invalid object.",
                    $"{g_AppName}",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1
                );
            }
        }

        private void FormInspector_Shown(object sender, EventArgs e)
        {

        }

        private void ButtonInspect_Click(object sender, EventArgs e)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            try
            {
                if (ComboBoxObjectType.SelectedIndex == 0)
                {
                    RichTextBoxInspectResult.Text = JsonSerializer.Serialize(g_Soup.Tiles[(int)NumericUpDownObjectIndex.Value], options);
                }
                else if (ComboBoxObjectType.SelectedIndex == 1)
                {
                    RichTextBoxInspectResult.Text = JsonSerializer.Serialize(g_Soup.Plants[(int)NumericUpDownObjectIndex.Value], options);
                }
                else if (ComboBoxObjectType.SelectedIndex == 2)
                {
                    RichTextBoxInspectResult.Text = JsonSerializer.Serialize(g_Soup.Animals[(int)NumericUpDownObjectIndex.Value], options);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                MessageBox.Show(
                    $"Could not load object.\r\nWrong index or invalid object.",
                    $"{g_AppName}",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1
                );
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComboBoxObjectType.SelectedIndex == 0)
                {
                    Tile? deserializedObject = JsonSerializer.Deserialize<Tile>(RichTextBoxInspectResult.Text);
                    if (deserializedObject is not null) g_Soup.Tiles[(int)NumericUpDownObjectIndex.Value] = deserializedObject;
                }
                else if (ComboBoxObjectType.SelectedIndex == 1)
                {
                    Plant? deserializedObject = JsonSerializer.Deserialize<Plant>(RichTextBoxInspectResult.Text);
                    if (deserializedObject is not null) g_Soup.Plants[(int)NumericUpDownObjectIndex.Value] = deserializedObject;
                }
                else if (ComboBoxObjectType.SelectedIndex == 2)
                {
                    Animal? deserializedObject = JsonSerializer.Deserialize<Animal>(RichTextBoxInspectResult.Text);
                    if (deserializedObject is not null) g_Soup.Animals[(int)NumericUpDownObjectIndex.Value] = deserializedObject;
                }

                MessageBox.Show(
                    $"Object data has been saved.",
                    $"{g_AppName}",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                MessageBox.Show(
                    $"Could not save object.\r\nThe object data has incorrect or invalid syntax.",
                    $"{g_AppName}",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1
                );
            }
        }

        private void ComboBoxObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxObjectType.SelectedIndex == 0)
            {
                NumericUpDownObjectIndex.Maximum = g_Soup.Tiles.Length - 1;
            }
            else if (ComboBoxObjectType.SelectedIndex == 1)
            {
                NumericUpDownObjectIndex.Maximum = g_Soup.Plants.Count - 1;
            }
            else if (ComboBoxObjectType.SelectedIndex == 2)
            {
                NumericUpDownObjectIndex.Maximum = g_Soup.Animals.Count - 1;
            }
            NumericUpDownObjectIndex.Value = 0;
        }
    }
}