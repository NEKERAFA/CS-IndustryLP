using UnityEngine;
using ColossalFramework.UI;
using IndustryLP.Common;
using IndustryLP.Constants;

namespace IndustryLP.UI.Buttons
{
    public class UIFactoryButton : UIButton
    {
        public static readonly string ObjectName = $"{LibraryConstants.UIPrefix}_FactoryButton";

        public override void Start()
        {
            LoadResources();

            name = ObjectName;
            normalBgSprite = ResourceConstants.FactoryIcon;
            size = new Vector2(32, 32);

        }

        private void LoadResources()
        {
            string[] sprites = 
            {
                ResourceConstants.FactoryIcon
            };

            atlas = ResourceLoader.CreateTextureAtlas(LibraryConstants.LibPrefix, sprites, ResourceConstants.IconPath);
        }
    }
}
