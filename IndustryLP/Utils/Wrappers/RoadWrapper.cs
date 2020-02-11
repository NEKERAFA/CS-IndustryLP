using System.Collections.Generic;

namespace IndustryLP.Utils.Wrappers
{
    /// <summary>
    /// This class is a wrapper that gets the information about a road in Cities: Skylines
    /// </summary>
    internal class RoadWrapper
    {
        /// <summary>
        /// A list of segments that represents the road
        /// </summary>
        public List<SegmentWrapper> Segments { get; set; }

        /// <summary>
        /// The type of road
        /// </summary>
        public NetInfo Road { get; set; }
    }
}
