using IndustryLP.Entities;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using UnityEngine;

namespace IndustryLP.Actions
{
    internal class AddBuildingAction : ToolAction
    {
        #region Attribute

        private readonly BuildingInfo m_building;
        private readonly ActionType m_type;
        private IMainTool m_mainTool;
        private ushort? m_snappingCellId = null;

        #endregion

        public enum ActionType
        {
            Preference, Restriction
        }

        #region Constructor

        public AddBuildingAction(BuildingInfo building, ActionType type)
        {
            m_building = building;
            m_type = type;
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
                switch (m_type)
                {
                    case ActionType.Preference:
                        m_mainTool.AddPreference(cell.GridId, m_building);
                        break;
                    case ActionType.Restriction:
                        m_mainTool.AddRestriction(cell.GridId, m_building);
                        break;
                }
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

            RenderBuildingList(cameraInfo, ref matrixTRS, midPoint);
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

        #region Private methods
        
        private void RenderBuildingList(RenderManager.CameraInfo cameraInfo, ref Matrix4x4 matrixTRS, Vector3 center)
        {
            var buildingList = m_type == ActionType.Preference ? m_mainTool.Preferences : m_mainTool.Restrictions;

            foreach (var building in buildingList)
            {
                Color buildingColor = BuildingUtils.GetColor(0, building.Building);
                BuildingUtils.RenderBuildingGeometry(cameraInfo, ref matrixTRS, center, building.Position, building.Rotation, building.Building, buildingColor);
//#if DEBUG
//                DrawForwardDirection(building);
//#endif
            }
        }

//#if DEBUG
//        private void DrawForwardDirection(Parcel building)
//        {
//            var start = building.Position + Vector3.up * 10;
//            var dir = new Vector3(0, 0, 40);
//            var rotate = Quaternion.AngleAxis(building.Rotation * Mathf.Rad2Deg, Vector3.down);
//            var end = rotate * dir + building.Position + Vector3.up * 10;
//            DrawLine(start, end, new Color32(255, 0, 0, 255));
//        }

//        private void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 1f / 30f)
//        {
//            var lr = GameObjectUtils.AddObjectWithComponent<LineRenderer>();
//            lr.transform.position = start;
//            lr.material = new Material(Shader.Find("Hidden/Internal-Colored"));
//            lr.hideFlags = HideFlags.HideAndDontSave;
//            lr.startColor = color;
//            lr.startWidth = 2;
//            lr.SetPosition(0, start);
//            lr.endColor = color;
//            lr.endWidth = 2;
//            lr.SetPosition(1, end);
//            Object.Destroy(lr.gameObject, duration);
//        }
//#endif

        #endregion
    }
}
