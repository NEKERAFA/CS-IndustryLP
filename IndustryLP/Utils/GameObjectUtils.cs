using ColossalFramework.UI;
using UnityEngine;

namespace IndustryLP.Utils
{
    internal static class GameObjectUtils
    {
        #region GameObject Extensions

        /// <summary>
        /// Creates a new object with a specific <see cref="Component"/> and converts it to type T
        /// </summary>
        /// <typeparam name="T">A <see cref="Component"/> type</typeparam>
        /// <returns>The new <see cref="Component"/> object</returns>
        public static T AddObjectWithComponent<T>() where T : Component
        {
            return (T)AddObjectWithComponent(typeof(T));
        }

        /// <summary>
        /// Creates a new object with a specific component
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns>The new <see cref="Component"/> object</returns>
        public static Component AddObjectWithComponent(System.Type componentType)
        {
            var obj = new GameObject();
            return obj.AddComponent(componentType);
        }

        #endregion

        #region UIView extensions

        /// <summary>
        /// Attachs a specific <see cref="UIComponent"/> to a <see cref="UIView"/> object and converts it to type T
        /// </summary>
        /// <typeparam name="T">A <see cref="UIComponent"/> type</typeparam>
        /// <param name="view"></param>
        /// <returns>The new <see cref="UIComponent"/> object</returns>
        public static T AddUIComponent<T>(this UIView view) where T : UIComponent
        {
            return (T)view.AddUIComponent(typeof(T));
        }

        /// <summary>
        /// Attachs a specific <see cref="UIComponent"/> to the parent <see cref="UIView"/> object and puts in the <c>(0, 0)</c> of the parent
        /// </summary>
        /// <typeparam name="T">A <see cref="UIComponent"/> type</typeparam>
        /// <returns>The new <see cref="UIComponent"/> object</returns>
        public static T AddUIComponent<T>() where T : UIComponent
        {
            var view = UIView.GetAView();
            T uiComponent = view.AddUIComponent<T>();
            uiComponent.transform.parent = view.transform;
            uiComponent.transform.localPosition = Vector2.zero;
            return uiComponent;
        }

        #endregion
    }
}
