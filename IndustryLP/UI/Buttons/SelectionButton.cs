using IndustryLP.Constants;

namespace IndustryLP.UI.Buttons
{
    /// <summary>
    /// Creates a clickable button that represents the selection tool
    /// </summary>
    internal class SelectionButton : ToolButton
    {
        #region Propeties

        public static string ObjectName => $"{LibraryConstants.UIPrefix}_SelectionButton";

        #endregion

        #region Button Behaviour

        public override void Start()
        {
            base.Start();
            name = ObjectName;
            normalFgSprite = ResourceConstants.SelectionIcon;
        }

        #endregion
    }
}
