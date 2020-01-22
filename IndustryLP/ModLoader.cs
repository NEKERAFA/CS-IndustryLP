using ICities;
using UnityEngine;

namespace IndustryLP
{
    public class ModLoader : ILoadingExtension
    {
        #region Properties

        private bool IsGameplayMode = false;

        #endregion

        private IndustryLPTool tool = null;

        #region Loading Extension

        /// <summary>
        /// Invoked when the extension initializes.
        /// </summary>
        /// <param name="loading">Status of loading extensions</param>
        public void OnCreated(ILoading loading)
        {
            IsGameplayMode = loading.currentMode == AppMode.Game;
        }

        /// <summary>
        /// Invoked when a level has completed the loading process.
        /// </summary>
        /// <param name="mode">Defines what kind of level was just loaded</param>
        public void OnLevelLoaded(LoadMode mode)
        {
            // Checks if we are in gameplay mode
            if (IsGameplayMode)
            {
                if (tool == null)
                {
                    var obj = new GameObject();
                    tool = obj.AddComponent<IndustryLPTool>();
                }

                tool.Start();
            }
        }

        /// <summary>
        /// Invoked when the level is unloading (typically when going back to the main menu or prior to loading a new level).
        /// </summary>
        public void OnLevelUnloading()
        {
            if (tool != null)
            {
                Object.Destroy(tool.gameObject);
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
