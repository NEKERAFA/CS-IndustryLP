using ColossalFramework.Math;
using UnityEngine;

namespace IndustryLP.Utils
{
    /// <summary>
    /// This class has methods that abstracts some operations about building creation
    /// </summary>
    internal class BuildingUtils
    {
        /// <summary>
        /// Renders the overlay building in the terrain.
        /// </summary>
        /// <param name="cameraInfo">A <see cref="RenderManager.CameraInfo"/> object</param>
        /// <param name="position">The position of the building</param>
        /// <param name="angle">The angle of the building</param>
        /// <param name="buildingPrefab">A <see cref="BuildingInfo"/> object</param>
        /// <param name="buildingColor">The color of the building</param>
        public static void RenderBuildingOverlay(RenderManager.CameraInfo cameraInfo, ref Matrix4x4 matrixTRS, Vector3 center, Vector3 position, float angle, BuildingInfo buildingPrefab, Color buildingColor)
        {
            Vector3 buildingPosition = matrixTRS.MultiplyPoint(position - center);

            buildingPrefab.m_buildingAI.RenderBuildOverlay(cameraInfo, buildingColor, position, angle, default);
            BuildingTool.RenderOverlay(cameraInfo, buildingPrefab, 0, buildingPosition, angle, buildingColor, true);
            
            if (buildingPrefab.m_subBuildings != null && buildingPrefab.m_subBuildings.Length != 0)
            {
                Matrix4x4 subMatrix4x = default;
                subMatrix4x.SetTRS(buildingPosition, Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.down), Vector3.one);
                for (int i = 0; i < buildingPrefab.m_subBuildings.Length; i++)
                {
                    BuildingInfo buildingInfo2 = buildingPrefab.m_subBuildings[i].m_buildingInfo;
                    Vector3 position2 = subMatrix4x.MultiplyPoint(buildingPrefab.m_subBuildings[i].m_position);
                    float angle2 = buildingPrefab.m_subBuildings[i].m_angle * Mathf.Deg2Rad + angle;
                    buildingInfo2.m_buildingAI.RenderBuildOverlay(cameraInfo, buildingColor, position2, angle2, default);
                    BuildingTool.RenderOverlay(cameraInfo, buildingInfo2, 0, position2, angle2, buildingColor, true);
                }
            }
        }

        /// <summary>
        /// Renders a building.
        /// </summary>
        /// <param name="cameraInfo">A <see cref="RenderManager.CameraInfo"/> object</param>
        /// <param name="position">The position of the building</param>
        /// <param name="angle">The angle of the building</param>
        /// <param name="buildingPrefab">A <see cref="BuildingInfo"/> object</param>
        /// <param name="selectionColor">The color to aply to the overlay</param>
        public static void RenderBuildingGeometry(RenderManager.CameraInfo cameraInfo, ref Matrix4x4 matrixTRS, Vector3 center, Vector3 position, float angle, BuildingInfo buildingPrefab, Color buildingColor)
        {
            Vector3 buildingPosition = matrixTRS.MultiplyPoint(position - center);

            buildingPrefab.m_buildingAI.RenderBuildGeometry(cameraInfo, buildingPosition, angle, 0);
            BuildingTool.RenderGeometry(cameraInfo, buildingPrefab, 0, buildingPosition, angle, false, buildingColor);

            if (buildingPrefab.m_subBuildings != null && buildingPrefab.m_subBuildings.Length != 0)
            {
                Matrix4x4 subMatrix4x = Matrix4x4.identity;
                subMatrix4x.SetTRS(buildingPosition, Quaternion.identity, Vector3.one);
                for (int n = 0; n < buildingPrefab.m_subBuildings.Length; n++)
                {
                    BuildingInfo buildingInfo2 = buildingPrefab.m_subBuildings[n].m_buildingInfo;
                    Vector3 subPosition = subMatrix4x.MultiplyPoint(buildingPrefab.m_subBuildings[n].m_position);
                    var subAngle = buildingPrefab.m_subBuildings[n].m_angle * Mathf.Deg2Rad + angle;
                    buildingInfo2.m_buildingAI.RenderBuildGeometry(cameraInfo, subPosition, subAngle, 0);
                    BuildingTool.RenderGeometry(cameraInfo, buildingInfo2, 0, subPosition, subAngle, true, buildingColor);
                }
            }
        }

        /// <summary>
        /// Gets the color of the building
        /// </summary>
        /// <param name="buildingID"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Color GetColor(ushort seed, BuildingInfo info)
        {
            if (!info.m_useColorVariations)
            {
                return info.m_color0;
            }
            
            Randomizer randomizer = new Randomizer(seed);
            
            switch (randomizer.Int32(4u))
            {
                case 0:
                    return info.m_color0;
                case 1:
                    return info.m_color1;
                case 2:
                    return info.m_color2;
                case 3:
                    return info.m_color3;
                default:
                    return info.m_color0;
            }
        }
    }
}
