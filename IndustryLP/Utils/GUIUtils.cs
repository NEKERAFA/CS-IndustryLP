using ColossalFramework.UI;
using IndustryLP.Utils.Constants;
using UnityEngine;

namespace IndustryLP.Utils
{
    internal static class GUIUtils
    {
        public sealed class UITitle : UIComponent
        {
            public UILabel label { get; set; }
            public UIDragHandle dragHandle { get; set; }
            public UIButton closeButton { get; set; }

            public override void OnDestroy()
            {
                base.OnDestroy();

                label?.OnDestroy();
                dragHandle?.OnDestroy();
                closeButton?.OnDestroy();
            }
        }

        public sealed class UITextDebug : UIPanel
        {
            private UILabel m_textLabel;
            private string m_text;

            public override void Awake()
            {
                base.Awake();

                m_textLabel = AddUIComponent<UILabel>();
                m_textLabel.transform.parent = transform;
                m_textLabel.transform.localPosition = Vector2.zero;
                m_textLabel.relativePosition = new Vector2(5f, 5f);

                atlas = ResourceLoader.GetAtlas("Ingame");
                backgroundSprite = "SubcategoriesPanel";
                opacity = 0.6f;

                if (m_text != null)
                {
                    m_textLabel.text = m_text;
                    size = new Vector2(m_textLabel.width + 10, m_textLabel.height + 10);
                }
            }

            public void SetText(string text)
            {
                m_text = text;

                if (m_textLabel != null)
                {
                    m_textLabel.text = m_text;
                    size = new Vector2(m_textLabel.width + 10, m_textLabel.height + 10);
                }
            }
        }

        public static UIButton CreateButton(UIComponent parent, string text = null, bool small = false)
        {
            UIButton btn = parent.AddUIComponent<UIButton>();
            btn.transform.parent = parent.transform;
            btn.transform.localPosition = Vector2.zero;
            btn.atlas = ResourceLoader.GetAtlas("Ingame");

            if (small)
            {
                btn.normalBgSprite = "ButtonSmall";
                btn.disabledBgSprite = "ButtonSmallDisabled";
                btn.focusedBgSprite = "ButtonSmallFocused";
                btn.hoveredBgSprite = "ButtonSmallHovered";
                btn.pressedBgSprite = "ButtonSmallPressed";
            }
            else
            {
                btn.normalBgSprite = "ButtonMenu";
                btn.disabledBgSprite = "ButtonMenuDisabled";
                btn.focusedBgSprite = "ButtonMenuFocused";
                btn.hoveredBgSprite = "ButtonMenuHovered";
                btn.pressedBgSprite = "ButtonMenuPressed";
            }

            if (text != null) btn.text = text; 

            return btn;
        }

        public static UIButton CreateCloseButton(UIComponent parent, Vector2? size = null)
        {
            UIButton closeBtn = parent.AddUIComponent<UIButton>();
            closeBtn.transform.parent = parent.transform;
            closeBtn.transform.localPosition = Vector2.zero;

            if (size.HasValue) closeBtn.size = size.Value;
            closeBtn.atlas = ResourceLoader.GetAtlas("Ingame");
            closeBtn.normalBgSprite = "buttonclose";
            closeBtn.hoveredBgSprite = "buttonclosehover";
            closeBtn.pressedBgSprite = "buttonclosepressed";

            return closeBtn;
        }

        public static UILabel CreateLabel(UIComponent parent, string text)
        {
            UILabel lbl = parent.AddUIComponent<UILabel>();
            lbl.transform.parent = parent.transform;
            lbl.transform.localPosition = Vector2.zero;
            lbl.text = text;

            return lbl;
        }

        public static UIDragHandle CreateDragHandle(UIComponent parent, Vector2? size)
        {
            UIDragHandle dragHandle = parent.AddUIComponent<UIDragHandle>();
            dragHandle.transform.parent = parent.transform;
            dragHandle.transform.localPosition = Vector2.zero;
            dragHandle.relativePosition = Vector2.zero;
            dragHandle.target = parent;
            dragHandle.size = size ?? parent.size;

            return dragHandle;
        }

        public static UITitle CreateTitle(UIComponent parent, string text, Vector2 size, MouseEventHandler onClose = null)
        {
            UITitle titleContainer = parent.AddUIComponent<UITitle>();
            titleContainer.transform.parent = parent.transform;
            titleContainer.transform.localPosition = Vector2.zero;
            titleContainer.relativePosition = Vector2.zero;

            UILabel lbl = CreateLabel(titleContainer, text);
            lbl.relativePosition = new Vector2(size.x / 2 - lbl.width / 2, size.y / 2 - lbl.height / 2);
            titleContainer.label = lbl;

            UIDragHandle dragHandle = CreateDragHandle(parent, size);
            titleContainer.dragHandle = dragHandle;

            if (onClose != null)
            {
                UIButton closeButton = CreateCloseButton(titleContainer, new Vector2(size.y - 6, size.y - 6));
                closeButton.relativePosition = new Vector2(size.x - closeButton.width - 3, 3);
                closeButton.eventClicked += onClose;
                dragHandle.width = dragHandle.width - closeButton.width - 6;
            }

            return titleContainer;
        }

        public enum TextFieldType { Float, Integer, UnsignedInteger, String, Password }

        public static UITextField CreateTextField(UIComponent parent, TextFieldType type = TextFieldType.String, string placeholder = "", bool isReadOnly = false)
        {
            UITextField textField = parent.AddUIComponent<UITextField>();
            textField.transform.parent = parent.transform;
            textField.transform.localPosition = Vector2.zero;
            textField.atlas = ResourceLoader.GetAtlas("Ingame");
            textField.text = placeholder;
            textField.isInteractive = true;
            textField.builtinKeyNavigation = true;
            textField.readOnly = false;
            /*
            textField.readOnly = isReadOnly;
            textField.numericalOnly = type != TextFieldType.String && type != TextFieldType.Password;
            textField.allowNegative = type != TextFieldType.UnsignedInteger;
            textField.allowFloats = type != TextFieldType.Integer && type != TextFieldType.UnsignedInteger;
            */
            textField.selectionSprite = "EmptySprite";
            textField.selectionBackgroundColor = ColorConstants.TextSelectionColor;
            textField.cursorWidth = 1;
            textField.cursorBlinkTime = 1;
            textField.normalBgSprite = "TextFieldPanel";
            textField.hoveredBgSprite = "TextFieldPanelHovered";
            textField.opacity = 0.7f;
            textField.textColor = ColorConstants.WhiteColor;
            textField.disabledTextColor = ColorConstants.DisableColor;
            textField.color = ColorConstants.WhiteColor;
            textField.padding = new RectOffset(2, 2, 2, 2);

            switch (type)
            {
                case TextFieldType.Float:
                case TextFieldType.Integer:
                case TextFieldType.UnsignedInteger:
                    textField.horizontalAlignment = UIHorizontalAlignment.Right;
                    break;
                default:
                    textField.horizontalAlignment = UIHorizontalAlignment.Left;
                    break;
            }

            return textField;
        }
    
        public static UITextDebug CreateTextDebug(UIComponent parent, string text)
        {
            UITextDebug textDebug = parent.AddUIComponent<UITextDebug>();
            textDebug.transform.parent = parent.transform;
            textDebug.transform.localPosition = Vector2.zero;
            textDebug.SetText(text);

            return textDebug;
        }
    }
}
