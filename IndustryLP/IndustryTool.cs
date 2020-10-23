using ColossalFramework.Math;
using ColossalFramework.UI;
using IndustryLP.Actions;
using IndustryLP.Tools;
using IndustryLP.UI;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IndustryLP
{
    /// <summary>
    /// This class represents the main funtionality of the mod
    /// </summary>
    public class IndustryTool : ToolBase, IMainTool
    {
        #region Attributes

        /// <summary>
        /// Represents current instance
        /// </summary>
        public static IndustryTool instance = null;

        /// <summary>
        /// Represents current atlas
        /// </summary>
        private static UITextureAtlas m_atlas = null;

        /// <summary>
        /// Toolbar button
        /// </summary>
        private UIButton m_toolbarButton = null;

        /// <summary>
        /// Scrollable panel
        /// </summary>
        private UIScrollablePanel m_scrollablePanel = null;

        /// <summary>
        /// Options panel
        /// </summary>
        private IndustryOptionPanel m_optionPanel = null;

        private ToolAction m_action = null;

        private Vector3? m_mouseTerrainPosition = null;
        private bool m_mouseHoverToolbar = false;
        private bool m_mouseHoverOptionPanel = false;
        private bool m_mouseHoverScrollablePanel = false;

        #region Actions

        private ToolAction m_zoningAction;
        private ToolAction m_movingZoneAction;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets the texture atlas
        /// </summary>
        public static UITextureAtlas Atlas
        {
            get
            {
                if (m_atlas == null)
                {
                    string[] spriteNames =
                    {
                        ResourceConstants.ToolbarNormal,
                        ResourceConstants.ToolbarHovered,
                        ResourceConstants.ToolbarFocused,
                        ResourceConstants.ToolbarPressed,
                        ResourceConstants.ToolbarDisabled,
                        ResourceConstants.SolutionUp,
                        ResourceConstants.SolutionDown,
                        ResourceConstants.OptionBuild,
                        ResourceConstants.OptionMove
                    };

                    // Loads custom resources
                    m_atlas = ResourceLoader.CreateTextureAtlas(ResourceConstants.AtlasName, spriteNames, ResourceConstants.IconsPath);

                    var defaultAtlas = ResourceLoader.GetAtlas("Ingame");
                    Texture2D[] textures =
                    {
                        defaultAtlas[ResourceConstants.OptionZoning].texture,
                        defaultAtlas[ResourceConstants.ToolbarFgNormal].texture,
                        defaultAtlas[ResourceConstants.ToolbarFgHovered].texture,
                        defaultAtlas[ResourceConstants.ToolbarFgFocused].texture,
                        defaultAtlas[ResourceConstants.ToolbarFgPressed].texture,
                        defaultAtlas[ResourceConstants.ToolbarFgDisabled].texture,
                        defaultAtlas[ResourceConstants.OptionFgNormal].texture,
                        defaultAtlas[ResourceConstants.OptionFgHovered].texture,
                        defaultAtlas[ResourceConstants.OptionFgFocused].texture,
                        defaultAtlas[ResourceConstants.OptionFgPressed].texture,
                        defaultAtlas[ResourceConstants.OptionFgDisabled].texture,
                    };

                    // Add default resources
                    ResourceLoader.AddTexturesInAtlas(Atlas, textures);
                }

                return m_atlas;
            }
        }

        /// <summary>
        /// The name of the object
        /// </summary>
        public static string ObjectName => LibraryConstants.ObjectPrefix + ".ToolBehaviour";

        public Quad3 Selection { get; set; }

        public float SelectionAngle { get; set; }

        #endregion

        #region Unity Behaviour methods

        /// <summary>
        /// Invoked when the tool is created
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_toolController = FindObjectOfType<ToolController>();
            name = ObjectName;

            LoggerUtils.Log("Loading IndustryLP");

            SetupToolbarButton();
            SetupOptionPanel();
            SetupScrollablePanel();
            SetupActions();

            LoggerUtils.Log("Finished");
        }

        /// <summary>
        /// Invoked when the tool is enabled
        /// </summary>
        protected override void OnEnable()
        {
            LoggerUtils.Log("Enabled");
            
            base.OnEnable();
            
            m_optionPanel.Show();

            m_action = m_zoningAction;
            m_action.OnEnterController();
        }

        /// <summary>
        /// Invoked when the tool is disabled
        /// </summary>
        protected override void OnDisable()
        {
            LoggerUtils.Log("Disabled");

            base.OnDisable();

            m_optionPanel.Hide();

            m_action.OnLeftController();
            m_action = null;
        }

        /// <summary>
        /// Invoked when the tool will be updated
        /// </summary>
        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            var mainToolbar = ToolsModifierControl.mainToolbar.component as UITabstrip;

            // Checks if mouse is over UI
            if (!m_mouseHoverOptionPanel && !m_mouseHoverScrollablePanel && !m_mouseHoverToolbar)
            {
                m_mouseTerrainPosition = GetTerrainMousePosition();

                m_action?.OnUpdate(m_mouseTerrainPosition.Value);

                if (Input.GetMouseButtonDown(0))
                {
                    m_action?.OnLeftMouseIsDown(m_mouseTerrainPosition.Value);
                }
                else if (Input.GetMouseButton(0))
                {
                    m_action?.OnLeftMouseIsPressed(m_mouseTerrainPosition.Value);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    m_action?.OnLeftMouseIsUp(m_mouseTerrainPosition.Value);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    m_action?.OnRightMouseIsDown(m_mouseTerrainPosition.Value);
                }
                else if (Input.GetMouseButton(1))
                {
                    m_action?.OnRightMouseIsPressed(m_mouseTerrainPosition.Value);
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    m_action?.OnRightMouseIsUp(m_mouseTerrainPosition.Value);
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
        /// Renders a geometry effect
        /// </summary>
        /// <param name="cameraInfo"></param>
        public override void RenderGeometry(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderGeometry(cameraInfo);
            if (!m_mouseHoverOptionPanel && !m_mouseHoverScrollablePanel && !m_mouseHoverToolbar)
                m_action?.OnRenderGeometry(cameraInfo, m_mouseTerrainPosition.Value);
        }

        /// <summary>
        /// Renders a overlay effect
        /// </summary>
        /// <param name="cameraInfo"></param>
        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);
            if (!m_mouseHoverOptionPanel && !m_mouseHoverScrollablePanel && !m_mouseHoverToolbar)
                m_action?.OnRenderOverlay(cameraInfo, m_mouseTerrainPosition.Value);
        }

        /// <summary>
        /// Invoked in simulation step updating
        /// </summary>
        public override void SimulationStep()
        {
            base.SimulationStep();
            if (!m_mouseHoverOptionPanel && !m_mouseHoverScrollablePanel && !m_mouseHoverToolbar)
                m_action?.OnSimulationStep(m_mouseTerrainPosition.Value);
        }

        #endregion

        #region Public methods

        public void CancelZoning()
        {
            m_optionPanel.DisableTab(1);
            m_optionPanel.DisableTab(2);
        }

        public void DoZoning(Quad3 zone)
        {
            Selection = zone;
            m_optionPanel.EnableTab(1);
            m_optionPanel.DisableTab(2);
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

        #endregion

        #region Private methods

        /// <summary>
        /// Creates the main toolbar button
        /// </summary>
        private void SetupToolbarButton()
        {
            // Gets the main toolbar
            var mainToolbar = ToolsModifierControl.mainToolbar.component as UITabstrip;

            mainToolbar.eventMouseEnter += OnMouseEnterToolbar;
            mainToolbar.eventMouseLeave += OnMouseLeaveToolbar;

            // Gets the templates
            var mainButtonGameObject = UITemplateManager.GetAsGameObject("MainToolbarButtonTemplate");
            var subPanelGameObject = UITemplateManager.GetAsGameObject("ScrollableSubPanelTemplate");

            // Creates the main button
            m_toolbarButton = mainToolbar.AddTab(LibraryConstants.ObjectPrefix + ".ToolButton", mainButtonGameObject, subPanelGameObject, new Type[] { typeof(IndustryGroupPanel) }) as UIButton;

            m_toolbarButton.atlas = Atlas;
            m_toolbarButton.playAudioEvents = true;

            // Background sprites
            m_toolbarButton.normalBgSprite = ResourceConstants.ToolbarNormal;
            m_toolbarButton.hoveredBgSprite = ResourceConstants.ToolbarHovered;
            m_toolbarButton.pressedBgSprite = ResourceConstants.ToolbarPressed;
            m_toolbarButton.focusedBgSprite = ResourceConstants.ToolbarFocused;
            m_toolbarButton.disabledBgSprite = ResourceConstants.ToolbarDisabled;

            // Foreground sprites
            m_toolbarButton.normalFgSprite = ResourceConstants.ToolbarFgNormal;
            m_toolbarButton.hoveredFgSprite = ResourceConstants.ToolbarFgHovered;
            m_toolbarButton.pressedFgSprite = ResourceConstants.ToolbarFgPressed;
            m_toolbarButton.focusedFgSprite = ResourceConstants.ToolbarFgFocused;
            m_toolbarButton.disabledFgSprite = ResourceConstants.ToolbarFgDisabled;

            // Events
            m_toolbarButton.eventClicked += OnClickToolbarButton;

            // Tooltip
            m_toolbarButton.tooltip = ModInfo.ModName;
        }

        /// <summary>
        /// Creates the option panel
        /// </summary>
        private void SetupOptionPanel()
        {
            m_optionPanel = GameObjectUtils.AddUIComponent<IndustryOptionPanel>();
            m_optionPanel.relativePosition = new Vector2(474 - m_optionPanel.width, 949);
            m_optionPanel.selectedIndex = 0;
            m_optionPanel.eventSelectedIndexChanged += OnChangeSelectedIndex;
            m_optionPanel.eventMouseEnter += OnMouseEnterOptionPanel;
            m_optionPanel.eventMouseLeave += OnMouseLeaveOptionPanel;
        }

        /// <summary>
        /// Setup the scrollable panel
        /// </summary>
        private void SetupScrollablePanel()
        {
            // Gets the main toolbar
            var mainToolbar = ToolsModifierControl.mainToolbar.component as UITabstrip;
            
            // Gets the main group panel
            var groupPanel = mainToolbar.GetComponentInContainer(m_toolbarButton, typeof(IndustryGroupPanel)) as IndustryGroupPanel;
            groupPanel.name = IndustryGroupPanel.Name;
            groupPanel.enabled = true;
            groupPanel.component.isInteractive = true;
            groupPanel.m_OptionsBar = m_optionPanel;
            groupPanel.m_DefaultInfoTooltipAtlas = ToolsModifierControl.mainToolbar.m_DefaultInfoTooltipAtlas;
            if (ToolsModifierControl.mainToolbar.enabled)
            {
                groupPanel.RefreshPanel();
            }

            // Gets scrollable toolbar
            m_scrollablePanel = groupPanel.GetComponentInChildren<UIScrollablePanel>();
            m_scrollablePanel.eventVisibilityChanged += OnChangeVisibilityPanel;
            m_scrollablePanel.eventMouseEnter += OnMouseEnterScrollablePanel;
            m_scrollablePanel.eventMouseLeave += OnMouseLeaveScrollablePanel;
        }

        /// <summary>
        /// Creates the tool actions
        /// </summary>
        private void SetupActions()
        {
            m_zoningAction = new ZoningAction();
            m_zoningAction.OnStart(this);
            m_movingZoneAction = new MovingZoneAction();
            m_movingZoneAction.OnStart(this);
        }

        private void OnMouseEnterToolbar(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_mouseHoverToolbar = true;
        }

        private void OnMouseLeaveToolbar(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_mouseHoverToolbar = false;
        }

        private void OnMouseEnterOptionPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_mouseHoverOptionPanel = true;
        }

        private void OnMouseLeaveOptionPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_mouseHoverOptionPanel = false;
        }

        private void OnMouseEnterScrollablePanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_mouseHoverScrollablePanel = true;
        }

        private void OnMouseLeaveScrollablePanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_mouseHoverScrollablePanel = false;
        }

        /// <summary>
        /// Invoked when the user clicks in the toolbar button
        /// </summary>
        /// <param name="component"></param>
        /// <param name="eventParam"></param>
        private void OnClickToolbarButton(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (eventParam.buttons.IsFlagSet(UIMouseButton.Left))
            {
                enabled = !enabled;
            }
        }

        /// <summary>
        /// Invoked when CS changes the visibility of the toolbar scrollable panel
        /// </summary>
        /// <param name="component"></param>
        /// <param name="visibility"></param>
        private void OnChangeVisibilityPanel(UIComponent component, bool visibility)
        {
            enabled = visibility;
        }

        /// <summary>
        /// Invoked when the user clicks in a option button
        /// </summary>
        /// <param name="component"></param>
        /// <param name="selectedIndex"></param>
        private void OnChangeSelectedIndex(UIComponent component, int selectedIndex)
        {
            m_action?.OnLeftController();
            var oldAction = m_action;

            switch (selectedIndex)
            {
                case 0:
                    m_action = m_zoningAction;
                    break;
                case 1:
                    m_action = m_movingZoneAction;
                    break;
                default:
                    m_action = null;
                    break;
            }

            m_action?.OnChangeController(oldAction);
            m_action?.OnEnterController();
        }

        /// <summary>
        /// Translate the current position of mouse in terrain position
        /// </summary>
        private Vector3 GetTerrainMousePosition()
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            GetColisingWithTerrain(mouseRay, out Vector3 output);
            return output;
        }

        #endregion
    }
}
