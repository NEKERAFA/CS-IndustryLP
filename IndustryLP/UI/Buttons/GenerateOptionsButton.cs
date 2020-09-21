using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    /// <summary>
    /// Creates a clickable button that represents the generator options tool
    /// </summary>
    internal class GenerateOptionsButton : ToolButton
    {
        #region Properties

        /// <summary>
        /// The name of the button
        /// </summary>
        public static string ObjectName => $"{LibraryConstants.UIPrefix}_OptionsButton";

        #endregion

        #region Button Behaviour

        /// <inheritdoc/>
        public override void Start()
        {
            Start(false);
            name = ObjectName;
            normalFgSprite = ResourceConstants.OptionsIcon;
            disabledColor = ColorConstants.DisableColor;
        }

        #endregion
    }
}
