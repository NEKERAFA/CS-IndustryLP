using UnityEngine;
using IndustryLP.Constants;
using IndustryLP.Common;
using IndustryLP.UI;
using ColossalFramework.UI;

namespace IndustryLP
{
    public class IndustryLPTool : MonoBehaviour
    {
        private static UIToolWindow m_mainView = null;

        #region Properties

        public static readonly string ObjectName = LibraryConstants.ObjectPrefix + "_ToolWindow";
        public static UITextureAtlas CustomAtlas { get; private set; } = null;

        #endregion

        #region Unity Script

        /// <summary>
        /// Invoked when the instance script is created
        /// </summary>
        public void Start()
        {
            name = ObjectName;
            
            DebugLogger.Debug("Loading IndustryLP");

            if (m_mainView == null)
            {
                DebugLogger.Debug("Creating window");

                var view = UIView.GetAView();
                m_mainView = view.AddUIComponent(typeof(UIToolWindow)) as UIToolWindow;
            }

            if (CustomAtlas == null)
            {
                DebugLogger.Debug("Loading custom atlas");

                string[] sprites =
                {
                    ResourceConstants.AreaSelectionIcon,
                    ResourceConstants.ButtonHover,
                    ResourceConstants.ButtonNormal
                };

                CustomAtlas = ResourceLoader.CreateTextureAtlas(LibraryConstants.AtlasName, sprites, ResourceConstants.IconPath);
            }

            DebugLogger.Debug("Finish");
        }

        /// <summary>
        /// Invoked when the GameObject is going to remove of the scene
        /// </summary>
        public void OnDestroy()
        {
            if (m_mainView != null)
            {
                Destroy(m_mainView);
                m_mainView = null;
            }
        }

        #endregion
    }
}
