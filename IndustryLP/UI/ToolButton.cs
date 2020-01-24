using IndustryLP.Constants;
using IndustryLP.Utils;
using ColossalFramework.UI;
using UnityEngine;

namespace IndustryLP.UI
{
    /// <summary>
    /// Creates a clickable tool button
    /// </summary>
    class ToolButton : UIButton
    {
        #region Properties
        
        /// <summary>
        /// Checks if the button is pushed or not
        /// </summary>
        public bool isPushed { get; private set; }

        #endregion

        #region Button Behaviour

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            isPushed = false;
            size = new Vector2(32, 32);
            atlas = ResourceUtils.GetAtlas(LibraryConstants.AtlasName);
            normalBgSprite = ResourceConstants.ButtonNormal;
            hoveredBgSprite = ResourceConstants.ButtonHover;
        }

        /// <summary>
        /// OnClick is called when the button is clicked
        /// </summary>
        /// <param name="p">Event with information about mouse</param>
        protected override void OnClick(UIMouseEventParameter p)
        {
            //base.OnClick(p);

            if (p.buttons.IsFlagSet(UIMouseButton.Left))
            {
                isPushed = !isPushed;

                if (isPushed)
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

        #endregion
    }
}
