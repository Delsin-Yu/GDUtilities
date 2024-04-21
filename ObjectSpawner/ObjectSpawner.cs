using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GDUtilities;

public abstract class ObjectSpawner<TComponent, TValue> where TComponent : Node
{
    private readonly Stack<TComponent> _activeInstance = [];
    private readonly Stack<TComponent> _pooledInstance = [];
    private readonly Control _container;
    private readonly PackedScene _prefab;

    private bool _disposed;

    protected ObjectSpawner(Control container, PackedScene prefab)
    {
        _container = container;
        _prefab = prefab;
    }

    public int ActiveCount => _activeInstance.Count;
    public int PooledCount => _pooledInstance.Count;

    public void Draw(TValue[] values)
    {
        ThrowIfDisposed();
        CollectedUsedInstance();
        var count = 0;
        foreach (var value in values.AsSpan())
        {
            DrawItem(value, count);
            count++;
        }
    }

    public void Draw(List<TValue> values)
    {
        ThrowIfDisposed();
        CollectedUsedInstance();
        var count = 0;
        foreach (var value in CollectionsMarshal.AsSpan(values))
        {
            DrawItem(value, count);
            count++;
        }
    }

    public void Draw(IReadOnlyList<TValue> values)
    {
        ThrowIfDisposed();
        CollectedUsedInstance();
        for (var index = 0; index < values.Count; index++)
        {
            var value = values[index];
            DrawItem(value, index);
        }
    }

    public void Draw(IEnumerable<TValue> values)
    {
        ThrowIfDisposed();
        CollectedUsedInstance();
        var count = 0;
        foreach (var value in values)
        {
            DrawItem(value, count);
            count++;
        }
    }

    public void Trim()
    {
        while (_pooledInstance.TryPop(out var instance)) 
            instance.Free();
    }

    protected abstract void DrawElement(TComponent instance, TValue value, int index);
    protected virtual void CleanupElement(TComponent instance) { }

    public override string ToString()
    {
        ThrowIfDisposed();

        return
            $"""
             Container: {_container.Name}
             Prefab: {_prefab}
             Active Instance: {ActiveCount}
             Pooled Instance: {PooledCount}
             Total Instance: {ActiveCount + PooledCount}
             """;
    }

    public void Free()
    {
        ThrowIfDisposed();

        while (_activeInstance.TryPop(out var instance))
        {
            ResetElement(instance);
            instance.Free();
        }

        while (_pooledInstance.TryPop(out var instance))
        {
            instance.Free();
        }

        _container?.Dispose();
        _prefab?.Dispose(); 
        
        _disposed = true;
    }

    ~ObjectSpawner()
    {
        if(_disposed) return;
        _container?.Dispose();
        _prefab?.Dispose(); 
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException("ObjectSpawner");
    }

    private void CollectedUsedInstance()
    {
        while (_activeInstance.TryPop(out var nodeInstance))
        {
            ResetElement(nodeInstance);
            _container.RemoveChild(nodeInstance);
            _pooledInstance.Push(nodeInstance);
        }
    }

    private void ResetElement(TComponent nodeInstance)
    {
        try
        {
            CleanupElement(nodeInstance);
        }
        catch (Exception e)
        {
            GD.Print(e.ToString());
        }
    }

    private void DrawItem(TValue value, int count)
    {
        if (!_pooledInstance.TryPop(out var nodeInstance))
            nodeInstance = _prefab.Instantiate<TComponent>();

        _container.AddChild(nodeInstance);
        _activeInstance.Push(nodeInstance);

        try
        {
            DrawElement(nodeInstance, value, count);
        }
        catch (Exception e)
        {
            GD.Print(e.ToString());
        }
    }
}