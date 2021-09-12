using ColossalFramework.DataBinding;
using ColossalFramework.UI;
using IndustryLP.UI.Panels.Items;
using IndustryLP.UI.RestrictionButtons;
using IndustryLP.Utils;
using System;
using UnityEngine;

namespace IndustryLP.UI.Panels
{
    internal class UIBuildingsOptionPanel : UIFastList<UIBuildingItem.ItemData, UIBuildingItem, UIButton>
    {
        #region Public Methods

        public static UIBuildingsOptionPanel Create(UIScrollablePanel oldPanel)
        {
            UIBuildingsOptionPanel panel = oldPanel.parent.AddUIComponent<UIBuildingsOptionPanel>();
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
            var arrow = panel.parent.AddUIComponent<UIArrowScrollablePanelButton>();
            arrow.Initialize(UIArrowScrollablePanelButton.Direction.Left);
            arrow.relativePosition = new Vector2(16, 0);
            panel.leftArrow = arrow;

            arrow = panel.parent.AddUIComponent<UIArrowScrollablePanelButton>();
            arrow.Initialize(UIArrowScrollablePanelButton.Direction.Right);
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
            try
            {
                itemsData.Clear();

                for (var i = 0u; i < PrefabCollection<BuildingInfo>.PrefabCount(); i++)
                {
                    var prefab = PrefabCollection<BuildingInfo>.GetPrefab(i);

                    if (prefab != null && 
                        (prefab.m_class.m_subService == ItemClass.SubService.IndustrialGeneric ||
                         prefab.m_class.m_subService == ItemClass.SubService.IndustrialFarming ||
                         prefab.m_class.m_subService == ItemClass.SubService.IndustrialForestry ||
                         prefab.m_class.m_subService == ItemClass.SubService.IndustrialOil ||
                         prefab.m_class.m_subService == ItemClass.SubService.IndustrialOre))
                    {
                        var data = new UIBuildingItem.ItemData
                        {
                            Name = LocaleUtils.GetLocalizedTitle(prefab),
                            Prefab = prefab,
                            Tooltip = LocaleUtils.GetLocalizedTooltip(prefab),
                            Panel = this
                        };
                        data.TooltipBox = GeneratedPanel.GetTooltipBox(TooltipHelper.GetHashCode(data.Tooltip));

                        itemsData.Add(data);
                    }
                }

                DisplayAt(0);
            }
            catch (Exception e)
            {
                LoggerUtils.Error(e, "Error populating table");
            }
        }

        private static void DestroyScrollbars(UIComponent parent)
        {
            UIScrollbar[] scrollbars = parent.GetComponentsInChildren<UIScrollbar>();
            foreach (UIScrollbar scrollbar in scrollbars)
            {
                DestroyImmediate(scrollbar.gameObject);
            }
        }

        #endregion
    }
}
