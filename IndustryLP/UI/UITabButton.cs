using ColossalFramework.UI;
using IndustryLP.Utils.Constants;
using UnityEngine;

namespace IndustryLP.UI
{
    internal abstract class UITabButton : UIButton
    {
        #region Unity Behaviour

        public override void Awake()
        {
            base.Awake();

            atlas = IndustryTool.IconAtlas;

            normalBgSprite = ResourceConstants.SubBarBackgroundNormal;
            focusedBgSprite = ResourceConstants.SubBarBackgroundFocused;
            hoveredBgSprite = ResourceConstants.SubBarBackgroundHovered;
            pressedBgSprite = ResourceConstants.SubBarBackgroundPressed;
            disabledBgSprite = ResourceConstants.SubBarBackgroundDisabled;
        }

        #endregion
    }
}
