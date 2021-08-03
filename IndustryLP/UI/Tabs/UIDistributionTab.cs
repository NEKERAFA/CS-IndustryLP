using ColossalFramework.UI;
using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Tabs
{
    internal class UIDistributionTab: UITabButton
    {
        public static string Name => $"{LibraryConstants.ObjectPrefix}.DistributionTab";

        #region Mono Behaviour

        public override void Awake()
        {
            base.Awake();

            normalFgSprite = ResourceConstants.SubBarDistributionNormal;
            focusedFgSprite = ResourceConstants.SubBarDistributionFocused;
            hoveredFgSprite = ResourceConstants.SubBarDistributionHovered;
            pressedFgSprite = ResourceConstants.SubBarDistributionPressed;
            disabledFgSprite = ResourceConstants.SubBarDistributionDisabled;
            name = Name;
        }

        #endregion
    }
}
