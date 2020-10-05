using ColossalFramework.UI;
using IndustryLP.Tools;
using IndustryLP.UI;
using IndustryLP.UI.Buttons;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using IndustryLP.Utils.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace IndustryLP
{
    /// <summary>
    /// This class represents the main funtionality of the mod
    /// </summary>
    public class MainTool : ToolBase
    {
        #region Attributes

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

        /// <summary>
        /// The main tool panel
        /// </summary>
        internal ToolPanel MainPanel { get; private set; }

        private ToolActionController CurrentState
        {
            set
            {
                var oldState = m_currentState;
                if (oldState != null) oldState.OnLeftController();
                m_currentState = value;
                m_currentState.OnChangeController(oldState);
                m_currentState.OnEnterController();
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

            if (MainPanel == null)
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

            if (MainPanel != null)
            {
                Destroy(MainPanel);
                MainPanel = null;
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
            MainPanel = GameObjectUtils.AddUIComponent<ToolPanel>();
            MainPanel.relativePosition = new Vector3(80f, 10f);
            MainPanel.ToolActions = new List<ToolPanel.ToolAction>()
            {
                new ToolPanel.ToolAction()
                {
                    Controller = m_selectionState,
                    Callback = isChecked => OnSelectionPressed(isChecked)
                },
                new ToolPanel.ToolAction()
                {
                    Controller = m_generatorState,
                    Callback = isChecked => OnGeneratorPressed()
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
                ResourceConstants.BuildIcon,
                ResourceConstants.UpArrowIcon,
                ResourceConstants.DownArrowIcon,
                ResourceConstants.ButtonHover,
                ResourceConstants.ButtonNormal,
                ResourceConstants.ButtonPushed,
                ResourceConstants.OptionsIcon
            };

            m_textureAtlas = ResourceLoader.CreateTextureAtlas(ResourceConstants.AtlasName, sprites, ResourceConstants.IconPath);
        }

        /// <summary>
        /// Invoked when the tool is disabled
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            MainPanel.DisableAllButtons();
            m_currentState?.OnLeftController();
            m_currentState = null;
        }

        /// <summary>
        /// Invoked before new frame rendering
        /// </summary>
        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            var mousePosition = GetTerrainMousePosition();

            m_currentState?.OnUpdate(mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                m_currentState?.OnLeftMouseIsDown(mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                m_currentState?.OnLeftMouseIsPressed(mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_currentState?.OnLeftMouseIsUp(mousePosition);
            }

            if (Input.GetMouseButtonDown(1))
            {
                m_currentState?.OnRightMouseIsDown(mousePosition);
            }
            else if (Input.GetMouseButton(1))
            {
                m_currentState?.OnRightMouseIsPressed(mousePosition);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                m_currentState?.OnRightMouseIsUp(mousePosition);
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
        /// Renders a geometry effect
        /// </summary>
        /// <param name="cameraInfo"></param>
        public override void RenderGeometry(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderGeometry(cameraInfo);
            m_currentState?.OnRenderGeometry(cameraInfo);
        }

        /// <summary>
        /// Renders a overlay effect
        /// </summary>
        /// <param name="cameraInfo"></param>
        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);
            m_currentState?.OnRenderOverlay(cameraInfo);
        }

        /// <summary>
        /// Invoked in simulation step updating
        /// </summary>
        public override void SimulationStep()
        {
            base.SimulationStep();
            m_currentState?.OnSimulationStep();
        }

        #endregion

        #region IndustryLP Controller

        /// <summary>
        /// Called when the selection starts
        /// </summary>
        public void StartSelection()
        {
            MainPanel.IsSelectionDone = false;
        }

        /// <summary>
        /// Called when the selection finished
        /// </summary>
        public void FinishSelection()
        {
            MainPanel.IsSelectionDone = true;
        }

        /// <summary>
        /// Called when the selection is cancel
        /// </summary>
        public void CancelSelection()
        {
            MainPanel.DisableButton(SelectionButton.ObjectName);
            m_currentState.OnLeftController();
            m_currentState = null;
        }

        /// <summary>
        /// Obtiene una colision contra el terreno
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool GetColisingWithTerrain(Ray ray, out Vector3 output)
        {
            var raycastInput = new RaycastInput(ray, Camera.main.farClipPlane)
            {
                m_ignoreTerrain = false
            };

            var result = RayCast(raycastInput, out RaycastOutput raycastOutput);
            output = result ? raycastOutput.m_hitPos : default;

            return result;
        }

        /// <summary>
        /// Translate the current position of mouse in terrain position
        /// </summary>
        public Vector3 GetTerrainMousePosition()
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            GetColisingWithTerrain(mouseRay, out Vector3 output);
            return output;
        }

        /// <summary>
        /// Invoked when the selection button is pressed
        /// </summary>
        private void OnSelectionPressed(bool isChecked)
        {
            if (isChecked)
            {
                if (!enabled) enabled = true;
                CurrentState = m_selectionState;
                MainPanel.DisableButton(GenerateOptionsButton.ObjectName);
            }
        }

        /// <summary>
        /// Invoked when the generation button is pressed
        /// </summary>
        private void OnGeneratorPressed()
        {
            if (!enabled) enabled = true;
            if (m_currentState != m_generatorState)
            {
                CurrentState = m_generatorState;
                MainPanel.DisableButton(SelectionButton.ObjectName);
            }
        }

        #endregion
    }
}
