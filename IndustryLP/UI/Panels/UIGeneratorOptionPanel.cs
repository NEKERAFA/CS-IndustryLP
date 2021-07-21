using ColossalFramework.DataBinding;
using ColossalFramework.UI;
using IndustryLP.UI.Panels.Items;
using IndustryLP.UI.RestrictionButtons;
using IndustryLP.Utils;
using UnityEngine;

namespace IndustryLP.UI.Panels
{
    internal class UIGeneratorOptionPanel : UIFastList<UIGridItem.ItemData, UIGridItem, UIButton>
    {
        public static UIGeneratorOptionPanel Create(UIScrollablePanel oldPanel)
        {
            UIGeneratorOptionPanel panel = oldPanel.parent.AddUIComponent<UIGeneratorOptionPanel>();
            panel.name = oldPanel.name;
            panel.autoLayout = false;
            panel.autoReset = false;
            panel.autoSize = false;
            panel.template = "PlaceableItemTemplate";
            panel.itemWidth = 109f;
            panel.itemHeight = 100f;
            panel.canSelect = true;
            panel.size = new Vector2(763, 100);
            panel.relativePosition = new Vector3(48, 5);
            panel.atlas = oldPanel.atlas;

            panel.parent.parent.eventSizeChanged += (c, p) =>
            {
                if (panel.isVisible)
                {
                    panel.size = new Vector2((int)((p.x - 40f) / panel.itemWidth) * panel.itemWidth, (int)(p.y / panel.itemHeight) * panel.itemHeight);
                    panel.relativePosition = new Vector3(panel.relativePosition.x, Mathf.Floor((p.y - panel.height) / 2));

                    if (panel.rightArrow != null)
                    {
                        panel.rightArrow.relativePosition = new Vector3(panel.relativePosition.x + panel.width, 0);
                    }
                }
            };

            int zOrder = oldPanel.zOrder;

            DestroyImmediate(oldPanel.gameObject);
            DestroyScrollbars(panel.parent);

            panel.zOrder = zOrder;

            // Left / Right buttons
            var arrow = panel.parent.AddUIComponent<UIArrowPanelButton>();
            arrow.Initialize(UIArrowPanelButton.Direction.Left);
            arrow.relativePosition = new Vector2(16, 0);
            panel.leftArrow = arrow;

            arrow = panel.parent.AddUIComponent<UIArrowPanelButton>();
            arrow.Initialize(UIArrowPanelButton.Direction.Right);
            arrow.relativePosition = new Vector2(811, 0);
            panel.rightArrow = arrow;

            PopulateList(panel);

            return panel;
        }

        private static void DestroyScrollbars(UIComponent parent)
        {
            UIScrollbar[] scrollbars = parent.GetComponentsInChildren<UIScrollbar>();
            foreach (UIScrollbar scrollbar in scrollbars)
            {
                DestroyImmediate(scrollbar.gameObject);
            }
        }

        private static void PopulateList(UIGeneratorOptionPanel panel)
        {
            for (var i = 0u; i < PrefabCollection<BuildingInfo>.PrefabCount(); i++)
            {
                var prefab = PrefabCollection<BuildingInfo>.GetPrefab(i) as BuildingInfo;

                if (prefab.m_class.m_subService == ItemClass.SubService.IndustrialGeneric)
                {
                    var data = new UIGridItem.ItemData
                    {
                        name = LocaleUtils.GetLocalizedTitle(prefab),
                        prefab = prefab,
                        tooltip = LocaleUtils.GetLocalizedTooltip(prefab),
                    };
                    data.tooltipBox = GeneratedPanel.GetTooltipBox(TooltipHelper.GetHashCode(data.tooltip));

                    panel.itemsData.Add(data);
                }
            }

            panel.DisplayAt(0);
        }

        /*
        public static void SetupInstance(UIScrollablePanel oldPanel)
        {
            var grid = oldPanel.parent.AddUIComponent<UIDistributionButton>();
            grid.Initialize("Grid");
            grid.transform.parent = oldPanel.transform;

            var line = oldPanel.parent.AddUIComponent<UIDistributionButton>();
            line.Initialize("Line");
            line.transform.parent = oldPanel.transform;
            line.Disable();

            var mine = oldPanel.parent.AddUIComponent<UIDistributionButton>();
            mine.Initialize("Mine");
            mine.transform.parent = oldPanel.transform;
            mine.Disable();

            grid.eventClicked += (c, p) =>
            {
                line.Pressed = false;
                mine.Pressed = false;
                if (!grid.Pressed)
                {
                    IndustryTool.instance.SetGridDistribution();
                }
            };

            line.eventClicked += (c, p) =>
            {
                grid.Pressed = false;
                mine.Pressed = false;
                if (!line.Pressed)
                {
                    IndustryTool.instance.SetLineDistribution();
                }
            };

            mine.eventClicked += (c, p) =>
            {
                grid.Pressed = false;
                line.Pressed = false;
                if (!mine.Pressed)
                {
                    IndustryTool.instance.SetMineDistribution();
                }
            };
        }
        */
    }
}
