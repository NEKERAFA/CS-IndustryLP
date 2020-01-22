using ICities;
using UnityEngine;

namespace IndustryLP
{
    public class ModLoader : ILoadingExtension
    {

        private static IndustryLPTool m_mainTool = null;

        #region Loading Extension

        /// <summary>
        /// Invoked when the extension initializes.
        /// </summary>
        /// <param name="loading">Status of loading extensions</param>
        public void OnCreated(ILoading loading)
        {
        }

        /// <summary>
        /// Invoked when a level has completed the loading process.
        /// </summary>
        /// <param name="mode">Defines what kind of level was just loaded</param>
        public void OnLevelLoaded(LoadMode mode)
        {
            // Checks if we are in gameplay mode
            switch (mode)
            {
                case LoadMode.NewGame:
                case LoadMode.LoadGame:
                    if (m_mainTool != null)
                    {
                        m_mainTool.OnDestroy();
                        Object.Destroy(m_mainTool.gameObject);
                    }

                    var obj = new GameObject();
                    m_mainTool = obj.AddComponent<IndustryLPTool>();
                    m_mainTool.Start();

                    break;
            }
        }

        /// <summary>
        /// Invoked when the level is unloading (typically when going back to the main menu or prior to loading a new level).
        /// </summary>
        public void OnLevelUnloading()
        {
            if (m_mainTool != null)
            {
                m_mainTool.OnDestroy();
                Object.Destroy(m_mainTool.gameObject);
                m_mainTool = null;
            }
        }

        /// <summary>
        /// Invoked when the extension deinitializes.
        /// </summary>
        public void OnReleased()
        {
        }

        #endregion
    }
}
