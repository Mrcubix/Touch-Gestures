using System;
using Avalonia.Media;
using TouchGestures.UX.ViewModels.Controls.Nodes;

namespace TouchGestures.UX.Interfaces.Nodes;

#nullable enable

public interface INodeBuilder : IDisposable
{
    /// <summary>
    ///   Raised when the node builder is requesting a build from the owning view model.
    /// </summary>
    public event EventHandler? BuildRequested;

    /// <summary>
    ///   The name of the shape built by this builder.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    ///   The icon of the shape built by this builder.
    /// </summary>
    public IImage Icon { get; init; }

    /// <summary>
    ///   Builds the node view model.
    /// </summary>
    /// <returns><see cref="NodeViewModel"/></returns>
    public NodeViewModel Build();

    /// <summary>
    ///   Requests a build from the owning view model.
    /// </summary>
    public void RequestBuild();
}