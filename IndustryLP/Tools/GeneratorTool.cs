using IndustryLP.UI;

namespace IndustryLP.Tools
{
    internal class GeneratorTool : ToolActionController
    {
        #region Attributes
        #endregion

        #region Generator Behaviour

        public override void OnChangeController(ToolActionController oldController)
        {
            if (oldController is SelectionTool)
            {

            }
        }

        public override ToolButton CreateButton(ToolButton.OnButtonPressedDelegate callback)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
