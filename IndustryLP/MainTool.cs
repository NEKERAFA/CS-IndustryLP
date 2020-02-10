using UnityEngine;
using ColossalFramework.UI;
using IndustryLP.Utils.Constants;
using IndustryLP.Utils;
using IndustryLP.UI;
using System.Collections.Generic;
using IndustryLP.Tools;
using IndustryLP.UI.Buttons;

namespace IndustryLP
{
    /// <summary>
    /// This class represents the main funtionality of the mod
    /// </summary>
    public class MainTool : ToolBase
    {
        #region Attributes

        private ToolPanel m_mainView;
        private UITextureAtlas m_textureAtlas;
        private SelectionTool m_selectionState;
        private GeneratorTool m_generatorState;
        private ToolActionController m_currentState;

        #endregion

        #region Properties

        /// <summary>
        /// The name of the object
        /// </summary>
        public static string ObjectName => LibraryConstants.ObjectPrefix + "_ToolWindow";

        private ToolActionController CurrentState
        {
            set
            {
                var oldState = m_currentState;
                m_currentState = value;
                m_currentState.OnLeftController();
                m_currentState.OnChangeController(oldState);
            }
        }

        #endregion

        #region Unity Behaviour

        /// <summary>
        /// Invoked when the tool is created
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_toolController = FindObjectOfType<ToolController>();
            name = ObjectName;
            
            LoggerUtils.Log("Loading IndustryLP");

            if (m_textureAtlas == null)
            {
                LoggerUtils.Log("Loading altas");
                SetupResources();
            }

            m_selectionState = new SelectionTool();
            m_selectionState.OnStart(this);
            m_generatorState = new GeneratorTool();
            m_generatorState.OnStart(this);
            m_currentState = null;

            if (m_mainView == null)
            {
                LoggerUtils.Log("Setting tool panel");
                SetupToolPanel();
            }


            LoggerUtils.Log("Finish");
        }

        /// <summary>
        /// Invoked when the tool is going to remove of the scene
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (m_mainView != null)
            {
                Destroy(m_mainView);
                m_mainView = null;
            }

            if (m_textureAtlas != null)
            {
                var sprites = new List<UITextureAtlas.SpriteInfo>();
                m_textureAtlas.sprites.ForEach(s => sprites.Add(s));
                sprites.ForEach(s => m_textureAtlas.Remove(s));
                Destroy(m_textureAtlas);
                Resources.UnloadUnusedAssets();
                m_textureAtlas = null;
            }

            if (m_currentState != null)
            {
                m_currentState.OnDestroy();
                m_currentState = null;
            }
        }

        #endregion

        #region Tool Behaviour

        /// <summary>
        /// Creates the background pannel
        /// </summary>
        private void SetupToolPanel()
        {
            var view = UIView.GetAView();
            m_mainView = view.AddUIComponent<ToolPanel>();
            m_mainView.transform.parent = view.transform;
            m_mainView.transform.localPosition = Vector3.zero;
            m_mainView.relativePosition = new Vector3(80f, 10f);
            m_mainView.ToolActions = new List<ToolPanel.ToolAction>()
            {
                new ToolPanel.ToolAction()
                {
                    Controller = m_selectionState,
                    Callback = isChecked => OnSelectionPressed(isChecked)
                },
                new ToolPanel.ToolAction()
                {
                    Controller = m_generatorState,
                    Callback = isChecked => OnGeneratorPressed(isChecked)
                }
            };
        }

        /// <summary>
        /// Loads the icons and image buttons
        /// </summary>
        private void SetupResources()
        {
            string[] sprites =
            {
                ResourceConstants.SelectionIcon,
                ResourceConstants.GeneratorIcon,
                ResourceConstants.ButtonHover,
                ResourceConstants.ButtonNormal,
                ResourceConstants.ButtonPushed
            };

            m_textureAtlas = ResourceLoader.CreateTextureAtlas(ResourceConstants.AtlasName, sprites, ResourceConstants.IconPath);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_mainView.DisableAllButtons();
            if (m_currentState != null)
            {
                m_currentState.OnLeftController();
                m_currentState = null;
            }
        }

        /// <summary>
        /// Invoked before new frame rendering
        /// </summary>
        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            if (m_currentState != null)
            {
                var mousePosition = GetTerrainMousePosition();

                m_currentState.OnUpdate(mousePosition);

                if (Input.GetMouseButtonDown(0))
                {
                    m_currentState.OnLeftMouseIsDown(mousePosition);
                }
                else if (Input.GetMouseButton(0))
                {
                    m_currentState.OnLeftMouseIsPressed(mousePosition);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    m_currentState.OnLeftMouseIsUp(mousePosition);
                }
            }
        }

        /// <summary>
        /// Returns if the tool was an error
        /// </summary>
        /// <returns></returns>
        public override ToolErrors GetErrors()
        {
            return ToolErrors.None;
        }

        /// <summary>
        /// Renders a overlay effect
        /// </summary>
        /// <param name="cameraInfo"></param>
        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            if (m_currentState != null)
            {
                m_currentState.OnRenderOverlay(cameraInfo);
            }
        }

        public override void SimulationStep()
        {
            base.SimulationStep();

            if (m_currentState != null)
            {
                m_currentState.OnSimulationStep();
            }
        }

        #endregion

        #region IndustryLP Controller

        private Vector3 GetTerrainMousePosition()
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            var input = new RaycastInput(mouseRay, Camera.main.farClipPlane);
            input.m_ignoreTerrain = false;
            RayCast(input, out RaycastOutput output);
            return output.m_hitPos;
        }

        private void OnSelectionPressed(bool isChecked)
        {
            if (isChecked)
            {
                if (!enabled) enabled = true;
                CurrentState = m_selectionState;
                m_mainView.DisableButton(GeneratorButton.ObjectName);
            }
        }

        private void OnGeneratorPressed(bool isChecked)
        {
            if (isChecked)
            {
                if (!enabled) enabled = true;
                CurrentState = m_generatorState;
                m_mainView.DisableButton(SelectionButton.ObjectName);
            }
        }

        #endregion
    }
}
