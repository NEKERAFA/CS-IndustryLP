using ColossalFramework.Math;
using ColossalFramework.UI;
using IndustryLP.DomainDefinition;
using IndustryLP.Tools;
using IndustryLP.UI.Panels;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using IndustryLP.Utils.Enums;
using UnityEngine;

namespace IndustryLP.Actions
{
    internal class BuildZoneAction : ToolAction
    {
        #region Attributes

        internal Quad3? m_selection;
        internal float? m_angle;

        private IMainTool m_mainTool;
        private BuildThread m_generator = null;
        private UIBuildPanel m_controls = null;
        private int rows;
        private int cols;

#if DEBUG
        private GUIUtils.UITextDebug debug_m_showSize;
#endif

        #endregion

        #region

        private GenerationState CurrentState { get; set; } = GenerationState.None;

        #endregion

        #region Controller Behaviour

        public override void OnStart(IMainTool mainTool)
        {
            m_mainTool = mainTool;

#if DEBUG
            debug_m_showSize = GameObjectUtils.AddUIComponent<GUIUtils.UITextDebug>();
            debug_m_showSize.Hide();
#endif
        }

#if DEBUG
        public override void OnDestroy()
        {
            Object.Destroy(debug_m_showSize);
            debug_m_showSize = null;
        }

        public override void OnLeftController()
        {
            debug_m_showSize.Hide();
        }
#endif

        /*
        public override void OnChangeController(ToolActionController oldController)
        {
            if (oldController is SelectionTool)
            {
                var selectionTool = oldController as SelectionTool;
                if (selectionTool.m_selection.HasValue)
                {
                    m_selection = selectionTool.m_selection;

                    var rowLength = Vector3.Distance(m_selection.Value.a, m_selection.Value.d);
                    var columnLength = Vector3.Distance(m_selection.Value.a, m_selection.Value.b);

                    rows = System.Convert.ToInt32(System.Math.Floor(rowLength / 40f));
                    cols = System.Convert.ToInt32(System.Math.Floor(columnLength / 40f));

#if DEBUG
                    debug_m_showSize.SetText(string.Format("({0}, {1})", rows, cols));
#endif
                }

                if (selectionTool.m_angle.HasValue)
                {
                    m_angle = -selectionTool.m_angle.Value;
                }
            }
        }
        */

        public override void OnRenderGeometry(RenderManager.CameraInfo cameraInfo, Vector3 mousePosition)
        {
            // Draws the buildings
            if (m_controls != null && m_controls.Solution > 0 && m_generator != null && m_generator.Count > 0)
            {
                // Gets the directions
                var directionB = (m_selection.Value.b - m_selection.Value.a).normalized;
                var directionD = (m_selection.Value.d - m_selection.Value.a).normalized;

                // Gets the midpoint
                var midPoint = Vector3.Lerp(m_selection.Value.a, m_selection.Value.c, 0.5f);

                // Gets the lengths
                var rowSize = Vector3.Distance(m_selection.Value.a, m_selection.Value.b) / rows;
                var columnSize = Vector3.Distance(m_selection.Value.a, m_selection.Value.d) / cols;

                // Gets TRS matrix
                Matrix4x4 matrixTRS = Matrix4x4.TRS(midPoint, Quaternion.AngleAxis(0, Vector3.down), Vector3.one);

                // Gets the current solution
                var solution = m_generator.GetSolution(m_controls.Solution - 1);

                // Iterate over solution
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        string parcel = solution.Parcels[i, j];
                        if (!string.IsNullOrEmpty(parcel?.Trim()))
                        {
                            // Gets the cell
                            BuildingInfo buildingPrefab = PrefabCollection<BuildingInfo>.FindLoaded(solution.Parcels[i, j]);
                            Color buildingColor = BuildingUtils.GetColor(System.Convert.ToUInt16(i * j), buildingPrefab);

                            // Get the position
                            Vector3 position = m_selection.Value.a + (directionB * i * rowSize) + (directionB * rowSize / 2) + (directionD * j * columnSize) + (directionD * columnSize / 2);

                            // Draws the building
                            BuildingUtils.RenderBuildingGeometry(cameraInfo, ref matrixTRS, midPoint, position, m_angle.Value, buildingPrefab, buildingColor);
                        }
                    }
                }
            }
        }

        public override void OnRenderOverlay(RenderManager.CameraInfo cameraInfo, Vector3 mousePosition)
        {
            if (m_selection.HasValue)
            {
                // Gets the midpoint
                var midPoint = Vector3.Lerp(m_selection.Value.a, m_selection.Value.c, 0.5f);

                // Draws the roads
                NetUtils.RenderRoadGrid(cameraInfo, m_selection.Value, rows, cols, ColorConstants.SelectedColor);

                // Draws the buildings
                if (m_controls != null && m_controls.Solution > 0 && m_generator != null && m_generator.Count > 0)
                {
                    var directionB = (m_selection.Value.b - m_selection.Value.a).normalized;
                    var directionD = (m_selection.Value.d - m_selection.Value.a).normalized;

                    // Gets the lengths
                    var rowSize = Vector3.Distance(m_selection.Value.a, m_selection.Value.b) / rows;
                    var columnSize = Vector3.Distance(m_selection.Value.a, m_selection.Value.d) / cols;

                    // Gets TRS matrix
                    Matrix4x4 matrixTRS = Matrix4x4.TRS(midPoint, Quaternion.AngleAxis(0, Vector3.down), Vector3.one);

                    // Gets the current solution
                    var solution = m_generator.GetSolution(m_controls.Solution - 1);

                    // Iterate over solution
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            string parcel = solution.Parcels[i, j];
                            if (!string.IsNullOrEmpty(parcel?.Trim()))
                            {
                                // Gets the cell
                                BuildingInfo buildingPrefab = PrefabCollection<BuildingInfo>.FindLoaded(solution.Parcels[i, j]);

                                // Get the position
                                Vector3 position = m_selection.Value.a + (directionB * i * rowSize) + (directionB * rowSize / 2) + (directionD * j * columnSize) + (directionD * columnSize / 2);

                                // Draws the building
                                //BuildingUtils.RenderBuildingOverlay(cameraInfo, ref matrixTRS, midPoint, position, m_angle.Value, buildingPrefab, ColorConstants.SelectedColor);
                            }
                        }
                    }
                }

