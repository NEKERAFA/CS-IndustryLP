using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    internal class UIBuildSolutionButton : UIOptionButton
    {
        #region Properties

        /// <summary>
        /// The name of the button
        /// </summary>
        public static string ObjectName => $"{LibraryConstants.UIPrefix}_BuildButton";

        #endregion

        #region Button Behaviour

        /// <inheritdoc/>
        public override void Awake()
        {
            base.Awake();
            name = ObjectName;
            normalFgSprite = ResourceConstants.BuildNow;
            disabledFgSprite = ResourceConstants.BuildNowDisabled;
        }

        #endregion
    }
}
