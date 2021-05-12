using ColossalFramework.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace IndustryLP.Utils
{
    /// <summary>
    /// This class has methods that abstracts some operations about terrain
    /// </summary>
    internal static class TerrainUtils
    {
        /// <summary>
        /// Gets the proyection of a quad with the terrain
        /// </summary>
        /// <param name="quad"></param>
        /// <returns></returns>
        public static Quad3 Quad3ToTerrain(Quad3 quad)
        {
            var raycast = new Ray(quad.a, Vector3.up);
            if (!IndustryTool.instance.GetColisingWithTerrain(raycast, out Vector3 a1))
            {
                raycast = new Ray(quad.a, Vector3.down);
                IndustryTool.instance.GetColisingWithTerrain(raycast, out a1);
            }

            raycast = new Ray(quad.b, Vector3.up);
            if (!IndustryTool.instance.GetColisingWithTerrain(raycast, out Vector3 b1))
            {
                raycast = new Ray(quad.b, Vector3.down);
                IndustryTool.instance.GetColisingWithTerrain(raycast, out b1);
            }

            raycast = new Ray(quad.c, Vector3.up);
            if (!IndustryTool.instance.GetColisingWithTerrain(raycast, out Vector3 c1))
            {
                raycast = new Ray(quad.c, Vector3.down);
                IndustryTool.instance.GetColisingWithTerrain(raycast, out c1);
            }

            raycast = new Ray(quad.d, Vector3.up);
            if (!IndustryTool.instance.GetColisingWithTerrain(raycast, out Vector3 d1))
            {
                raycast = new Ray(quad.d, Vector3.down);
                IndustryTool.instance.GetColisingWithTerrain(raycast, out d1);
            }

            return new Quad3(a1, b1, c1, d1);
        }

        /// <summary>
        /// Translate the current position of mouse in terrain position
        /// </summary>
        public static Vector3 GetTerrainMousePosition()
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            IndustryTool.instance.GetColisingWithTerrain(mouseRay, out Vector3 output);
            return output;
        }
    }
}
