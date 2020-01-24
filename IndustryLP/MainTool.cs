using UnityEngine;
using ColossalFramework.UI;
using IndustryLP.Constants;
using IndustryLP.Utils;
using IndustryLP.UI;
using System.Collections.Generic;

namespace IndustryLP
{
    class MainTool : MonoBehaviour
    {
        private ToolPanel m_mainView = null;

        private UITextureAtlas m_textureAtlas = null;

        #region Properties

        public static readonly string ObjectName = LibraryConstants.ObjectPrefix + "_ToolWindow";

        #endregion

        #region Unity Behaviour

        /// <summary>
        /// Invoked when the tool is created
        /// </summary>
        protected void Awake()
        {
            name = ObjectName;
            
            LoggerUtils.Debug("Loading IndustryLP");

            if (m_textureAtlas == null)
            {
                LoggerUtils.Debug("Loading altas");
                SetupResources();
            }

            if (m_mainView == null)
            {
                LoggerUtils.Debug("Setting tool panel");
                SetupToolPanel();
            }

            LoggerUtils.Debug("Finish");
        }

        /// <summary>
        /// Invoked when the tool is going to remove of the scene
        /// </summary>
        protected void OnDestroy()
        {
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
        }

        #endregion

        #region Tool Behaviour

        private void SetupToolPanel()
        {
            var view = UIView.GetAView();
            m_mainView = view.AddUIComponent<ToolPanel>();
            m_mainView.transform.parent = view.transform;
            m_mainView.transform.localPosition = Vector3.zero;
            m_mainView.relativePosition = new Vector3(80f, 10f);
        }

        private void SetupResources()
        {
            string[] sprites =
            {
                ResourceConstants.SelectionIcon,
                ResourceConstants.ButtonHover,
                ResourceConstants.ButtonNormal,
                ResourceConstants.ButtonPushed
            };

            m_textureAtlas = ResourceUtils.CreateTextureAtlas(LibraryConstants.AtlasName, sprites, ResourceConstants.IconPath);
        }

        #endregion
    }
}
