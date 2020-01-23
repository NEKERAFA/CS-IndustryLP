using UnityEngine;
using ColossalFramework.UI;
using IndustryLP.Constants;
using IndustryLP.Common;
using IndustryLP.UI;
using IndustryLP.Enums;

namespace IndustryLP
{
    public class IndustryLPTool : ToolBase
    {
        private UIToolWindow m_mainView = null;

        #region Properties

        internal static readonly string ObjectName = LibraryConstants.ObjectPrefix + "_ToolWindow";
        
        internal static UITextureAtlas CustomAtlas { get; private set; } = null;

        internal static ToolType CurrentTool = ToolType.None;

        #endregion

        #region Unity Script

        /// <summary>
        /// Invoked when the tool is created
        /// </summary>
        protected override void Awake()
        {
            name = ObjectName;
            
            DebugLogger.Debug("Loading IndustryLP");

            if (m_mainView == null)
            {
                DebugLogger.Debug("Creating window");

                var view = UIView.GetAView();
                m_mainView = view.AddUIComponent(typeof(UIToolWindow)) as UIToolWindow;
                m_mainView.transform.parent = view.transform;
                m_mainView.transform.localPosition = Vector3.zero;
                m_mainView.relativePosition = new Vector3(80f, 10f);
            }

            if (CustomAtlas == null)
            {
                DebugLogger.Debug("Loading custom atlas");

                string[] sprites =
                {
                    ResourceConstants.SelectionIcon,
                    ResourceConstants.ButtonHover,
                    ResourceConstants.ButtonNormal,
                    ResourceConstants.ButtonFocused
                };

                CustomAtlas = ResourceLoader.CreateTextureAtlas(LibraryConstants.AtlasName, sprites, ResourceConstants.IconPath);
            }

            DebugLogger.Debug("Finish");
        }

        /// <summary>
        /// Invoked when the tool is going to remove of the scene
        /// </summary>
        protected override void OnDestroy()
        {
            if (m_mainView != null)
            {
                Destroy(m_mainView);
                m_mainView = null;
            }

            base.OnDestroy();
        }

        /// <summary>
        /// Invoked when the tool is going to render over the scene
        /// </summary>
        /// <param name="cameraInfo"></param>
        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
        }

        #endregion
    }
}
