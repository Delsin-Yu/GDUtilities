using Godot;
using System;
using System.Collections.Generic;
using Bogus;
using GDUtilities;

namespace GDUtilitiesExamples;

public partial class ObjectSpawner_Main : Node
{
    [Export] private Button _button;
    [Export] private Button _trim;
    [Export] private PackedScene _prefab;
    [Export] private Control _container;
    [Export] private Label _label;

    private readonly Faker _faker = new();
    
    private Spawner _spawner;
    
    private class Spawner : ObjectSpawner<ObjectSpawner_Item, Model>
    {
        public Spawner(Control container, PackedScene prefab) : base(container, prefab) { }
        protected override void DrawElement(ObjectSpawner_Item instance, Model value, int index) => 
            instance.DrawElement(value.Text, value.Color);
    }

    private record struct Model(string Text, Color Color);

    public override void _Ready()
    {
        _spawner = new(_container, _prefab);
        _button.Pressed += PerformRedraw;
        _trim.Pressed += PerformTrim;
        PerformRedraw();
    }

    private void PerformRedraw()
    {
        var newDataSet = new List<Model>();
        var count = Random.Shared.NextInt64(5, 100);
        for (var i = 0; i < count; i++) 
            newDataSet.Add(new(_faker.Lorem.Word(), Color.FromHsv(_faker.Random.Float(), 1, 1)));
        
        _spawner.Draw(newDataSet);
        _label.Text = _spawner.ToString();
    }

    private void PerformTrim()
    {
        _spawner.Trim();
        _label.Text = _spawner.ToString();
    }
    
    public override void _Notification(int what)
    {
        if(what != NotificationPredelete) return;
        _spawner.Free();
    }
}
