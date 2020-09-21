using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    /// <summary>
    /// Creates a clickable button that represents a down arrow
    /// </summary>
    internal class DownArrowButton : ToolButton
    {
        #region Properties

        /// <summary>
        /// The name of the button
        /// </summary>
        public static string ObjectName => $"{LibraryConstants.UIPrefix}_DownArrowButton";

        #endregion

        #region Button Behaviour

        /// <inheritdoc/>
        public override void Start()
        {
            Start(false);
            name = ObjectName;
            normalFgSprite = ResourceConstants.DownArrowIcon;
            disabledColor = ColorConstants.DisableColor;
        }

        #endregion
    }
}
