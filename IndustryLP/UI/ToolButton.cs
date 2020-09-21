using ColossalFramework.UI;
using IndustryLP.Utils.Constants;
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
        /// <param name="isChecked"><c>true</c> if the button is pushed on, <c>false</c> otherwise. <c>null</c> if the button is not switched</param>
        public delegate void OnButtonClickedDelegate(bool isChecked);

        #region Attributes

        private bool m_isChecked;

        #endregion

        #region Properties

        /// <summary>
        /// Invoked when the button is pressed.
        /// </summary>
        public OnButtonClickedDelegate OnButtonClicked { get; set; }

        /// <summary>
        /// The button is setted as checkbox
        /// </summary>
        public bool AsCheckbox { get; set; }

        /// <summary>
        /// Checks if the button is pushed or not.
        /// </summary>
        internal bool IsChecked
        {
            get => m_isChecked;
            set
            {
                m_isChecked = value;

                if (AsCheckbox)
                {
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
                else
                {
                    normalBgSprite = ResourceConstants.ButtonNormal;
                    hoveredBgSprite = ResourceConstants.ButtonHover;
                    pressedBgSprite = ResourceConstants.ButtonPushed;
                }
            }
        }

        #endregion

        #region Button Behaviour

        /// <summary>
        /// Start is called when the script instance is being loaded.
        /// </summary>
        public void Start(bool asCheckbox)
        {
            base.Start();
            size = new Vector2(32, 32);
            atlas = ResourceLoader.GetAtlas(ResourceConstants.AtlasName);
            AsCheckbox = asCheckbox;
            IsChecked = false;
        }

        /// <summary>
        /// OnClick is called when the button is clicked
        /// </summary>
        /// <param name="p">Event with information about mouse</param>
        protected override void OnClick(UIMouseEventParameter p)
        {
            base.OnClick(p);

            if (p.buttons.IsFlagSet(UIMouseButton.Left))
            {
                if (AsCheckbox) IsChecked = !IsChecked;
                OnButtonClicked?.Invoke(IsChecked);
            }
        }

        #endregion
    }
}
