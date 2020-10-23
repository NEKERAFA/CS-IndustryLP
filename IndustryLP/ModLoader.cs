using ICities;
using IndustryLP.Utils;
using UnityEngine;

namespace IndustryLP
{
    public class ModLoader : ILoadingExtension
    {
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
                    if (IndustryTool.instance != null)
                    {
                        Object.Destroy(IndustryTool.instance);
                    }

                    // Creates a new tool instance
                    IndustryTool.instance = GameObjectUtils.AddObjectWithComponent<IndustryTool>();
                    break;
            }
        }

        /// <summary>
        /// Invoked when the level is unloading (typically when going back to the main menu or prior to loading a new level).
        /// </summary>
        public void OnLevelUnloading()
        {
            if (IndustryTool.instance != null)
            {
                Object.Destroy(IndustryTool.instance);
                IndustryTool.instance = null;
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
