using Paramecium.Engine;
using Paramecium.Variables;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Paramecium.Forms
{
    // オブジェクトエディターのForm
    public partial class FormObjectEditor : Form
    {
        private static JsonSerializerOptions _jsonSerializerOptions;

        static FormObjectEditor()
        {
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                Converters = { new JsonStringEnumConverter() },
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                WriteIndented = true,
                IndentSize = 4
            };
        }

        public SoupObjectPointer SelectedObjectPointer = new SoupObjectPointer();

        public FormObjectEditor()
        {
            InitializeComponent();

            RichTextBoxEditingObjectRawJson.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
        }

        public void LoadSoupObject()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (SelectedObjectPointer.ObjectType != SoupObjectType.None)
                {
                    SoupState soupState = soup.SoupState;

                    soup.SetSoupState(SoupState.Pause);

                    object? loadedObject = SelectedObjectPointer.GetSoupObject();

                    if (loadedObject is not null)
                    {
                        RichTextBoxEditingObjectRawJson.Text = JsonSerializer.Serialize(loadedObject, _jsonSerializerOptions);
                        if (SelectedObjectPointer.ObjectType == SoupObjectType.Tile) LabelTargetObject.Text = $"Currently Edited Object : {SelectedObjectPointer.ObjectType.ToString()} #{SelectedObjectPointer.ObjectIndex}";
                        else LabelTargetObject.Text = $"Currently Edited Object : {SelectedObjectPointer.ObjectType.ToString()} #{Soup.StringFromCellId(SelectedObjectPointer.ObjectId)}";
                    }

                    soup.SetSoupState(soupState);
                }
            }
        }
        public void LoadSoupObject(SoupObjectPointer soupObject)
        {
            SelectedObjectPointer = soupObject;
            LoadSoupObject();
        }

        private void ButtonReload_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                LoadSoupObject();
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (SelectedObjectPointer.ObjectType != SoupObjectType.None)
                {
                    SoupState soupState = soup.SoupState;

                    soup.SetSoupState(SoupState.Pause);

                    if (SelectedObjectPointer.GetSoupObject() is not null)
                    {
                        if (SelectedObjectPointer.ObjectType == SoupObjectType.Tile)
                        {
                            Tile? deserializedObject = JsonSerializer.Deserialize<Tile>(RichTextBoxEditingObjectRawJson.Text, _jsonSerializerOptions);
                            if (deserializedObject is not null)
                            {
                                soup.Tiles[SelectedObjectPointer.ObjectIndex] = deserializedObject;
                                soup.Modified = true;
                            }
                        }
                        else if (SelectedObjectPointer.ObjectType == SoupObjectType.Plant)
                        {
                            Plant? deserializedObject = JsonSerializer.Deserialize<Plant>(RichTextBoxEditingObjectRawJson.Text, _jsonSerializerOptions);
                            if (deserializedObject is not null)
                            {
                                soup.Plants[SelectedObjectPointer.ObjectIndex] = deserializedObject;
                                soup.Modified = true;
                            }
                        }
                        else if (SelectedObjectPointer.ObjectType == SoupObjectType.Animal)
                        {
                            Animal? deserializedObject = JsonSerializer.Deserialize<Animal>(RichTextBoxEditingObjectRawJson.Text, _jsonSerializerOptions);
                            if (deserializedObject is not null)
                            {
                                soup.Animals[SelectedObjectPointer.ObjectIndex] = deserializedObject;
                                soup.Modified = true;
                            }
                        }
                    }

                    soup.SetSoupState(soupState);
                }
            }
        }
    }
}
