using ColossalFramework.Globalization;
using ColossalFramework.Math;
using ColossalFramework.UI;
using IndustryLP.Actions;
using IndustryLP.DistributionDefinition;
using IndustryLP.Entities;
using IndustryLP.UI.Panels;
using IndustryLP.UI.Panels.Items;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using IndustryLP.Utils.Enums;
using IndustryLP.Utils.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

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
        /// Represents current icon atlas
        /// </summary>
        private static UITextureAtlas m_iconAtlas = null;

        /// <summary>
        /// Represents current thumbnail atlas
        /// </summary>
        private static UITextureAtlas m_thumbnailAtlas = null;

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
        private UIOptionPanel m_optionPanel = null;

        /// <summary>
        /// Category Options Panel
        /// </summary>
        private UICategoryOptionPanel m_categoryPanel = null;

        /// <summary>
        /// Current action
        /// </summary>
        private ToolAction m_action = null;

        private Vector3? m_mouseTerrainPosition = null;
        private bool m_mouseHoverToolbar = false;
        private bool m_mouseHoverOptionPanel = false;
        private bool m_mouseHoverScrollablePanel = false;
        private float m_defaultXPos;

#if DEBUG
        private Dictionary<ParcelWrapper, GUIUtils.UITextDebug> lblParcels = new Dictionary<ParcelWrapper, GUIUtils.UITextDebug>();
