using UnityEngine;
using ColossalFramework.UI;
using IndustryLP.Constants;
using IndustryLP.UI.Buttons;

namespace IndustryLP.UI
{
    /// <summary>
    /// Defines the main toolbar of the mod
    /// </summary>
    public class UIToolWindow : UIPanel
    {
        private UILabel title = null;

        #region Properties

        public static readonly string ObjectName = LibraryConstants.UIPrefix + "_ToolWindow";

        #endregion

        #region Panel

        /// <summary>
        /// Invoked when the panel is created
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Set the toolbar values
            name = ObjectName;
            backgroundSprite = "SubcategoriesPanel";
            size = new Vector2(200, 64);
            opacity = 0.8f;

            // Set the title
            SetupTitle();

            // Defines a drag handler of the toolbar
            var dragHandler = AddUIComponent<UIDragHandle>();
            dragHandler.transform.parent = transform;
            dragHandler.transform.localPosition = Vector3.zero;
            dragHandler.target = this;
            dragHandler.size = size;

            // set the tool buttons
            SetupTools();
        }

        /// <summary>
        /// Creates a label as the title
        /// </summary>
        private void SetupTitle()
        {
            title = AddUIComponent<UILabel>();
            title.transform.parent = transform;
            title.transform.localPosition = Vector3.zero;
            title.relativePosition = new Vector2(5f, 5f);
            title.text = ModInfo.ModName;
        }
        
        /// <summary>
        /// Creates the tool buttons
        /// </summary>
        private void SetupTools()
        {
            var buttonFactory = AddUIComponent<UISelectionButton>();
            buttonFactory.transform.parent = transform;
            buttonFactory.transform.localPosition = Vector3.zero;
            buttonFactory.relativePosition = new Vector3(5f, title.height+5f);
        }

        /// <summary>
        /// Invokes when the tool is going to destroy
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();

            title = null;
        }

        #endregion Panel
    }
}
