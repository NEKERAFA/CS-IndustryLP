using ColossalFramework.UI;
using IndustryLP.Utils.Constants;

namespace IndustryLP.UI
{
    internal abstract class UIOptionButton : UIButton
    {
        #region Unity Behaviour Methods

        public override void Awake()
        {
            base.Awake();

            atlas = IndustryTool.IconAtlas;
            playAudioEvents = true;

            // Set background sprite
            normalBgSprite = ResourceConstants.OptionFgNormal;
            focusedBgSprite = ResourceConstants.OptionFgFocused;
            hoveredBgSprite = ResourceConstants.OptionFgHovered;
            pressedBgSprite = ResourceConstants.OptionFgPressed;
            disabledBgSprite = ResourceConstants.OptionFgDisabled;
        }

        #endregion
    }
}
