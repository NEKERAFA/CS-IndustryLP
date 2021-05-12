using ColossalFramework.UI;
using IndustryLP.Utils.Constants;

namespace IndustryLP.UI
{
    internal class UIDistributionButton : UIButton
    {
        private string m_thumnail;
        private bool m_pressed = false;

        public bool Pressed
        {
            get
            {
                return m_pressed;
            }

            set
            {
                m_pressed = value;

                if (m_pressed)
                {
                    if (hoveredBgSprite != focusedBgSprite)
                        hoveredBgSprite = focusedBgSprite;

                    if (pressedBgSprite != focusedBgSprite)
                        pressedBgSprite = focusedBgSprite;
                }
                else
                {
                    if (hoveredBgSprite == focusedBgSprite)
                        hoveredBgSprite = $"{m_thumnail}DistributionHovered";

                    if (pressedBgSprite == focusedBgSprite)
                        pressedBgSprite = $"{m_thumnail}DistributionPressed";
                }
            }
        }

        public void Initialize(string thumbnail)
        {
            atlas = IndustryTool.ThumbnailAtlas;
            playAudioEvents = true;

            m_thumnail = thumbnail;
            disabledBgSprite = ResourceConstants.DistributionDisabled;
            normalBgSprite = $"{m_thumnail}Distribution";
            focusedBgSprite = $"{m_thumnail}DistributionFocused";
            hoveredBgSprite = $"{m_thumnail}DistributionHovered";
            pressedBgSprite = $"{m_thumnail}DistributionPressed";
        }

        #region Unity Behaviour Methods

        protected override void OnButtonStateChanged(ButtonState value)
        {
            base.OnButtonStateChanged(value);
        }

        protected override void OnClick(UIMouseEventParameter p)
        {
            base.OnClick(p);

            Pressed = !Pressed;
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();

            if (!isVisible) m_pressed = false;
        }

        #endregion
    }
}