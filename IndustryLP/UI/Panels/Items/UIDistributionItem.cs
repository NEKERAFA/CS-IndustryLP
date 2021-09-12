using ColossalFramework.UI;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using System;

namespace IndustryLP.UI.Panels.Items
{
    internal class UIDistributionItem : IUIFastListItem<UIDistributionItem.ItemData, UIButton>
    {
        #region Properties

        private ItemData m_currentData { get; set; }
        public UIButton component { get; set; }

        #endregion

        #region Class Data

        public enum ItemType
        {
            Grid, Line, Forestal
        }

        public class ItemData : IItemData
        {
            public string Name { get; set; }
            public string Tooltip { get; set; }
            public UIScrollablePanel Panel { get; set; }
            public ItemType Type { get; set; }
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            component.text = string.Empty;
            component.tooltipAnchor = UITooltipAnchor.Floating;
            component.tabStrip = true;
            component.horizontalAlignment = UIHorizontalAlignment.Center;
            component.verticalAlignment = UIVerticalAlignment.Middle;
            component.pivot = UIPivotPoint.TopCenter;
            component.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            component.group = component.parent;

            UIComponent uIComponent = (component.childCount <= 0) ? null : component.components[0];
            if (uIComponent != null)
            {
                uIComponent.isVisible = false;
            }
        }

        public void Display(ItemData data, int index)
        {
            try
            {
                if (data == null)
                {
                    LoggerUtils.Debug("Data null");
                }

                if (component == null || data?.Name == null)
                {
                    return;
                }

                m_currentData = data;

                component.Unfocus();
                component.name = data.Name;
                component.gameObject.GetComponent<TutorialUITag>().tutorialTag = data.Name;
                component.atlas = IndustryTool.ThumbnailAtlas;
                component.normalFgSprite = ConvertTypeToSprite(data);
                component.hoveredFgSprite = $"{component.normalFgSprite}Hovered";
                component.pressedFgSprite = $"{component.normalFgSprite}Pressed";
                component.focusedFgSprite = $"{component.normalFgSprite}Focused";
                component.disabledFgSprite = ResourceConstants.DistributionDisabled;
                component.isEnabled = true;
                component.tooltip = data.Tooltip;
                component.objectUserData = data.Type;
                component.forceZOrder = index;

                LoggerUtils.Log("Finished", component.name);
            }
            catch (Exception e)
            {
                if (data != null)
                {
                    LoggerUtils.Warning($"Display failed: {data.Name}");
                }
                else
                {
                    LoggerUtils.Warning("Display failed");
                }
                LoggerUtils.Error(e);
            }
        }

        public void Select(int index)
        {
            component.normalFgSprite = $"{ConvertTypeToSprite(m_currentData)}Focused";
            component.hoveredFgSprite = $"{ConvertTypeToSprite(m_currentData)}Focused";
        }

        public void Deselect(int index)
        {
            component.normalFgSprite = $"{ConvertTypeToSprite(m_currentData)}";
            component.hoveredFgSprite = $"{ConvertTypeToSprite(m_currentData)}Hovered";
        }

        #endregion

        #region Private Methods

        private string ConvertTypeToSprite(ItemData data)
        {
            switch (data.Type)
            {
                case ItemType.Grid:
                    return ResourceConstants.GridDistributionNormal;
                case ItemType.Line:
                    return ResourceConstants.LineDistributionNormal;
                case ItemType.Forestal:
                    return ResourceConstants.ForestalDistributionNormal;
            }

            return "";
        }

        #endregion
    }
}
