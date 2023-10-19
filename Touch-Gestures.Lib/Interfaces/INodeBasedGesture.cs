using System.Collections.Generic;

namespace TouchGestures.Lib.Interfaces
{
    public interface INodeBasedGesture : IMixedBasedGesture
    {
        public List<IGestureNode> Nodes { get; set; }
        public int CurrentNodeIndex { get; set; }

        protected void OnGestureProgress(GestureEventArgs args);
    }
}