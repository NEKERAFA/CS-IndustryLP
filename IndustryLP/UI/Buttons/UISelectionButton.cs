using UnityEngine;
using ColossalFramework.UI;
using IndustryLP.Constants;
using IndustryLP.Enums;

namespace IndustryLP.UI.Buttons
{
    /// <summary>
    /// Creates a clickable button that represents the selection tool
    /// </summary>
    public class UISelectionButton : UIButton
    {
        #region Propeties

        public static readonly string ObjectName = $"{LibraryConstants.UIPrefix}_FactoryButton";

        public bool IsPressed { get; private set; }

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
            pressedBgSprite = ResourceConstants.ButtonFocused;
            normalFgSprite = ResourceConstants.SelectionIcon;

            IsPressed = false;
        }

        protected override void OnClick(UIMouseEventParameter ev)
        {
            if (ev.buttons.IsFlagSet(UIMouseButton.Left))
            {
                IsPressed = !IsPressed;
                IndustryLPTool.CurrentTool = ToolType.Selection;
            }
        }

        public override void Update()
        {
            base.Update();

            if (IsPressed)
            {
                normalBgSprite = ResourceConstants.ButtonFocused;
                hoveredBgSprite = ResourceConstants.ButtonFocused;
            }
            else
            {
                normalBgSprite = ResourceConstants.ButtonNormal;
                hoveredBgSprite = ResourceConstants.ButtonHover;
            }
        }

        #endregion
    }
}
