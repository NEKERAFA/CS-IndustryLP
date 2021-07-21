using ColossalFramework.UI;
using IndustryLP.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndustryLP.UI.Panels.Items
{
    internal class UIGridItem : IUIFastListItem<UIGridItem.ItemData, UIButton>
    {
        public ItemData m_currentData;
        private static UIComponent m_tooltipBox;

        public static HashSet<PrefabInfo> fixedFocusedTexture = new HashSet<PrefabInfo>();

        public UIButton component { get; set; }

        public class ItemData : UIItemData
        {
            public PrefabInfo prefab;
        }

        public void Init()
        {
            component.text = string.Empty;
            component.tooltipAnchor = UITooltipAnchor.Anchored;
            component.tabStrip = true;
            component.horizontalAlignment = UIHorizontalAlignment.Center;
            component.verticalAlignment = UIVerticalAlignment.Middle;
            component.pivot = UIPivotPoint.TopCenter;
            component.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
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

        public void Select(int index)
        {
            try
            {
                if (m_currentData != null && m_currentData.prefab != null && !fixedFocusedTexture.Contains(m_currentData.prefab))
                {
                    if (ImageUtils.FixFocusedTexture(m_currentData.prefab))
                    {
                        // Debugging.Message("Fixed focused texture: " + currentData.asset.prefab.name);
                    }
                    fixedFocusedTexture.Add(m_currentData.prefab);
                }

                component.normalFgSprite = m_currentData.prefab.m_Thumbnail + "Focused";
                component.hoveredFgSprite = m_currentData.prefab.m_Thumbnail + "Focused";
            }
            catch (Exception e)
            {
                if (m_currentData != null)
                {
                    LoggerUtils.Log($"Select failed : {m_currentData.name}");
                }
                else
                {
                    LoggerUtils.Log("Select failed");
                }
                LoggerUtils.Error(e);
            }
        }

        public void Deselect(int index)
        {
            try
            {
                component.normalFgSprite = m_currentData.prefab.m_Thumbnail;
                component.hoveredFgSprite = m_currentData.prefab.m_Thumbnail + "Hovered";
            }
            catch (Exception e)
            {
                if (m_currentData != null)
                {
                    LoggerUtils.Log("Deselect failed : " + m_currentData.name);
                }
                else
                {
                    LoggerUtils.Log("Deselect failed");
                }
                LoggerUtils.Error(e);
            }
        }

        public void Display(ItemData data, int index)
        {
            try
            {
                if (data == null)
                {
                    LoggerUtils.Log("Data null");
                    return;
                }

                if (component == null || data?.name == null) return;

                m_currentData = data;

                component.Unfocus();
                component.name = data.name;
                component.gameObject.GetComponent<TutorialUITag>().tutorialTag = data.name;

                PrefabInfo prefab = data.prefab;
                if (prefab == null)
                {
                    LoggerUtils.Log("Couldn't display item. Prefab is null");
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
                component.tooltip = data.tooltip;
                component.tooltipBox = data.tooltipBox;
                component.objectUserData = data.prefab;
                component.forceZOrder = index;

                if (component.containsMouse)
                {
                    component.RefreshTooltip();

                    if (m_tooltipBox != null && m_tooltipBox.isVisible && m_tooltipBox != data.tooltipBox)
                    {
                        m_tooltipBox.Hide();
                        data.tooltipBox.Show(true);
                        data.tooltipBox.opacity = 1f;
                        data.tooltipBox.relativePosition = m_tooltipBox.relativePosition + new Vector3(0, m_tooltipBox.height - data.tooltipBox.height);
                    }

                    m_tooltipBox = data.tooltipBox;

                    RefreshTooltipAltas(component);
                }
            }
            catch (Exception e)
            {
                if (data != null)
                {
                    LoggerUtils.Warning("Display failed : " + data.name);
                }
                else
                {
                    LoggerUtils.Warning("Display failed");
                }

                LoggerUtils.Error(e);
            }
        }

        public static void RefreshTooltipAltas(UIComponent item)
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
    }
}
