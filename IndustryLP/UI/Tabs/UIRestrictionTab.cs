using ColossalFramework.UI;
using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Tabs
{
    internal class UIRestrictionTab: UITabButton
    {
        public static string Name => $"{LibraryConstants.ObjectPrefix}.RestrictionTab";

        #region Mono Behaviour

        public override void Awake()
        {
            base.Awake();

            normalFgSprite = ResourceConstants.SubBarRestrictionNormal;
            focusedFgSprite = ResourceConstants.SubBarRestrictionFocused;
            hoveredFgSprite = ResourceConstants.SubBarRestrictionHovered;
            pressedFgSprite = ResourceConstants.SubBarRestrictionPressed;
            disabledFgSprite = ResourceConstants.SubBarRestrictionDisabled;
            name = Name;
        }

        #endregion
    }
}
