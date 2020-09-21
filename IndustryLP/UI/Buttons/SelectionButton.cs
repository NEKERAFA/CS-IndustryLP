using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    /// <summary>
    /// Creates a switchable button that represents the selection tool
    /// </summary>
    internal class SelectionButton : ToolButton
    {
        #region Properties

        /// <summary>
        /// The name of the button
        /// </summary>
        public static string ObjectName => $"{LibraryConstants.UIPrefix}_SelectionButton";

        #endregion

        #region Button Behaviour

        public override void Start()
        {
            Start(true);
            name = ObjectName;
            normalFgSprite = ResourceConstants.SelectionIcon;
        }

        #endregion
    }
}
