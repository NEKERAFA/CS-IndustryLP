using IndustryLP.Utils.Constants;

namespace IndustryLP.UI.Buttons
{
    /// <summary>
    /// Creates a clickable button that represents the generator tool
    /// </summary>
    internal class GeneratorButton : ToolButton
    {
        #region Properties

        public static string ObjectName => $"{LibraryConstants.UIPrefix}_GeneratorButton";

        #endregion

        #region Button Behaviour

        public override void Start()
        {
            base.Start();
            name = ObjectName;
            normalFgSprite = ResourceConstants.GeneratorIcon;
        }

        #endregion
    }
}
