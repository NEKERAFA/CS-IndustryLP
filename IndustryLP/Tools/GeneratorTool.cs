using ColossalFramework;
using ColossalFramework.Math;
using IndustryLP.UI;
using IndustryLP.UI.Buttons;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using UnityEngine;

namespace IndustryLP.Tools
{
    /// <summary>
    /// Represents the generator tool
    /// </summary>
    internal class GeneratorTool : ToolActionController
    {
        #region Attributes

        internal Quad3? m_selection;
        private bool m_isGeneratedTerrain = false;

        #endregion

        #region Generator Behaviour

        public override void OnChangeController(ToolActionController oldController)
        {
            if (oldController is SelectionTool)
            {
                var selectionTool = oldController as SelectionTool;
                if (selectionTool.m_currentMouseSelection.HasValue)
                {
                    m_selection = selectionTool.m_currentMouseSelection;
                }
            }
        }

        public override ToolButton CreateButton(ToolButton.OnButtonPressedDelegate callback)
        {
            var generatorButton = GameObjectUtils.AddObjectWithComponent<GeneratorButton>();

            if (callback != null)
                generatorButton.OnButtonPressed = isChecked => callback(isChecked);

            return generatorButton;
        }

        public override void OnRenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (m_selection.HasValue)
            {
                RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, ColorConstants.SelectedColor, m_selection.Value, -1f, 1280f, false, true);
            }
        }

        public override void OnSimulationStep()
        {
            // Generate terrain
            if (m_selection.HasValue && !m_isGeneratedTerrain)
            {
                // Gets the vertices
                var posA = m_selection.Value.a;
                var posB = m_selection.Value.b;
                var posC = m_selection.Value.c;
                var posD = m_selection.Value.d;

                LoggerUtils.Log("Calculating midpoints");

                // Calculates the midpoints
                var midAB = Vector3.Lerp(posA, posB, 0.5f);
                var midAC = Vector3.Lerp(posA, posC, 0.5f);
                var midAD = Vector3.Lerp(posA, posD, 0.5f);
                var midBC = Vector3.Lerp(posB, posC, 0.5f);
                var midCD = Vector3.Lerp(posC, posD, 0.5f);

                LoggerUtils.Log("Creating intersection");

                // Creates al roads
                var firstRoad = NetUtils.CreateStraightRoad(midAC, midAB, "Basic Road");
                var centerNode = firstRoad.Segments[0].StartPosition;
                NetUtils.CreateStraightRoad(centerNode, midBC, "Basic Road");
                NetUtils.CreateStraightRoad(centerNode, midCD, "Basic Road");
                NetUtils.CreateStraightRoad(centerNode, midAD, "Basic Road");

                m_isGeneratedTerrain = true;
            }
        }

        #endregion
    }
}
