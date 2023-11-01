using System.Collections.Generic;

namespace TouchGestures.Lib.Interfaces
{
    public interface INodeBasedGesture : IMixedBasedGesture
    {
        List<IGestureNode> Nodes { get; }
        int CurrentNodeIndex { get; }
    }
}