using UnityEngine;
using ColossalFramework.UI;
using IndustryLP.Constants;
using IndustryLP.UI.Buttons;

namespace IndustryLP.UI
{
    public class UIToolWindow : UIPanel
    {
        public static readonly string ObjectName = LibraryConstants.UIPrefix + "_ToolWindow";

        /// <summary>
        /// Invoked when the panel is created
        /// </summary>
        public override void Start()
        {
            base.Start();

            name = ObjectName;

            LoadBackground();
            LoadTools();
        }

        private void LoadBackground()
        {
            backgroundSprite = "SubcategoriesPanel";
            isVisible = true;
            width = 40f;
            height = 40f;

            var dragHandler = AddUIComponent<UIDragHandle>();
            dragHandler.size = size;
            dragHandler.BringToFront();
        }

        private void LoadTools()
        {
            var region = AddUIComponent<UIFactoryButton>();
        }
    }
}
