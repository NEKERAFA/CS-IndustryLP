using ColossalFramework.UI;
using IndustryLP.Utils.Constants;

namespace IndustryLP.UI
{
    internal abstract class OptionButton : UIButton
    {
        #region Unity Behaviour Methods

        public override void Awake()
        {
            base.Awake();

            atlas = IndustryTool.Atlas;
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