#endif


        #region Actions

        private ToolAction m_zoningAction;
        private ToolAction m_movingZoneAction;
        private ToolAction m_buildingAction;

        #endregion

        #region Distributions

        private DistributionThread gridDistribution;
        private DistributionThread lineDistribution;
        private DistributionThread forestalDistribution;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets the icon texture atlas
        /// </summary>
        public static UITextureAtlas IconAtlas
        {
            get
            {
                if (m_iconAtlas == null)
                {
                    string[] iconsNames =
                    {
                        ResourceConstants.ToolbarNormal,
                        ResourceConstants.ToolbarHovered,
                        ResourceConstants.ToolbarFocused,
                        ResourceConstants.ToolbarPressed,
                        ResourceConstants.ToolbarDisabled,
                        ResourceConstants.SolutionUp,
                        ResourceConstants.SolutionDown,
                        ResourceConstants.OptionBuild,
                        ResourceConstants.OptionMove,
                        ResourceConstants.SubBarPreferenceNormal,
                        ResourceConstants.SubBarPreferenceFocused,
                        ResourceConstants.SubBarPreferenceHovered,
                        ResourceConstants.SubBarPreferencePressed,
                        ResourceConstants.SubBarPreferenceDisabled,
                        ResourceConstants.SubBarRestrictionNormal,
                        ResourceConstants.SubBarRestrictionFocused,
                        ResourceConstants.SubBarRestrictionHovered,
                        ResourceConstants.SubBarRestrictionPressed,
                        ResourceConstants.SubBarRestrictionDisabled,
                        ResourceConstants.BuildNow
                    };

                    m_iconAtlas = ResourceLoader.CreateTextureAtlas(ResourceConstants.IconAtlasName, iconsNames, ResourceConstants.IconsPath);

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
                        defaultAtlas[ResourceConstants.SubBarBackgroundNormal].texture,
                        defaultAtlas[ResourceConstants.SubBarBackgroundDisabled].texture,
                        defaultAtlas[ResourceConstants.SubBarBackgroundFocused].texture,
                        defaultAtlas[ResourceConstants.SubBarBackgroundHovered].texture,
                        defaultAtlas[ResourceConstants.SubBarBackgroundPressed].texture,
                        defaultAtlas[ResourceConstants.SubBarDistributionNormal].texture,
                        defaultAtlas[ResourceConstants.SubBarDistributionDisabled].texture,
                        defaultAtlas[ResourceConstants.SubBarDistributionFocused].texture,
                        defaultAtlas[ResourceConstants.SubBarDistributionHovered].texture,
                        defaultAtlas[ResourceConstants.SubBarDistributionPressed].texture,
                        defaultAtlas[ResourceConstants.Loading].texture
                    };

                    // Add resources
                    ResourceLoader.AddTexturesInAtlas(m_iconAtlas, textures, true);
                }

                return m_iconAtlas;
            }
        }

        /// <summary>
        /// Gets the thumbnail texture atlas
        /// </summary>
        public static UITextureAtlas ThumbnailAtlas
        {
            get
            {
                if (m_thumbnailAtlas == null)
                {
                    string[] distributionsNames =
                    {
                        ResourceConstants.DistributionDisabled,
                        ResourceConstants.GridDistributionNormal,
                        ResourceConstants.GridDistributionHovered,
                        ResourceConstants.GridDistributionPressed,
                        ResourceConstants.GridDistributionFocused,
                        ResourceConstants.LineDistributionNormal,
                        ResourceConstants.LineDistributionHovered,
                        ResourceConstants.LineDistributionPressed,
                        ResourceConstants.LineDistributionFocused,
                        ResourceConstants.ForestalDistributionNormal,
                        ResourceConstants.ForestalDistributionHovered,
                        ResourceConstants.ForestalDistributionPressed,
                        ResourceConstants.ForestalDistributionFocused,
                    };

                     m_thumbnailAtlas = ResourceLoader.CreateTextureAtlas(ResourceConstants.DistributionAtlasName, distributionsNames, ResourceConstants.DistributionsPath);
                }

                return m_thumbnailAtlas;
            }
        }

        /// <summary>
        /// The name of the object
        /// </summary>
        public static string ObjectName => LibraryConstants.ObjectPrefix + ".ToolBehaviour";

        public Quad3? Selection { get; set; } = null;

        public float? SelectionAngle { get; set; } = null;

        /// <summary>
        /// Current distribution
        /// </summary>
        public DistributionInfo Distribution { get; set; } = null;

        public List<Parcel> Preferences { get; set; } = new List<Parcel>();

        public List<Parcel> Restrictions { get; set; } = new List<Parcel>();

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
            SetupTutorialPanel();
            SetupOptionPanel();
            SetupScrollablePanels();
            SetupActions();
            SetupDistributions();
            

            LoggerUtils.Log("Finished");
        }

        /// <summary>
        /// Invoked when the tool is enabled
        /// </summary>
        protected override void OnEnable()
        {
            LoggerUtils.Log("Enabled");
            
            base.OnEnable();

            if (m_optionPanel != null)
            {
                m_optionPanel.Show();
            }

            if (m_optionPanel != null)
            {
                m_action = m_zoningAction;
                m_action.OnEnterController();
            }
        }

        /// <summary>
        /// Invoked when the tool is disabled
        /// </summary>
        protected override void OnDisable()
        {
            LoggerUtils.Log("Disabled");

            base.OnDisable();

            if (m_optionPanel != null) {
                m_optionPanel.Hide();
            }

            if (m_action != null)
            {
                m_action.OnLeftController();
                m_action = null;
            }

            Preferences.Clear();
            Restrictions.Clear();
        }

        /// <summary>
        /// Invoked when the tool will be updated
        /// </summary>
        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            m_mouseTerrainPosition = TerrainUtils.GetTerrainMousePosition();

            // Checks if mouse is over UI
            if (!IsPointerOverUIView())
            {
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

                if (Input.GetMouseButtonUp(1) && (Preferences.Any() || Restrictions.Any()))
                {
                    RemoveBuilding(m_mouseTerrainPosition.Value);
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

            if (m_mouseTerrainPosition.HasValue)
                m_action?.OnRenderGeometry(cameraInfo, m_mouseTerrainPosition.Value);
        }

        /// <summary>
        /// Renders a overlay effect
        /// </summary>
        /// <param name="cameraInfo"></param>
        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            if (m_mouseTerrainPosition.HasValue)
            {
                if (m_action != null)
                {
                    m_action.OnRenderOverlay(cameraInfo, m_mouseTerrainPosition.Value);
                }
                else
                {
                    var cell = Utils.MathUtils.FindNeighbour(Preferences, m_mouseTerrainPosition.Value, 20);
                    if (cell != null)
                    {
                        RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.PreferenceColor, cell.Position, 40f, -1f, 1280f, false, true);
                    }

                    cell = Utils.MathUtils.FindNeighbour(Restrictions, m_mouseTerrainPosition.Value, 20);
                    if (cell != null)
                    {
                        RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.RestrictionColor, cell.Position, 40f, -1f, 1280f, false, true);
                    }
                }

            }

            if (Distribution != null)
            {
                DistributionUtils.RenderSegments(cameraInfo, ColorConstants.SelectionColor, Distribution.Road);
            }

            if (Selection.HasValue)
            {
                var midPoint = Vector3.Lerp(Selection.Value.a, Selection.Value.c, 0.5f);
                Matrix4x4 matrixTRS = Matrix4x4.TRS(midPoint, Quaternion.AngleAxis(0, Vector3.down), Vector3.one);

                foreach (var preference in Preferences)
                {
                    BuildingUtils.RenderBuildingOverlay(cameraInfo, ref matrixTRS, midPoint, preference.Position, preference.Rotation, preference.Building, ColorConstants.PreferenceColor);
                }

                foreach (var restriction in Restrictions)
                {
                    BuildingUtils.RenderBuildingOverlay(cameraInfo, ref matrixTRS, midPoint, restriction.Position, restriction.Rotation, restriction.Building, ColorConstants.PreferenceColor);
                }
            }

