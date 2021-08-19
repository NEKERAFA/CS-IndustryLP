using ColossalFramework.UI;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    internal class UIArrowBuildPanelButton : UIButton
    {
        public enum Direction { Up, Down }

        #region Public methods

        public void Initialize(Direction direction)
        {
            atlas = ResourceLoader.GetAtlas("Ingame");
            name = $"{LibraryConstants.UIPrefix}_Solution{direction}";
            horizontalAlignment = UIHorizontalAlignment.Center;
            verticalAlignment = UIVerticalAlignment.Middle;
            normalFgSprite = $"Solution{direction}";
            disabledColor = ColorConstants.DisableColor;
            isEnabled = false;
        }

        #endregion
    }
}
