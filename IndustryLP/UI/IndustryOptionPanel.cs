using ColossalFramework.UI;
using IndustryLP.UI.Buttons;
using IndustryLP.Utils;

namespace IndustryLP.UI
{
    internal class IndustryOptionPanel : UITabstrip
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
            var zoningTemplate = GameObjectUtils.AddObjectWithComponent<ZoningOptionButton>();
            AddTab(ZoningOptionButton.Name, zoningTemplate, false);

            // Zone movement option
            var moveZoneTemplate = GameObjectUtils.AddObjectWithComponent<MoveZoneOptionButton>();
            AddTab(MoveZoneOptionButton.Name, moveZoneTemplate, false);

            // Zoning option
            var buildZoneTemplate = GameObjectUtils.AddObjectWithComponent<BuildZoneOptionButton>();
            AddTab(BuildZoneOptionButton.Name, buildZoneTemplate, false);

            // Set disabled buttons
            DisableTab(1);
            DisableTab(2);

            // Set width
            width = padding.left + zoningTemplate.width + padding.right + padding.left + moveZoneTemplate.width + padding.right + padding.left + buildZoneTemplate.width + padding.right;
        }

        #endregion
    }
}
