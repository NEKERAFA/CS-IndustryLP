using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    internal class UIArrowBuildPanelButton : UIOptionButton
    {
        public enum ButtonType { Previous, Next }

        #region Public methods

        public void Initialize(ButtonType type)
        {
            name = $"{LibraryConstants.UIPrefix}_Solution{type}";
            normalFgSprite = $"SolutionOption{type}";
            disabledFgSprite = $"SolutionOption{type}Disabled";
        }

        #endregion
    }
}
