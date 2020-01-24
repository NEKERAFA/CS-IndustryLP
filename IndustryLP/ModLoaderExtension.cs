using ICities;
using IndustryLP.Utils;
using UnityEngine;

namespace IndustryLP
{
    public class ModLoaderExtension : ILoadingExtension
    {

        internal static MainTool mainTool { get; private set; } = null;

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
                    // For remove old instances if the assembly is updated (development)
                    GameObjectUtils.DestroyOldPanels();

                    // Creates a new tool controller
                    mainTool = GameObjectUtils.AddObjectWithComponent<MainTool>();
                    break;
            }
        }

        /// <summary>
        /// Invoked when the level is unloading (typically when going back to the main menu or prior to loading a new level).
        /// </summary>
        public void OnLevelUnloading()
        {
            if (mainTool != null)
            {
                Object.Destroy(mainTool.gameObject);
                mainTool = null;
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
