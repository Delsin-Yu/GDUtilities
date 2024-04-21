using Godot;

namespace GDUtilitiesExamples;

public partial class ObjectSpawner_Item : Control
{
    [Export] private Label _text;

    public void DrawElement(string text, Color color)
    {
        _text.Text = text;
        Modulate = color;
    }
}