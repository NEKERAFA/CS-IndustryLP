using ICities;
using IndustryLP.Utils;
using UnityEngine;

namespace IndustryLP
{
    public class ModLoader : ILoadingExtension
    {
        #region Attributes

        private static MainTool m_mainTool = null;

        #endregion

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
            // Checks if we are in gameplay mode and creates the main utility
            switch (mode)
            {
                case LoadMode.NewGame:
                case LoadMode.LoadGame:
                    // Removes old attached tool
                    if (m_mainTool != null)
                    {
                        Object.Destroy(m_mainTool.gameObject);
                    }

                    // Creates a new tool controller
                    m_mainTool = GameObjectUtils.AddObjectWithComponent<MainTool>();
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
                Object.Destroy(m_mainTool.gameObject);
                m_mainTool = null;
            }

            // For remove old instances if the assembly is updated (development)
            GameObjectUtils.DestroyOldPanels();
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
