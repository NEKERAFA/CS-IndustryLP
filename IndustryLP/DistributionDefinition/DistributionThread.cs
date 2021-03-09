using ColossalFramework.Math;

namespace IndustryLP.DistributionDefinition
{
    /// <summary>
    /// This class represents the interface to the distribution generation functions 
    /// </summary>
    internal abstract class DistributionThread
    {
        /// <summary>
        /// Generates a building distribution inside a selection
        /// </summary>
        /// <param name="selection">A <see cref="Quad3"/> object</param>
        /// <param name="angle">The object</param>
        /// <returns>A <see cref="DistributionInfo"/> object</returns>
        public abstract DistributionInfo Generate(Quad3 selection, float angle);
    }
}
