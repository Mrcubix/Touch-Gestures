using System.Collections.Generic;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures.Bases
{
    public abstract class NodeBasedGesture : MixedBasedGesture, INodeBasedGesture
    {
        public abstract List<IGestureNode> Nodes { get; set; }
        public int CurrentNodeIndex { get; protected set; }

        protected abstract void OnGestureProgress(GestureEventArgs args);
    }
}