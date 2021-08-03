using ColossalFramework.UI;
using IndustryLP.UI.Panels.Items;
using IndustryLP.UI.RestrictionButtons;
using IndustryLP.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace IndustryLP.UI.Panels
{
    internal class UIDistributionOptionPanel : UIFastList<UIDistributionItem.ItemData, UIDistributionItem, UIButton>
    {
        #region Public Methods

        public static UIDistributionOptionPanel Create(UIScrollablePanel oldPanel)
        {
            UIDistributionOptionPanel panel = oldPanel.parent.AddUIComponent<UIDistributionOptionPanel>();
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

            panel.parent.parent.eventSizeChanged += panel.OnPanelChangeSize;

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

            panel.PopulateTable();

            return panel;
        }

        #endregion

        #region Private Methods

        private void OnPanelChangeSize(UIComponent c, Vector2 size)
        {
            if (isVisible)
            {
                size = new Vector2((int)((size.x - 40f) / itemWidth) * itemWidth, (int)(size.y / itemHeight) * itemHeight);
                relativePosition = new Vector3(relativePosition.x, Mathf.Floor((size.y - height) / 2));

                if (rightArrow != null)
                {
                    rightArrow.relativePosition = new Vector3(relativePosition.x + width, 0);
                }
            }
        }

        private void PopulateTable()
        {
            itemsData.Clear();

            foreach (var type in Enum.GetValues(typeof(UIDistributionItem.ItemType)).Cast<UIDistributionItem.ItemType>())
            {
                var data = new UIDistributionItem.ItemData
                {
                    Name = type.ToString(),
                    Tooltip = type.ToString(),
                    Panel = this,
                    Type = type
                };

                LoggerUtils.Log($"Added {type} item");

                itemsData.Add(data);
            }

            DisplayAt(0);
        }

        private static void DestroyScrollbars(UIComponent parent)
        {
            UIScrollbar[] scrollbars = parent.GetComponentsInChildren<UIScrollbar>();
            foreach (var scrollbar in scrollbars)
            {
                DestroyImmediate(scrollbar.gameObject);
            }

            UIArrowPanelButton[] arrows = parent.GetComponentsInChildren<UIArrowPanelButton>();
            foreach (var arrow in arrows)
            {
                DestroyImmediate(arrow.gameObject);
            }
        }

        #endregion

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
