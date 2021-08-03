using ColossalFramework;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;

namespace IndustryLP.Utils
{
    internal static class LocaleUtils
    {
        public static string GetLocalizedTitle(PrefabInfo prefab)
        {
            if (!Locale.GetUnchecked("BUILDING_TITLE", prefab.name, out string name))
            {
                name = prefab.name;
            }
            else
            {
                name = name.Replace(".", "");
            }

            int index = name.IndexOf('.');
            if (index >= 0)
            {
                name = name.Substring(index + 1);
            }

            if (name.IsNullOrWhiteSpace())
            {
                name = prefab.name;
            }

            index = name.LastIndexOf("_Data");
            if (index >= 0)
            {
                name = name.Substring(0, index);
            }

            return name;
        }

        public static string GetLocalizedTooltip(PrefabInfo prefab)
        {
            MilestoneInfo unlockMilestone = prefab.GetUnlockMilestone();

            string text = TooltipHelper.Format(new string[]
            {
                LocaleFormatter.Title,
                GetLocalizedTitle(prefab),
                LocaleFormatter.Sprite,
                (!string.IsNullOrEmpty(prefab.m_InfoTooltipThumbnail)) ? prefab.m_InfoTooltipThumbnail : prefab.name,
                LocaleFormatter.Text,
                prefab.GetLocalizedDescription(),
                LocaleFormatter.Locked,
                (!ToolsModifierControl.IsUnlocked(unlockMilestone)).ToString()
            });

            ToolsModifierControl.GetUnlockingInfo(unlockMilestone, out string unlockDesc, out string currentValue, out string targetValue, out string progress, out string locked);

            string addTooltip = TooltipHelper.Format(new string[]
            {
                LocaleFormatter.LockedInfo,
                locked,
                LocaleFormatter.UnlockDesc,
                unlockDesc,
                LocaleFormatter.UnlockPopulationProgressText,
                progress,
                LocaleFormatter.UnlockPopulationTarget,
                targetValue,
                LocaleFormatter.UnlockPopulationCurrent,
                currentValue
            });

            text = TooltipHelper.Append(text, addTooltip);
            PrefabAI aI = prefab.GetAI();
            if (aI != null)
            {
                text = TooltipHelper.Append(text, aI.GetLocalizedTooltip());
            }

            return text;
        }

        public static string GetLocalizedDescription(PrefabInfo prefab)
        {
            if (Locale.GetUnchecked("BUILDING_DESC", prefab.name, out string result))
            {
                return result;
            }

            return prefab.GetLocalizedDescription();
        }
    }
}
