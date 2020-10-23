using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    internal class MoveZoneOptionButton : OptionButton
    {
        #region Properties

        public static string Name => LibraryConstants.ObjectPrefix + ".MoveZoneOptionButton";

        #endregion

        #region Unity Behaviour Methods

        public override void Awake()
        {
            base.Awake();
            normalFgSprite = ResourceConstants.OptionMove;
        }

        #endregion
    }
}
