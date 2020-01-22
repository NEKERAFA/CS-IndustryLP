using UnityEngine;
using ColossalFramework.UI;
using IndustryLP.Constants;

namespace IndustryLP.UI.Buttons
{
    /// <summary>
    /// Creates a clickable button that represents the selection tool
    /// </summary>
    public class UISelectionButton : UIButton
    {
        #region Propeties

        public static readonly string ObjectName = $"{LibraryConstants.UIPrefix}_FactoryButton";

        #endregion

        #region Button

        /// <summary>
        /// Invoked when the button is created
        /// </summary>
        public override void Start()
        {
            name = ObjectName;
            size = new Vector2(32, 32);
            atlas = IndustryLPTool.CustomAtlas;
            normalBgSprite = ResourceConstants.ButtonNormal;
            hoveredBgSprite = ResourceConstants.ButtonHover;
            normalFgSprite = ResourceConstants.AreaSelectionIcon;
        }

        #endregion
    }
}
