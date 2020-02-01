using ColossalFramework.UI;
using IndustryLP.Constants;
using IndustryLP.Utils;
using UnityEngine;

namespace IndustryLP.UI
{
    /// <summary>
    /// Creates a clickable tool button
    /// </summary>
    internal class ToolButton : UIButton
    {
        /// <summary>
        /// Delegates that represents the action when the user press the button
        /// </summary>
        /// <param name="isChecked">True if the button is pushed on, false otherwise</param>
        internal delegate void OnButtonPressedDelegate(bool isChecked);

        protected bool m_isChecked;

        #region Properties

        /// <summary>
        /// Checks if the button is pushed or not.
        /// </summary>
        internal bool IsChecked
        { 
            get => m_isChecked;
            set
            {
                m_isChecked = value;

                if (value)
                {
                    normalBgSprite = ResourceConstants.ButtonPushed;
                    hoveredBgSprite = ResourceConstants.ButtonPushed;
                }
                else
                {
                    normalBgSprite = ResourceConstants.ButtonNormal;
                    hoveredBgSprite = ResourceConstants.ButtonHover;
                }
            }
        }

        /// <summary>
        /// Invoked when the button is pressed.
        /// </summary>
        internal OnButtonPressedDelegate OnButtonPressed { get; set; }

        #endregion

        #region Button Behaviour

        /// <summary>
        /// Start is called when the script instance is being loaded.
        /// </summary>
        public override void Start()
        {
            base.Start();
            IsChecked = false;
            size = new Vector2(32, 32);
            atlas = ResourceLoader.GetAtlas(ResourceConstants.AtlasName);
            normalBgSprite = ResourceConstants.ButtonNormal;
            hoveredBgSprite = ResourceConstants.ButtonHover;
        }

        /// <summary>
        /// OnClick is called when the button is clicked
        /// </summary>
        /// <param name="p">Event with information about mouse</param>
        protected override void OnClick(UIMouseEventParameter p)
        {
            if (p.buttons.IsFlagSet(UIMouseButton.Left))
            {
                IsChecked = !IsChecked;
                OnButtonPressed?.Invoke(this.IsChecked);
            }
        }

        #endregion
    }
}
