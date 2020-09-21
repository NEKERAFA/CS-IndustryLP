using UnityEngine;

namespace IndustryLP.Utils.Constants
{
    internal static class ColorConstants
    {
        /// <summary>
        /// The color to apply in selection mode when selection is not correct
        /// </summary>
        public static Color BadSelectionColor => new Color32(255, 0, 0, 220);

        /// <summary>
        /// The color to apply in selection mode
        /// </summary>
        public static Color SelectionColor => new Color32(255, 128, 0, 220);

        /// <summary>
        /// The color to apply when the object are selected
        /// </summary>
        public static Color SelectedColor => new Color32(200, 200, 200, 220);

        /// <summary>
        /// The color to apply when the object are disable
        /// </summary>
        public static Color32 DisableColor => new Color32(128, 128, 128, 255);

        /// <summary>
        /// The color to apply when the text is selected
        /// </summary>
        public static Color32 TextSelectionColor => new Color32(0, 192, 255, 255);

        /// <summary>
        /// White color, <c>RGBA(255, 255, 255, 255)</c>
        /// </summary>
        public static Color32 WhiteColor => new Color32(255, 255, 255, 255);
    }
}
