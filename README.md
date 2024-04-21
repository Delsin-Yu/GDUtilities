# GDUtilities

A collection of util scripts that fits my development needs.

## Auto Scroller

The `auto scroller` watches the attached `scroll container`, and makes its content do ping-pong action automatically when exceeds the size of the viewport, this is an easy and design-wise cheap way for addressing UI overflow issues when dealing with the situation when the text becomes larger than it was when designing the UI. This script also disables user interactions for the controlled `scroll container`.

## Object Spawner

The `object spawner` is a C# abstract class that performs automatically `object pooling` when performing batch drawing on elements, the developer may inherit this class to implement their draw logic in `DrawElement`, and clean up logic in `CleanupElement`. To use this type, the developer needs to first instantiate their inherited type by supplying the `container` for the instances and `prefab` to instantiate from and call `Draw` with a collection of values to perform the batch drawing. The `Trim` method is available when the `PooledCount` has exceeded the desired amount of the developer.