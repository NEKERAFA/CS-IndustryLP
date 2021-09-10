using ColossalFramework.Math;
using IndustryLP.DistributionDefinition;
using IndustryLP.DomainDefinition;
using IndustryLP.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace IndustryLP
{
    /// <summary>
    /// Represents a interface to comunicate actions with main tool
    /// </summary>
    internal interface IMainTool
    {
        Quad3? Selection { get; }
        float? SelectionAngle { get; }
        DistributionInfo Distribution { get; }
        List<Parcel> Preferences { get; }
        List<Parcel> Restrictions { get; }

        /// <summary>
        /// Intersects a ray with the terrain
        /// </summary>
        /// <param name="ray">The ray to intersect with the terrain</param>
        /// <param name="output">The intersection point</param>
        /// <returns><c>true</c> if the ray intersects with terrain, <c>false</c> otherwise</returns>
        bool GetColisingWithTerrain(Ray ray, out Vector3 output);

        /// <summary>
        /// Cancels the current selection
        /// </summary>
        void CancelZoning();

        /// <summary>
        /// Updates the current selection
        /// </summary>
        /// <param name="selection">The new zone</param>
        /// <param name="angle">The angle with the main camera</param>
        void DoZoning(Quad3 selection, float angle);

        /// <summary>
        /// Sets build distribution as grid distribution
        /// </summary>
        void SetGridDistribution();

        /// <summary>
        /// Sets build distribution as line distribution
        /// </summary>
        void SetLineDistribution();

        /// <summary>
        /// Sets build distribution as mine distribution
        /// </summary>
        void SetForestalDistribution();

        /// <summary>
        /// Ads a new preference to the grid
        /// </summary>
        /// <param name="gridId">The id in the grid</param>
        /// <param name="building">The building that defines the preference</param>
        void AddPreference(ushort gridId, BuildingInfo building);

        /// <summary>
        /// Ads a new restriction to the grid
        /// </summary>
        /// <param name="gridId"></param>
        /// <param name="building"></param>
        void AddRestriction(ushort gridId, BuildingInfo building);

        /// <summary>
        /// Build a solution and closes the tool
        /// </summary>
        void BuildGeneration(Region solution);

        /// <summary>
        /// Cancel the current solution
        /// </summary>
        void CancelGeneration();
    }
}
