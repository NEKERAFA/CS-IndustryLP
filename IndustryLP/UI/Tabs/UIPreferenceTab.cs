using ColossalFramework.UI;
using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Tabs
{
    internal class UIPreferenceTab: UITabButton
    {
        public static string Name => $"{LibraryConstants.ObjectPrefix}.PreferenceTab";

        #region Mono Behaviour

        public override void Awake()
        {
            base.Awake();

            normalFgSprite = ResourceConstants.SubBarPreferenceNormal;
            focusedFgSprite = ResourceConstants.SubBarPreferenceFocused;
            hoveredFgSprite = ResourceConstants.SubBarPreferenceHovered;
            pressedFgSprite = ResourceConstants.SubBarPreferencePressed;
            disabledFgSprite = ResourceConstants.SubBarPreferenceDisabled;
            name = Name;
        }

        #endregion
    }
}
