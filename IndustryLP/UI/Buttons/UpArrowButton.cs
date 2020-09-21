using IndustryLP.Utils;
using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    /// <summary>
    /// Creates a clickable button that represents a up arrow
    /// </summary>
    internal class UpArrowButton : ToolButton
    {
        #region Properties

        /// <summary>
        /// The name of the button
        /// </summary>
        public static string ObjectName => $"{LibraryConstants.UIPrefix}_UpArrowButton";

        #endregion

        #region Button Behaviour

        /// <inheritdoc/>
        public override void Start()
        {
            Start(false);
            name = ObjectName;
            normalFgSprite = ResourceConstants.UpArrowIcon;
            disabledColor = ColorConstants.DisableColor;

            LoggerUtils.Log($"Created {name}");
        }

        #endregion
    }
}
