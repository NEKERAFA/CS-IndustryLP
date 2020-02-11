namespace IndustryLP.Utils.Wrappers
{
    /// <summary>
    /// This class is a wrapper that gets the information about a net segment in Cities: Skylines
    /// </summary>
    internal class SegmentWrapper
    {
        /// <summary>
        /// The id of the segment in Cities: Skylines
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// The start node
        /// </summary>
        public NodeWrapper StartPosition { get; set; }

        /// <summary>
        /// The end node
        /// </summary>
        public NodeWrapper EndPosition { get; set; }

        /// <summary>
        /// The type of the road
        /// </summary>
        public NetInfo Road { get; set; }
    }
}
