using ColossalFramework.UI;
using IndustryLP.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndustryLP.UI.Panels.Items
{
    internal class UIBuildingItem : IUIFastListItem<UIBuildingItem.ItemData, UIButton>
    {
        public static HashSet<PrefabInfo> fixedFocusedTexture = new HashSet<PrefabInfo>();

        #region Properties

        private ItemData m_currentData { get; set; }
        private UIComponent m_tooltipBox { get; set; }
        public UIButton component { get; set; }

        #endregion

        #region Class Data

        public class ItemData : IItemData
        {
            public string Name { get; set; }
            public string Tooltip { get; set; }
            public UIComponent TooltipBox { get; set; }
            public UIScrollablePanel Panel { get; set; }
            public PrefabInfo Prefab { get; set; }
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            component.text = string.Empty;
            component.tooltipAnchor = UITooltipAnchor.Anchored;
            component.tabStrip = true;
            component.horizontalAlignment = UIHorizontalAlignment.Center;
            component.verticalAlignment = UIVerticalAlignment.Middle;
            component.pivot = UIPivotPoint.TopCenter;
            component.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            component.group = component.parent;

            component.eventTooltipShow += (c, p) =>
            {
                if (m_tooltipBox != null && m_tooltipBox.isVisible && m_tooltipBox != p.tooltip)
                {
                    m_tooltipBox.Hide();
                }
                m_tooltipBox = p.tooltip;
            };

            component.eventMouseLeave += (c, p) =>
            {
                if (m_tooltipBox != null && m_tooltipBox.isVisible)
                {
                    m_tooltipBox.Hide();
                }
            };

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
                    return;
                }

                if (component == null || data?.Name == null) return;

                m_currentData = data;

                component.Unfocus();
                component.name = data.Name;
                component.gameObject.GetComponent<TutorialUITag>().tutorialTag = data.Name;

                PrefabInfo prefab = data.Prefab;
                if (prefab == null)
                {
                    LoggerUtils.Warning("Couldn't display item. Prefab is null");
                    return;
                }

                ImageUtils.FixThumbnails(prefab, null);

                component.atlas = prefab.m_Atlas;
                component.verticalAlignment = UIVerticalAlignment.Middle;

                component.normalFgSprite = prefab.m_Thumbnail;
                component.hoveredFgSprite = prefab.m_Thumbnail + "Hovered";
                component.pressedFgSprite = prefab.m_Thumbnail + "Pressed";
                component.disabledFgSprite = prefab.m_Thumbnail + "Disabled";
                component.focusedFgSprite = null;

                component.isEnabled = true; //ToolsModifierControl.IsUnlocked(prefab.GetUnlockMilestone());
                component.tooltip = data.Tooltip;
                component.tooltipBox = data.TooltipBox;
                component.objectUserData = data.Prefab;
                component.forceZOrder = index;

                if (component.containsMouse)
                {
                    component.RefreshTooltip();

                    if (m_tooltipBox != null && m_tooltipBox.isVisible && m_tooltipBox != data.TooltipBox)
                    {
                        m_tooltipBox.Hide();
                        data.TooltipBox.Show(true);
                        data.TooltipBox.opacity = 1f;
                        data.TooltipBox.relativePosition = m_tooltipBox.relativePosition + new Vector3(0, m_tooltipBox.height - data.TooltipBox.height);
                    }

                    m_tooltipBox = data.TooltipBox;

                    RefreshTooltipAltas(component);
                }
            }
            catch (Exception e)
            {
                if (data != null)
                {
                    LoggerUtils.Warning("Display failed : " + data.Name);
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
            try
            {
                if (m_currentData != null && m_currentData.Prefab != null && !fixedFocusedTexture.Contains(m_currentData.Prefab))
                {
                    if (ImageUtils.FixFocusedTexture(m_currentData.Prefab))
                    {
                        LoggerUtils.Log("Fixed focused texture: " + m_currentData.Prefab.name);
                    }
                    fixedFocusedTexture.Add(m_currentData.Prefab);
                }

                component.normalFgSprite = m_currentData.Prefab.m_Thumbnail + "Focused";
                component.hoveredFgSprite = m_currentData.Prefab.m_Thumbnail + "Focused";
            }
            catch (Exception e)
            {
                if (m_currentData != null)
                {
                    LoggerUtils.Error($"Select failed : {m_currentData.Name}", e);
                }
                else
                {
                    LoggerUtils.Error("Select failed", e);
                }
            }
        }

        public void Deselect(int index)
        {
            try
            {
                component.normalFgSprite = m_currentData.Prefab.m_Thumbnail;
                component.hoveredFgSprite = m_currentData.Prefab.m_Thumbnail + "Hovered";
            }
            catch (Exception e)
            {
                if (m_currentData != null)
                {
                    LoggerUtils.Error("Deselect failed : " + m_currentData.Name, e);
                }
                else
                {
                    LoggerUtils.Error("Deselect failed", e);
                }
            }
        }

        #endregion

        #region Private Methods

        private void RefreshTooltipAltas(UIComponent item)
        {
            PrefabInfo prefab = item.objectUserData as PrefabInfo;
            if (prefab != null)
            {
                UISprite uISprite = item.tooltipBox.Find<UISprite>("Sprite");
                if (uISprite != null)
                {
                    if (prefab.m_InfoTooltipAtlas != null)
                    {
                        uISprite.atlas = prefab.m_InfoTooltipAtlas;
                    }
                    if (!string.IsNullOrEmpty(prefab.m_InfoTooltipThumbnail) && uISprite.atlas[prefab.m_InfoTooltipThumbnail] != null)
                    {
                        uISprite.spriteName = prefab.m_InfoTooltipThumbnail;
                    }
                    else
                    {
                        uISprite.spriteName = "ThumbnailBuildingDefault";
                    }
                }
            }
        }

        #endregion
    }
}
