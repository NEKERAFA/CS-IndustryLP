using UnityEngine;

namespace IndustryLP.Utils.Wrappers
{
    internal class NodeWrapper
    {
        public ushort Id { get; set; }
        public Vector3 Position { get; set; }
        public NetInfo Road { get; set; }
    }
}
