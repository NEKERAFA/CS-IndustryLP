using ColossalFramework;
using ColossalFramework.Math;

namespace IndustryLP.Utils
{
    /// <summary>
    /// This class has methods that abstracts some operations about instance creation 
    /// </summary>
    internal static class SimulationUtils
    {
        /// <summary>
        /// Gets the next build index and update the simulation manager build index
        /// </summary>
        public static uint GetNewBuildIndex()
        {
            // Gets managers
            var simulationManager = Singleton<SimulationManager>.instance;
            // Get fresh build index
            uint newBuildIndex = simulationManager.m_currentBuildIndex++;
            // Set new build index
            simulationManager.m_currentBuildIndex = newBuildIndex;

            return newBuildIndex;
        }

        public static Randomizer GetRandomizer()
        {
            // Gets managers
            var simulationManager = Singleton<SimulationManager>.instance;

            return simulationManager.m_randomizer;
        }
    }
}
