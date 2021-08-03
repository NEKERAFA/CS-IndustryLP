using ColossalFramework.UI;
using IndustryLP.UI.Tabs;
using IndustryLP.Utils;
using UnityEngine;

namespace IndustryLP.UI.Panels
{
    class UICategoryOptionPanel : UITabstrip
    {
        #region Unity Behaviour Methods

        public override void Awake()
        {
            base.Awake();
            padding = new RectOffset(2, 2, 0, 0);

            SetupTabs();
        }

        #endregion

        #region Public methods

        private void SetupTabs()
        {
            var distributionTabTemplate = GameObjectUtils.AddObjectWithComponent<UIDistributionTab>();
            var distributionTab = AddTab(UIDistributionTab.Name, distributionTabTemplate, false);
            SetTab(distributionTab, "Distributions");

            var preferenceTabTemplate = GameObjectUtils.AddObjectWithComponent<UIPreferenceTab>();
            var preferenceTab = AddTab(UIPreferenceTab.Name, preferenceTabTemplate, false);
            SetTab(preferenceTab, "Add preferences");

            var restrictionTemplateTab = GameObjectUtils.AddObjectWithComponent<UIRestrictionTab>();
            var restrictionTemplate = AddTab(UIRestrictionTab.Name, restrictionTemplateTab, false);
            SetTab(restrictionTemplate, "Add restrictions");
        }

        private void SetTab(UIButton button, string tooltipMessage)
        {
            button.playAudioEvents = true;
            button.size = new Vector2(58, 25);
            button.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            button.horizontalAlignment = UIHorizontalAlignment.Center;
            button.verticalAlignment = UIVerticalAlignment.Middle;
            button.tooltip = tooltipMessage;
            button.Disable();
        }

        #endregion
    }
}
