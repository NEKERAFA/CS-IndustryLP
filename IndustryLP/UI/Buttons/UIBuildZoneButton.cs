using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    internal class UIBuildZoneButton : UIOptionButton
    {
        #region Properties

        public static string Name => LibraryConstants.ObjectPrefix + ".BuildZoneOptionButton";

        #endregion

        #region Unity Behaviour Methods

        public override void Awake()
        {
            base.Awake();
            normalFgSprite = ResourceConstants.OptionBuild;
            name = Name;
        }

        #endregion
    }
}
