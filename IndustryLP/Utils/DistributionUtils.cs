using ColossalFramework.Math;
using System.Collections.Generic;
using UnityEngine;

namespace IndustryLP.Utils
{
    /// <summary>
    /// This class renderes a distribution info on game screen
    /// </summary>
    internal class DistributionUtils
    {
        /// <summary>
        /// Render a roads
        /// </summary>
        /// <param name="roads"></param>
        public static void RenderSegments(RenderManager.CameraInfo cameraInfo, Color segmentColor, List<Bezier3> roads)
        {
            var netPrefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road");

            foreach (var road in roads)
            {
                RenderManager.instance.OverlayEffect.DrawBezier(cameraInfo, segmentColor, road, netPrefab.m_halfWidth * 4f / 3f, 100000f, -100000f, -1f, 1280f, false, true);
            }
        }
    }
}
