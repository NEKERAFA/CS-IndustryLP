using ColossalFramework.UI;
using IndustryLP.UI.Buttons;
using IndustryLP.Utils;

namespace IndustryLP.UI.Panels
{
    internal class UIOptionPanel : UITabstrip
    {
        #region Unity Behaviour Methods

        public override void Awake()
        {
            base.Awake();

            SetupOptionButtons();
        }

        #endregion

        #region Private methods

        private void SetupOptionButtons()
        {
            // Zoning option
            var zoningTemplate = GameObjectUtils.AddObjectWithComponent<UIZoningButton>();
            AddTab(UIZoningButton.Name, zoningTemplate, false);

            // Zone movement option
            var moveZoneTemplate = GameObjectUtils.AddObjectWithComponent<UIMoveZoneButton>();
            AddTab(UIMoveZoneButton.Name, moveZoneTemplate, false);

            // Zoning option
            var buildZoneTemplate = GameObjectUtils.AddObjectWithComponent<UIBuildZoneButton>();
            AddTab(UIBuildZoneButton.Name, buildZoneTemplate, false);

            // Set disabled buttons
            DisableTab(1);
            DisableTab(2);

            // Set width
            width = padding.left + zoningTemplate.width + padding.right + padding.left + moveZoneTemplate.width + padding.right + padding.left + buildZoneTemplate.width + padding.right;
        }

        #endregion
    }
}
