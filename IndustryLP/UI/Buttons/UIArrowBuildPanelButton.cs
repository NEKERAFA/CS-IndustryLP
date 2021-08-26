using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    internal class UIArrowBuildPanelButton : UIOptionButton
    {
        public enum Direction { Up, Down }

        #region Public methods

        public void Initialize(Direction direction)
        {
            name = $"{LibraryConstants.UIPrefix}_Solution{direction}";
            normalFgSprite = $"SolutionOption{direction}";
        }

        #endregion
    }
}
