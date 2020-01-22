using UnityEngine;
using IndustryLP.Constants;
using IndustryLP.Common;
using IndustryLP.UI;
using ColossalFramework.UI;

namespace IndustryLP
{
    public class IndustryLPTool : MonoBehaviour
    {
        private UIToolWindow m_mainView;

        public static readonly string ObjectName = LibraryConstants.ObjectPrefix + "_ToolWindow";

        /// <summary>
        /// Invoked when the instance script is created
        /// </summary>
        public void Start()
        {
            name = ObjectName;
            
            DebugLogger.Debug("Loading IndustryLP");

            var view = UIView.GetAView();
            m_mainView = view.AddUIComponent(typeof(UIToolWindow)) as UIToolWindow;

            DebugLogger.Debug("Finish");
        }

        public void OnDestroy()
        {
            if (m_mainView != null)
            {
                Destroy(m_mainView);
            }
        }
    }
}