#if DEBUG
            if (lblParcels.Any())
            {
                var mainView = UIView.GetAView();

                foreach (var lbl in lblParcels)
                {
                    if (!lbl.Value.isVisible) lbl.Value.Show();
                    var pos = Camera.main.WorldToScreenPoint(lbl.Key.Position) / mainView.inputScale;
                    lbl.Value.relativePosition = mainView.ScreenPointToGUI(pos) - new Vector2(lbl.Value.width / 2f, lbl.Value.height / 2f);
                }
            }
#endif
        }

        /// <summary>
        /// Invoked in simulation step updating
        /// </summary>
        public override void SimulationStep()
        {
            base.SimulationStep();

            if (m_mouseTerrainPosition.HasValue)
                m_action?.OnSimulationStep(m_mouseTerrainPosition.Value);
        }

        #endregion

        #region Public methods

        /// <inheritdoc/>
        public void CancelZoning()
        {
            if (Distribution != null)
            {
                Distribution = null;

#if DEBUG
                foreach (var lbl in lblParcels.Values)
                {
                    DestroyImmediate(lbl);
                }

                lblParcels.Clear();
#endif
            }

            m_optionPanel.DisableTab(1);
            m_optionPanel.DisableTab(2);
            m_categoryPanel.DisableTab(0);
            m_categoryPanel.DisableTab(1);
            m_categoryPanel.DisableTab(2);
            m_scrollablePanel.Disable();
            Preferences.Clear();
            Restrictions.Clear();
        }

        /// <inheritdoc/>
        public void DoZoning(Quad3 selection, float angle)
        {
            Selection = selection;
            SelectionAngle = angle;
            m_optionPanel.EnableTab(1);
            m_categoryPanel.EnableTab(0);
            m_categoryPanel.selectedIndex = 0;
            m_scrollablePanel.Enable();

            if (Distribution != null)
            {
                m_optionPanel.EnableTab(2);
                if (Distribution?.Type == DistributionType.GRID)
                {
                    var distributionThread = new GridDistributionThread();
                    Distribution = distributionThread.Generate(Selection.Value);
                    m_categoryPanel.EnableTab(1);
                    m_categoryPanel.EnableTab(2);

#if DEBUG
                    foreach (var lbl in lblParcels.Values)
                    {
                        DestroyImmediate(lbl);
                    }

                    lblParcels.Clear();

                    foreach (var parcel in Distribution.Parcels)
                    {
                        var lbl = GameObjectUtils.AddUIComponent<GUIUtils.UITextDebug>();
                        lbl.Hide();
                        lbl.SetText(Convert.ToString((int)parcel.GridId));
                        lblParcels[parcel] = lbl;
                    }
#endif
                 }
            }
            else
            {
                m_optionPanel.DisableTab(2);
            }

            Preferences.Clear();
            Restrictions.Clear();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void SetGridDistribution()
        {
            if (Selection.HasValue && SelectionAngle.HasValue)
            {
#if DEBUG
                if (lblParcels.Any())
                {
                    foreach (var lbl in lblParcels.Values)
                    {
                        DestroyImmediate(lbl.gameObject);
                    }
                }
#endif

                Distribution = gridDistribution.Generate(Selection.Value);

#if DEBUG
                foreach (var parcel in Distribution.Parcels)
                {
                    var lbl = GameObjectUtils.AddUIComponent<GUIUtils.UITextDebug>();
                    lbl.Hide();
                    lbl.SetText(Convert.ToString((int)parcel.GridId));
                    lblParcels[parcel] = lbl;
                }
#endif

                m_optionPanel.EnableTab(2);
            }
        }

        /// <inheritdoc/>
        public void SetLineDistribution()
        {
            if (Selection.HasValue && SelectionAngle.HasValue)
            {
                Distribution = lineDistribution.Generate(Selection.Value);
            }
        }

        /// <inheritdoc/>
        public void SetForestalDistribution()
        {
            if (Selection.HasValue && SelectionAngle.HasValue)
            {
                Distribution = forestalDistribution.Generate(Selection.Value);
            }
        }

        /// <inheritdoc/>
        public void AddPreference(ushort gridId, BuildingInfo building)
        {
            var parcel = Distribution.FindById(gridId);
            Preferences.Add(new Parcel
            {
                GridId = gridId,
                Building = building,
                Position = parcel.Position,
                Rotation = parcel.Rotation
            });

            OnChangeSelectedIndex(null, m_optionPanel.selectedIndex);
        }

        public void AddRestriction(ushort gridId, BuildingInfo building)
        {
            var parcel = Distribution.FindById(gridId);
            Restrictions.Add(new Parcel
            {
                GridId = gridId,
                Building = building,
                Position = parcel.Position,
                Rotation = parcel.Rotation
            });

            OnChangeSelectedIndex(null, m_optionPanel.selectedIndex);
        }

        public void RemoveBuilding(Vector3 mousePosition)
        {
            var cell = Utils.MathUtils.FindNeighbour(Preferences, mousePosition, 20);
            if (cell != null)
            {
                LoggerUtils.Log("Removed", Preferences.Remove(cell));
            }
        }

        public void CancelGeneration()
        {
            m_optionPanel.selectedIndex = 1;
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
            m_toolbarButton = mainToolbar.AddTab(LibraryConstants.ObjectPrefix + ".ToolButton", mainButtonGameObject, subPanelGameObject, new Type[] { typeof(UIGroupPanel) }) as UIButton;

            m_toolbarButton.atlas = IconAtlas;
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
        /// Setup the tutorial panel
        /// </summary>
        private void SetupTutorialPanel()
        {
            Locale locale = (Locale)typeof(LocaleManager).GetField("m_Locale", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(LocaleManager.instance);
            Locale.Key key = new Locale.Key
            {
                m_Identifier = "TUTORIAL_ADVISER_TITLE",
                m_Key = m_toolbarButton.name
            };
            if (!locale.Exists(key))
            {
                locale.AddLocalizedString(key, ModInfo.ModName);
            }
            key = new Locale.Key
            {
                m_Identifier = "TUTORIAL_ADVISER",
                m_Key = m_toolbarButton.name
            };
            if (!locale.Exists(key))
            {
                locale.AddLocalizedString(key, "Work in progress...");
            }
        }

        /// <summary>
        /// Creates the option panel
        /// </summary>
        private void SetupOptionPanel()
        {
            m_optionPanel = GameObjectUtils.AddUIComponent<UIOptionPanel>();
            m_optionPanel.relativePosition = new Vector2(474 - m_optionPanel.width, 949);
            m_optionPanel.selectedIndex = 0;
            m_optionPanel.eventSelectedIndexChanged += OnChangeSelectedIndex;
            m_optionPanel.eventMouseEnter += OnMouseEnterOptionPanel;
            m_optionPanel.eventMouseLeave += OnMouseLeaveOptionPanel;
        }

        /// <summary>
        /// Creates all the scrollable panels
        /// </summary>
        private void SetupScrollablePanels()
        {
            // Gets the main toolbar
            var mainToolbar = ToolsModifierControl.mainToolbar.component as UITabstrip;

            m_defaultXPos = mainToolbar.relativePosition.x;
            UpdateMainToolbar();

            // Gets the main group panel
            var groupPanel = mainToolbar.GetComponentInContainer(m_toolbarButton, typeof(UIGroupPanel)) as UIGroupPanel;
            groupPanel.name = UIGroupPanel.Name;
            groupPanel.enabled = true;
            groupPanel.component.isInteractive = true;
            groupPanel.m_OptionsBar = m_optionPanel;
            groupPanel.m_DefaultInfoTooltipAtlas = ToolsModifierControl.mainToolbar.m_DefaultInfoTooltipAtlas;
            if (ToolsModifierControl.mainToolbar.enabled)
            {
                groupPanel.RefreshPanel();
            }

            // Gets scrollable toolbar
            var oldPanel = groupPanel.GetComponentInChildren<UIScrollablePanel>();
            m_scrollablePanel = UIDistributionOptionPanel.Create(oldPanel);
            m_scrollablePanel.Disable();
            m_scrollablePanel.eventVisibilityChanged += OnChangeVisibilityPanel;
            m_scrollablePanel.eventMouseEnter += OnMouseEnterScrollablePanel;
            m_scrollablePanel.eventMouseLeave += OnMouseLeaveScrollablePanel;
            m_scrollablePanel.eventClicked += OnItemClickedDistributionPanel;

            // Sets the main tabs
            m_categoryPanel = GameObjectUtils.AddUIComponent<UICategoryOptionPanel>();
            m_categoryPanel.relativePosition = new Vector3(604, 854);
            m_categoryPanel.selectedIndex = 0;
            m_categoryPanel.Hide();
            m_categoryPanel.eventSelectedIndexChanged += OnChangeTabIndex;
            m_categoryPanel.eventMouseEnter += OnMouseEnterOptionPanel;
            m_categoryPanel.eventMouseLeave += OnMouseLeaveOptionPanel;

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
            m_buildingAction = new BuildingAction();
            m_buildingAction.OnStart(this);
        }

        /// <summary>
        /// Creates the build distributions
        /// </summary>
        private void SetupDistributions()
        {
            gridDistribution = new GridDistributionThread();
            //lineDistribution = new LineDistribution();
            //mineDistribution = new MineDistribution();
        }

        /// <summary>
        /// Invoked when the main tool is updated
        /// </summary>
        private void UpdateMainToolbar()
        {
            UITabstrip tabstrip = ToolsModifierControl.mainToolbar.component as UITabstrip;
            if (tabstrip == null) return;

            tabstrip.eventComponentAdded -= new ChildComponentEventHandler(UpdatePosition);
            tabstrip.eventComponentRemoved -= new ChildComponentEventHandler(UpdatePosition);

            tabstrip.eventComponentAdded += new ChildComponentEventHandler(UpdatePosition);
            tabstrip.eventComponentRemoved += new ChildComponentEventHandler(UpdatePosition);
            UpdatePosition(tabstrip, null);
        }

        /// <summary>
        /// Invoked when the main tool updates their position
        /// </summary>
        /// <param name="c"></param>
        /// <param name="p"></param>
        private void UpdatePosition(UIComponent c, UIComponent p)
        {
            UITabstrip tabstrip = c as UITabstrip;

            float width = 0;
            foreach (UIComponent child in tabstrip.tabs)
            {
                width += child.width;
            }

            float newXPos = (tabstrip.parent.width - width) / 2;
            tabstrip.relativePosition = new Vector3(Mathf.Min(m_defaultXPos, newXPos), tabstrip.relativePosition.y);
        }

        /// <summary>
        /// Invoked when the main tool updates the scrollable panel
        /// </summary>
        private void UpdateScrollablePanel()
        {
            if (m_categoryPanel.selectedIndex == 0)
            {
                switch (Distribution.Type)
                {
                    case DistributionType.GRID:
                        var panel = m_scrollablePanel as UIDistributionOptionPanel;
                        panel.selectedItem = panel.itemsData[0];
                        panel.Refresh();
                        break;
                }
            }
        }

        /// <summary>
        /// Invoked when the mouse is hover the toolbar
        /// </summary>
        /// <param name="component"></param>
        /// <param name="eventParam"></param>
        private void OnMouseEnterToolbar(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_mouseHoverToolbar = true;
        }

        /// <summary>
        /// Invoked when the mouse leaves the toolbar
        /// </summary>
        /// <param name="component"></param>
        /// <param name="eventParam"></param>
        private void OnMouseLeaveToolbar(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_mouseHoverToolbar = false;
        }

        /// <summary>
        /// Invoked when the mouse is hover the panel options
        /// </summary>
        /// <param name="component"></param>
        /// <param name="eventParam"></param>
        private void OnMouseEnterOptionPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_mouseHoverOptionPanel = true;
        }

        /// <summary>
        /// Invoked when the mouse leaves the panel options
        /// </summary>
        /// <param name="component"></param>
        /// <param name="eventParam"></param>
        private void OnMouseLeaveOptionPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_mouseHoverOptionPanel = false;
        }

        /// <summary>
        /// Invoked when the mouse is hover the scrollable panel
        /// </summary>
        /// <param name="component"></param>
        /// <param name="eventParam"></param>
        private void OnMouseEnterScrollablePanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_mouseHoverScrollablePanel = true;
        }

        /// <summary>
        /// Invoked when the mouse leaves the scrollable panel
        /// </summary>
        /// <param name="component"></param>
        /// <param name="eventParam"></param>
        private void OnMouseLeaveScrollablePanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_mouseHoverScrollablePanel = false;
        }

        /// <summary>
        /// Invoked when the user clicks on the distributions panel
        /// </summary>
        /// <param name="component"></param>
        /// <param name="eventParam"></param>
        private void OnItemClickedDistributionPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            UIButton item = eventParam.source as UIButton;
            if (item != null)
            {
                //PrefabInfo prefab = item.objectUserData as PrefabInfo;
                var type = (UIDistributionItem.ItemType)item.objectUserData;
                switch (type)
                {
                    case UIDistributionItem.ItemType.Grid:
                        SetGridDistribution();
                        break;
                    case UIDistributionItem.ItemType.Line:
                        //SetLineDistribution();
                        break;
                    case UIDistributionItem.ItemType.Forestal:
                        //SetForestalDistribution();
                        break;
                }
                m_categoryPanel.EnableTab(1);
                m_categoryPanel.EnableTab(2);
            }
        }

        /// <summary>
        /// Invoked when the user clicks on the building panel to insert preferences
        /// </summary>
        /// <param name="component"></param>
        /// <param name="eventParam"></param>
        private void OnItemClickedPreferenceBuildingPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            UIButton item = eventParam.source as UIButton;
            if (item != null)
            {
                PrefabInfo prefab = item.objectUserData as PrefabInfo;
                LoggerUtils.Log($"Clicked on {prefab.name} preference");

                m_optionPanel.selectedIndex = -1;
                m_action?.OnLeftController();
                var oldAction = m_action;
                m_action = new AddBuildingAction(prefab as BuildingInfo, AddBuildingAction.ActionType.Preference);
                m_action.OnStart(this);
                m_action.OnChangeController(oldAction);
                m_action.OnEnterController();
            }
        }

        /// <summary>
        /// Invoked when the user clicks on the building panel to insert restrictions
        /// </summary>
        /// <param name="component"></param>
        /// <param name="eventParam"></param>
        private void OnItemClickedRestrictionBuildingPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            UIButton item = eventParam.source as UIButton;
            if (item != null)
            {
                PrefabInfo prefab = item.objectUserData as PrefabInfo;
                LoggerUtils.Log($"Clicked on {prefab.name} restriction");

                m_optionPanel.selectedIndex = -1;
                m_action?.OnLeftController();
                var oldAction = m_action;
                m_action = new AddBuildingAction(prefab as BuildingInfo, AddBuildingAction.ActionType.Restriction);
                m_action.OnStart(this);
                m_action.OnChangeController(oldAction);
                m_action.OnEnterController();
            }
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
        /// Invoked when CS changes the visibility of the options panel
        /// </summary>
        /// <param name="component"></param>
        /// <param name="visibility"></param>
        private void OnChangeVisibilityPanel(UIComponent component, bool visibility)
        {
            if (visibility) m_categoryPanel.Show(); else m_categoryPanel.Hide();
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
                case 2:
                    m_action = m_buildingAction;
                    break;
                default:
                    m_action = null;
                    break;
            }

            m_action?.OnChangeController(oldAction);
            m_action?.OnEnterController();
        }

        /// <summary>
        /// Invoked when the user clicks in a option button
        /// </summary>
        /// <param name="component"></param>
        /// <param name="tabIndex"></param>
        private void OnChangeTabIndex(UIComponent component, int tabIndex)
        {
            switch (tabIndex)
            {
                case 0:
                    m_scrollablePanel = UIDistributionOptionPanel.Create(m_scrollablePanel);
                    m_scrollablePanel.eventVisibilityChanged += OnChangeVisibilityPanel;
                    m_scrollablePanel.eventMouseEnter += OnMouseEnterScrollablePanel;
                    m_scrollablePanel.eventMouseLeave += OnMouseLeaveScrollablePanel;
                    m_scrollablePanel.eventClicked += OnItemClickedDistributionPanel;
                    break;
                case 1:
                    m_scrollablePanel = UIBuildingsOptionPanel.Create(m_scrollablePanel);
                    m_scrollablePanel.eventVisibilityChanged += OnChangeVisibilityPanel;
                    m_scrollablePanel.eventMouseEnter += OnMouseEnterScrollablePanel;
                    m_scrollablePanel.eventMouseLeave += OnMouseLeaveScrollablePanel;
                    m_scrollablePanel.eventClicked += OnItemClickedPreferenceBuildingPanel;
                    break;
                case 2:
                    m_scrollablePanel = UIBuildingsOptionPanel.Create(m_scrollablePanel);
                    m_scrollablePanel.eventVisibilityChanged += OnChangeVisibilityPanel;
                    m_scrollablePanel.eventMouseEnter += OnMouseEnterScrollablePanel;
                    m_scrollablePanel.eventMouseLeave += OnMouseLeaveScrollablePanel;
                    m_scrollablePanel.eventClicked += OnItemClickedRestrictionBuildingPanel;
                    break;
            }

            UpdateScrollablePanel();
        }

        private bool IsPointerOverUIView()
        {
            var dialogPanel = (m_buildingAction as BuildingAction).DialogPanel;

            return m_mouseHoverOptionPanel || 
                m_mouseHoverScrollablePanel || 
                m_mouseHoverToolbar || 
                (dialogPanel != null && dialogPanel.isVisible && dialogPanel.containsMouse);
        }

    #endregion
    }
}
