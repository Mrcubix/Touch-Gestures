using System.Collections.Generic;
using TouchGestures.Lib.Entities.Gestures.Bases;

namespace TouchGestures.Lib.Interfaces
{
    public abstract class NodeBasedGesture : MixedBasedGesture, INodeBasedGesture
    {
        public abstract List<IGestureNode> Nodes { get; set; }
        public int CurrentNodeIndex { get; protected set; }

        protected abstract void OnGestureProgress(GestureEventArgs args);
    }
}