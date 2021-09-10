using UnityEngine;
using static IndustryLP.Utils.MathUtils;

namespace IndustryLP.Utils.Wrappers
{
    /// <summary>
    /// This class is a wrapper that gets the information about a net node in Cities: Skylines
    /// </summary>
    internal class NodeWrapper : EntityPosition
    {
        /// <summary>
        /// The id of the node in Cities: Skylines
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// The type of the road
        /// </summary>
        public NetInfo Road { get; set; }
    }
}
