using IndustryLP.Tools;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using UnityEngine;

namespace IndustryLP.Actions
{
    internal class AddBuildingAction : ToolAction
    {
        #region Attribute

        private readonly BuildingInfo m_building;
        private IMainTool m_mainTool;
        private ushort? m_snappingCellId = null;

        #endregion

        #region Constructor

        public AddBuildingAction(BuildingInfo building)
        {
            m_building = building;
        }

        #endregion

        #region Action Behaviour Methods

        /// <inheritdoc/>
        public override void OnStart(IMainTool mainTool)
        {
            base.OnStart(mainTool);

            m_mainTool = mainTool;
        }

        public override void OnLeftMouseIsUp(Vector3 mousePosition)
        {
            base.OnLeftMouseIsUp(mousePosition);

            var cell = m_mainTool.Distribution.FindCell(mousePosition, 20f);
            if (cell != null)
            {
                m_mainTool.AddPreference(cell.GridId, m_building);
            }
        }

        public override void OnRenderGeometry(RenderManager.CameraInfo cameraInfo, Vector3 mousePosition)
        {
            base.OnRenderGeometry(cameraInfo, mousePosition);

            // Gets the midpoint
            var midPoint = Vector3.Lerp(m_mainTool.Selection.Value.a, m_mainTool.Selection.Value.c, 0.5f);

            // Gets TRS matrix
            Matrix4x4 matrixTRS = Matrix4x4.TRS(midPoint, Quaternion.AngleAxis(0, Vector3.down), Vector3.one);

            Color buildingColor = BuildingUtils.GetColor(0, m_building);

            var cell = m_mainTool.Distribution.FindCell(mousePosition, 20f);
            m_snappingCellId = cell?.GridId;
            if (cell == null)
            {
                BuildingUtils.RenderBuildingGeometry(cameraInfo, ref matrixTRS, midPoint, mousePosition, -m_mainTool.SelectionAngle.Value, m_building, buildingColor);
            }
            else
            {
                BuildingUtils.RenderBuildingGeometry(cameraInfo, ref matrixTRS, midPoint, cell.Position, cell.Rotation, m_building, buildingColor);
            }
        }

        public override void OnRenderOverlay(RenderManager.CameraInfo cameraInfo, Vector3 mousePosition)
        {
            base.OnRenderOverlay(cameraInfo, mousePosition);

            foreach (var cell in m_mainTool.Distribution.Parcels)
            {
                RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.PointerColor, cell.Position, m_snappingCellId.HasValue && m_snappingCellId == cell.GridId ? 20f : 10f, -1f, 1280f, false, true);
            }
        }

        #endregion
    }
}
