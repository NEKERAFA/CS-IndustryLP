using ColossalFramework.UI;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using System;
using UnityEngine;

namespace IndustryLP.UI
{
    internal class GeneratorDialog : UIPanel
    {
        #region Attributes

        private GUIUtils.UITitle m_title = null;
        private UILabel m_solutionsLbl = null;
        private UITextField m_solutionsInput = null;
        private UIButton m_generateBtn = null;

        #endregion

        #region Properties

        public static string ObjectName => LibraryConstants.UIPrefix + "_GeneratorDialog";

        /// <summary>
        /// Callback invoked when the dialog will be closed
        /// </summary>
        public MouseEventHandler CloseHandler { get; set; } = null;

        /// <summary>
        /// Callback invoked when the form will be accepted
        /// </summary>
        public MouseEventHandler GenerateHandler { get; set; } = null;

        /// <summary>
        /// Gets the number of solutions setted in the form
        /// </summary>
        public int Solutions
        {
            get
            {
                return Convert.ToInt32(m_solutionsInput.text);
            }
            set
            {
                m_solutionsInput.text = value.ToString();
            }
        }

        #endregion

        #region Unity Behaviour

        public override void Start()
        {
            base.Start();

            // Set the panel values
            name = ObjectName;
            backgroundSprite = "MenuBadgesBackground";
            size = new Vector2(300, 105);

            // Sets the widgets
            SetupTitle();
            SetupOptions();
            SetupAccept();
        }

        #endregion

        #region Panel Behaviour

        #region Panel widgets setup

        /// <summary>
        /// Creates a label as the title
        /// </summary>
        private void SetupTitle()
        {
            // Text
            m_title = GUIUtils.CreateTitle(this, "GENERATE OPTIONS", new Vector2(300, 31), 
                (UIComponent c, UIMouseEventParameter p) =>
                { 
                    Hide();
                    CloseHandler?.Invoke(c, p);
                });
        }

        /// <summary>
        /// Creates the menu
        /// </summary>
        private void SetupOptions()
        {
            // Text
            m_solutionsLbl = GUIUtils.CreateLabel(this, "Solutions:");
            m_solutionsLbl.relativePosition = new Vector2(10, 39);

            // Input
            m_solutionsInput = GUIUtils.CreateTextField(this, GUIUtils.TextFieldType.UnsignedInteger, "2");
            m_solutionsInput.relativePosition = new Vector2(290 - m_solutionsInput.width, 39);
        }

        /// <summary>
        /// Generate accept button
        /// </summary>
        private void SetupAccept()
        {
            // Accept button
            m_generateBtn = GUIUtils.CreateButton(this, "Generate", true);
            m_generateBtn.width = 100;
            m_generateBtn.relativePosition = new Vector2(190, 47 + m_solutionsInput.height);
            if (GenerateHandler != null)
                m_generateBtn.eventClicked += GenerateHandler;
        }

        #endregion

        #endregion
    }
}
