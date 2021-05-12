using ColossalFramework.Math;
using IndustryLP.Utils.Enums;
using IndustryLP.Utils.Wrappers;
using System.Collections.Generic;

namespace IndustryLP.DistributionDefinition
{
    /// <summary>
    /// This class represents a distribution result
    /// </summary>
    internal abstract class DistributionInfo
    {
        /// <summary>
        /// The segments that made the road
        /// </summary>
        public List<Bezier3> Road { get; set; }

        /// <summary>
        /// The buildable cells in the distribution
        /// </summary>
        public List<ParcelWrapper> Cells { get; set; }

        public DistributionType Type { get; set; }

        /// <summary>
        /// This method finds the adjacent neighbour following the distribution
        /// </summary>
        /// <param name="direction">The direction to look up</param>
        /// <param name="gridId">The id of the cell</param>
        /// <returns></returns>
        public abstract ParcelWrapper GetNext(CellNeighbour direction, ushort gridId);
    }
}
