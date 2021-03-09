using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    internal class UIZoningButton : UIOptionButton
    {
        #region Properties

        public static string Name => LibraryConstants.ObjectPrefix + ".ZoningOptionButton";

        #endregion

        #region Unity Behaviour Methods

        public override void Awake()
        {
            base.Awake();
            normalFgSprite = ResourceConstants.OptionZoning;
        }

        #endregion
    }
}
