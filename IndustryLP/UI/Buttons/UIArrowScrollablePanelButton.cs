using ColossalFramework.UI;
using IndustryLP.Utils;
using UnityEngine;

namespace IndustryLP.UI.RestrictionButtons
{
    internal class UIArrowScrollablePanelButton : UIButton
    {
        public enum Direction { Left, Right }

        #region Public methods

        public void Initialize(Direction direction)
        {
            atlas = ResourceLoader.GetAtlas("Ingame");
            name = $"Arrow{direction}";
            size = new Vector2(32, 109);
            foregroundSpriteMode = UIForegroundSpriteMode.Scale;
            horizontalAlignment = UIHorizontalAlignment.Center;
            verticalAlignment = UIVerticalAlignment.Middle;
            normalFgSprite = $"Arrow{direction}";
            focusedFgSprite = $"Arrow{direction}Focused";
            hoveredFgSprite = $"Arrow{direction}Hovered";
            pressedFgSprite = $"Arrow{direction}Pressed";
            disabledFgSprite = $"Arrow{direction}Disabled";
            isEnabled = false;
        }

        #endregion
    }
}
