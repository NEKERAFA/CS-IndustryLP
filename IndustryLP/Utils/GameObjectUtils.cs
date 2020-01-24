using ColossalFramework.UI;
using UnityEngine;

namespace IndustryLP.Utils
{
    static class GameObjectUtils
    {
        #region GameObject Utils

        /// <summary>
        /// Gets all old <see cref="MainTool"/> panels and destroy them
        /// </summary>
        public static void DestroyOldPanels()
        {
            var panels = Object.FindObjectsOfType<MainTool>();
            foreach (var panel in panels)
            {
                Object.Destroy(panel);
            }
        }

        /// <summary>
        /// Creates a new object with a specific <see cref="Component"/> and converts it to type T
        /// </summary>
        /// <typeparam name="T">A <see cref="Component"/> type</typeparam>
        /// <returns></returns>
        public static T AddObjectWithComponent<T>() where T : Component
        {
            return (T)AddObjectWithComponent(typeof(T));
        }

        /// <summary>
        /// Creates a new object with a specific component
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static Component AddObjectWithComponent(System.Type componentType)
        {
            var obj = new GameObject();
            return obj.AddComponent(componentType);
        }

        #endregion

        #region UIView extensions

        /// <summary>
        /// Attachs a specific <see cref="UIComponent"/> to a <see cref="UIView"/> object and convets it to type T
        /// </summary>
        /// <typeparam name="T">A <see cref="UIComponent"/> type</typeparam>
        /// <param name="view"></param>
        /// <returns></returns>
        public static T AddUIComponent<T>(this UIView view) where T : UIComponent
        {
            return (T)view.AddUIComponent(typeof(T));
        }

        #endregion
    }
}
