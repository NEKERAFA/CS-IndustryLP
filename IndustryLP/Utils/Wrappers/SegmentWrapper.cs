using UnityEngine;

namespace IndustryLP.Utils.Wrappers
{
    internal class SegmentWrapper
    {
        public ushort Id { get; set; }
        public NodeWrapper StartPosition { get; set; }
        public NodeWrapper EndPosition { get; set; }
        public NetInfo Road { get; set; }
    }
}
