using ColossalFramework.UI;
using IndustryLP.DomainDefinition;
using IndustryLP.Entities;
using IndustryLP.UI.Panels;
using IndustryLP.Utils;
using IndustryLP.Utils.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IndustryLP.Actions
{
    internal class BuildingAction : ToolAction
    {
        #region Attributes

        private IMainTool m_mainTool;
        private UIGeneratorOptionPanel m_panel = null;
        private UIGenerationDialog m_dialog = null;
        private BuildThread m_generator = null;
        private int m_rows;
        private int m_columns;

        #endregion

        #region Properties

        public GenerationState CurrentState { get; set; } = GenerationState.None;

        public UIPanel DialogPanel => m_dialog;

        public UIPanel GeneratorOptionPanel => m_panel;

        #endregion

        #region Action Behaviour methods

        public override void OnStart(IMainTool mainTool)
        {
            base.OnStart(mainTool);
            m_mainTool = mainTool;
        }

        public override void OnEnterController()
        {
            base.OnEnterController();
            m_rows = m_mainTool.Distribution.Rows;
            m_columns = m_mainTool.Distribution.Columns;
            CurrentState = GenerationState.None;
            m_dialog = SetupDialog();
        }

        public override void OnLeftController()
        {
            base.OnLeftController();
            if (m_dialog != null)
            {
                Object.DestroyImmediate(m_dialog);
                m_dialog = null;
            }
            if (m_panel != null)
            {
                Object.DestroyImmediate(m_panel);
                m_panel = null;
            }
            if (m_generator != null)
            {
                m_generator.Stop();
                Object.DestroyImmediate(m_generator);
                m_generator = null;
            }
            CurrentState = GenerationState.None;
        }

        public override void OnUpdate(Vector3 mousePosition)
        {
            base.OnUpdate(mousePosition);

            if (CurrentState == GenerationState.GeneratingSolutions)
            {
                if (m_panel == null)
                {
                    m_panel = SetupPanel();
                }
                else if (m_generator != null)
                {
                    m_panel.SetSolutions(m_generator.Count);
                }
                
                if (m_generator == null)
                {
                    m_generator = SetupBuildThread();
                }
                else if (m_generator.IsFinished)
                {
                    CurrentState = GenerationState.GeneratedSolutions;
                    if (m_generator.IsSatisfiable) 
                    {
                        m_panel.StopLoading();
                    }
                    else
                    {
                        m_panel.SetUnsatisfiable();
                    }
                }
            }
        }

        public override void OnRenderGeometry(RenderManager.CameraInfo cameraInfo, Vector3 mousePosition)
        {
            if (m_panel?.Solution > 0 && m_generator?.Count > 0)
            {
                var selection = m_mainTool.Selection.Value;
                var midPoint = Vector3.Lerp(selection.a, selection.c, 0.5f);
                var matrixTRS = Matrix4x4.TRS(midPoint, Quaternion.AngleAxis(0, Vector3.down), Vector3.one);
                var solution = m_generator.GetSolution(m_panel.Solution - 1);

                for (var row = 0; row < m_rows; row++)
                {
                    for (var column = 0; column < m_columns; column++)
                    {
                        var building = solution?.Parcels[row, column];
                        if (!string.IsNullOrEmpty(building))
                        {
                            var prefab = PrefabCollection<BuildingInfo>.FindLoaded(building);
                            if (prefab == null)
                            {
                                LoggerUtils.Warning("Prefab not found", building);
                            }
                            else
                            {
                                var gridId = m_mainTool.Distribution.GetId(row, column);
                                var parcel = m_mainTool.Distribution.FindById(gridId);
                                if (parcel != null)
                                {
                                    var color = BuildingUtils.GetColor(gridId, prefab);

                                    BuildingUtils.RenderBuildingGeometry(cameraInfo, ref matrixTRS, midPoint, parcel.Position, parcel.Rotation, prefab, color);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Private methods

        #region Setup UI

        private UIGenerationDialog SetupDialog()
        {
            var dialog = GameObjectUtils.AddUIComponent<UIGenerationDialog>();
            var screen = UIView.GetAView().GetScreenResolution();
            dialog.relativePosition = new Vector2((screen.x / 2f) - 150f, (screen.y / 2f) - 150f);
            dialog.OnCloseDialog += OnCloseDialog;
            dialog.OnClickAcceptButton += OnAcceptDialog;
            return dialog;
        }

        private UIGeneratorOptionPanel SetupPanel()
        {
            var panel = GameObjectUtils.AddUIComponent<UIGeneratorOptionPanel>();
            panel.relativePosition = new Vector3(791, 847); 
            panel.OnClickNextSolution += OnChangeSolution;
            panel.OnClickPrevSolution += OnChangeSolution;
            panel.OnClickBuildSolution += OnBuildSolution;
            return panel;
        }

        private BuildThread SetupBuildThread()
        {
            var thread = GameObjectUtils.AddObjectWithComponent<BuildThread>();
            thread.StartProgram(m_dialog.Solutions, m_rows, m_columns, ConvertTo(m_mainTool.Preferences), ConvertTo(m_mainTool.Restrictions), m_dialog.AdvancedEdition);
            return thread;
        }

        #endregion

        #region Dialog events

        private void OnCloseDialog(UIComponent component, UIMouseEventParameter eventParameter)
        {
            m_mainTool.CancelGeneration();
        }

        private void OnAcceptDialog(UIComponent component, UIMouseEventParameter eventParameter)
        {
            CurrentState = GenerationState.GeneratingSolutions;
            m_dialog.Hide();
        }

        #endregion

        private void OnChangeSolution(UIComponent component, UIMouseEventParameter eventParameter)
        {
            LoggerUtils.Debug($"Solution {m_panel.Solution}: ");
            var solution = m_generator.GetSolution(m_panel.Solution - 1);

            for (int i = 0; i < m_rows; i++)
            {
                for (int j = 0; j < m_columns; j++)
                {
                    string buildingName = solution.Parcels[i, j];
                    LoggerUtils.Debug($"[{i}, {j}] = {buildingName}");
                }
            }
        }

        private void OnBuildSolution(UIComponent component, UIMouseEventParameter eventParameter)
        {
            var solution = m_generator.GetSolution(m_panel.Solution - 1);
            m_mainTool.BuildGeneration(solution);
        }

        private List<BuildingAtom> ConvertTo(List<Parcel> parcels)
        {
            return parcels.Select(parcel => {
                var position = m_mainTool.Distribution.GetGridPosition(parcel.GridId);
                return new BuildingAtom { Row = position.First, Column = position.Second, Name = parcel.Building.name };
            }).ToList();
        }

        #endregion
    }
}