#if DEBUG
                if (!debug_m_showSize.isVisible) debug_m_showSize.Show();

                var mainView = UIView.GetAView();
                var newPosition = Camera.main.WorldToScreenPoint(midPoint) / mainView.inputScale;
                debug_m_showSize.relativePosition = mainView.ScreenPointToGUI(newPosition) - new Vector2(debug_m_showSize.width / 2f, debug_m_showSize.height / 2f);
#endif
            }
        }

        public override void OnUpdate(Vector3 mousePosition)
        {
            if (CurrentState == GenerationState.GeneratingSolutions)
            {
                if (m_controls == null)
                {
                    m_controls = GameObjectUtils.AddUIComponent<UIBuildPanel>();
                    m_controls.transform.localPosition = Vector2.zero;
                    m_controls.relativePosition = new Vector2(10, 10);
                    //m_controls.OnUpButtonClick = OnChangeSolution;
                    //m_controls.OnDownButtonClick = OnChangeSolution;
                }
                else if (m_generator != null)
                {
                    m_controls.SetSolutions(m_generator.Count);
                }

                if (m_generator == null)
                {
                    m_generator = GameObjectUtils.AddObjectWithComponent<BuildThread>();
                    m_generator.StartProgram(0, rows, cols);
                }
                else if (m_generator.IsFinished)
                {
                    CurrentState = GenerationState.GeneratedSolutions;
                    m_controls.StopLoading();
                }
            }
        }

        public override void OnSimulationStep(Vector3 mousePosition)
        {
            /*
            // Generate terrain
            if (!m_isGeneratedTerrain)
            {
                if (m_generator == null)
                {
                    m_generator = GameObjectUtils.AddObjectWithComponent<GeneratorThread>();
                    m_generator.StartProgram(1, 2, 2);
                    return;
                }
                
                var result = m_generator.GetSolution(0);

                if (result != null)
                {
                    // Gets the vertices
                    var posA = m_selection.Value.a;
                    var posB = m_selection.Value.b;
                    var posC = m_selection.Value.c;
                    var posD = m_selection.Value.d;

                    // Gets managers
                    var buildingManager = Singleton<BuildingManager>.instance;

                    // Create buildings
                    BuildingWrapper buildingWrapper = new BuildingWrapper(buildingManager);

                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            LoggerUtils.Log("Parcel", i, j, result.Parcels[i, j]);

                            string buildingName = GetBuildingName(result.Parcels[i, j]);

                            if (i == 0 && j == 0)
                            {
                                buildingWrapper.CreateBuilding(posA, 0, buildingName);
                            }
                            else if (i == 0 && j == 1)
                            {
                                buildingWrapper.CreateBuilding(posB, 0, buildingName);
                            }
                            else if (i == 1 && j == 0)
                            {
                                buildingWrapper.CreateBuilding(posC, 0, buildingName);
                            }
                            else
                            {
                                buildingWrapper.CreateBuilding(posD, 0, buildingName);
                            }
                        }
                    }

                    m_isGeneratedTerrain = true;
                }
            }
            */
        }

        #endregion

        #region Generator Behaviour

        #region Dialog events

        private void OnClosePopup(UIComponent c, UIMouseEventParameter p)
        {
            CurrentState = GenerationState.None;
            m_button.Enable();
        }

        private void OnPressGenerate(UIComponent c, UIMouseEventParameter p)
        {
            CurrentState = GenerationState.GeneratingSolutions;
            m_popup.Hide();
        }

        #endregion

        #region Panel event

        private void OnChangeSolution(bool _)
        {
            LoggerUtils.Log($"Solution {m_controls.Solution}: ");
            var solution = m_generator.GetSolution(m_controls.Solution - 1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    LoggerUtils.Log($"[{i}, {j}] = {solution.Parcels[i, j]}");
                }
            }
        }

        #endregion

        #endregion
    }
}
