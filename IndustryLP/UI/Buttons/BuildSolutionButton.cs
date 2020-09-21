using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    /// <summary>
    /// Creates a clickable button that represents the build solution
    /// </summary>
    internal class BuildSolutionButton : ToolButton
    {
        #region Properties

        /// <summary>
        /// The name of the button
        /// </summary>
        public static string ObjectName => $"{LibraryConstants.UIPrefix}_BuildButton";

        #endregion

        #region Button Behaviour

        /// <inheritdoc/>
        public override void Start()
        {
            Start(false);
            name = ObjectName;
            normalFgSprite = ResourceConstants.BuildIcon;
            disabledColor = ColorConstants.DisableColor;
        }

        #endregion
    }
}
