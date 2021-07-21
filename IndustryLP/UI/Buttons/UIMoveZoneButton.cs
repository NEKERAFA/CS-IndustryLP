using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    internal class UIMoveZoneButton : UIOptionButton
    {
        #region Properties

        public static string Name => LibraryConstants.ObjectPrefix + ".MoveZoneOptionButton";

        #endregion

        #region Unity Behaviour Methods

        public override void Awake()
        {
            base.Awake();
            normalFgSprite = ResourceConstants.OptionMove;
            name = Name;
        }

        #endregion
    }
}
