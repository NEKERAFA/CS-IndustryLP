using ColossalFramework;
using ColossalFramework.Math;
using IndustryLP.UI;
using IndustryLP.UI.Buttons;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;

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
            if (m_selection.HasValue && !m_isGeneratedTerrain)
            {
                var pos1 = m_selection.Value.a;
                var pos2 = m_selection.Value.c;

                /*
                // Gets the average point
                //var averagePoint = new Vector3((pos1.x + pos2.x) / 2.0f, (pos1.y + pos2.y) / 2.0f, (pos1.z + pos2.z) / 2.0f);

                // Gets the top point
                //var topPoint = new Vector3(averagePoint.x + 80, averagePoint.y, averagePoint.z);

                var randomizer = Singleton<SimulationManager>.instance.m_randomizer;

                // Gets road info
                var netPrefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road");

                LoggerUtils.Log("Creating net");

                // Create first position
                if (!NetManager.instance.CreateNode(out ushort node1, ref randomizer, netPrefab, pos1, Singleton<SimulationManager>.instance.m_currentBuildIndex + 1))
                    LoggerUtils.Error("Cannot create node1");

                Singleton<SimulationManager>.instance.m_currentBuildIndex++;

                LoggerUtils.Log(Singleton<SimulationManager>.instance.m_currentBuildIndex);

                // Create second position
                if (!NetManager.instance.CreateNode(out ushort node2, ref randomizer, netPrefab, pos2, Singleton<SimulationManager>.instance.m_currentBuildIndex + 1))
                    LoggerUtils.Error("Cannot create node2");

                Singleton<SimulationManager>.instance.m_currentBuildIndex++;

                LoggerUtils.Log(Singleton<SimulationManager>.instance.m_currentBuildIndex);

                // Create segment
                var dir1 = (pos2 - pos1).normalized;
                var dir2 = (pos1 - pos2).normalized;

                if (!NetManager.instance.CreateSegment(out ushort segment, ref randomizer, netPrefab, node1, node2, dir1, dir2, Singleton<SimulationManager>.instance.m_currentBuildIndex + 1, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                    LoggerUtils.Error("Cannot create segment");

                Singleton<SimulationManager>.instance.m_currentBuildIndex++;

                LoggerUtils.Log(Singleton<SimulationManager>.instance.m_currentBuildIndex);
                */

                LoggerUtils.Log("Creating net");
                NetUtils.CreateStraightRoad(pos1, pos2, "Basic Road");

                m_isGeneratedTerrain = true;
            }
        }

        #endregion
    }
}
