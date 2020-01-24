using IndustryLP.Constants;

namespace IndustryLP.UI.Buttons
{
    /// <summary>
    /// Creates a clickable button that represents the selection tool
    /// </summary>
    class SelectionButton : ToolButton
    {
        #region Propeties

        public static readonly string ObjectName = $"{LibraryConstants.UIPrefix}_FactoryButton";

        #endregion

        #region Button Behaviour

        public override void Awake()
        {
            base.Awake();
            name = ObjectName;
            normalFgSprite = ResourceConstants.SelectionIcon;
        }

        #endregion
    }
}
