using ColossalFramework.UI;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using System;
using UnityEngine;

namespace IndustryLP.UI.Panels
{
    internal class UIGenerationDialog : UIPanel
    {
        #region Attributes

        private GUIUtils.UITitle m_title;
        private UILabel m_solutionsLbl;
        private UITextField m_solutionsInput;
        private UILabel m_advancedLbl;
        private UITextField m_advancedInput;
        private UIButton m_generateBtn;
        private int m_solutions;
        private string m_logicProgram;

        #endregion

        #region Properties

        public static string ObjectName => LibraryConstants.UIPrefix + "_GeneratioDialog";

        /// <summary>
        /// Callback invoked when the dialog will be closed
        /// </summary>
        public MouseEventHandler OnCloseDialog { get; set; }

        /// <summary>
        /// Callback invoked when the form will be accepted
        /// </summary>
        public MouseEventHandler OnClickAcceptButton { get; set; }

        public int Solutions
        {
            get
            {
                if (m_solutionsInput != null)
                {
                    m_solutions = Convert.ToInt32(m_solutionsInput.text);
                }

                return m_solutions;
            }
            set
            {
                m_solutions = value;

                if (m_solutionsInput != null)
                {
                    m_solutionsInput.text = m_solutions.ToString();
                }
            }
        }

        public string AdvancedEdition
        {
            get
            {
                if (m_advancedInput != null)
                {
                    m_logicProgram = m_advancedInput.text;
                }
                return m_logicProgram;
            }
            set
            {

                m_logicProgram = value;
                
                if (m_advancedInput != null)
                {
                    m_advancedInput.text = m_logicProgram;
                }
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
            size = new Vector2(300, 300);

            // Sets the widgets
            SetupTitle();
            SetupOptions();
            SetupAccept();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            DestroyImmediate(m_title);
            DestroyImmediate(m_solutionsLbl);
            DestroyImmediate(m_solutionsInput);
            DestroyImmediate(m_advancedLbl);
            DestroyImmediate(m_advancedInput);
            DestroyImmediate(m_generateBtn);
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
            m_title = GUIUtils.CreateTitle(this, "GENERATION OPTIONS", new Vector2(300, 32),
                (UIComponent c, UIMouseEventParameter p) =>
                {
                    Hide();
                    OnCloseDialog?.Invoke(c, p);
                });
        }

        /// <summary>
        /// Creates the menu
        /// </summary>
        private void SetupOptions()
        {
            // Solution
            m_solutionsLbl = GUIUtils.CreateLabel(this, "Solutions:");
            m_solutionsLbl.relativePosition = new Vector2(10, 40);

            // Input Solution
            m_solutionsInput = GUIUtils.CreateTextField(this, GUIUtils.TextFieldType.UnsignedInteger, m_solutions.ToString());
            m_solutionsInput.relativePosition = new Vector2(290 - m_solutionsInput.width, 40);

            // Advanced Edition
            m_advancedLbl = GUIUtils.CreateLabel(this, "Advanced Edition:");
            m_advancedLbl.relativePosition = new Vector2(10, 63);

            // Input Advanced Edition
            m_advancedInput = GUIUtils.CreateTextField(this, GUIUtils.TextFieldType.String, m_logicProgram);
            m_advancedInput.multiline = true;
            m_advancedInput.relativePosition = new Vector2(10, 84);
            m_advancedInput.size = new Vector2(280, 176);
        }

        /// <summary>
        /// Generate accept button
        /// </summary>
        private void SetupAccept()
        {
            // Accept button
            m_generateBtn = GUIUtils.CreateButton(this, "Generate", true);
            m_generateBtn.width = 100;
            m_generateBtn.relativePosition = new Vector2(290 - m_generateBtn.width, 290 - m_generateBtn.height);
            if (OnClickAcceptButton != null)
            {
                m_generateBtn.eventClicked += (UIComponent c, UIMouseEventParameter p) =>
                {
                    OnClickAcceptButton?.Invoke(c, p);
                    m_advancedInput.isInteractive = false;
                    m_advancedInput.builtinKeyNavigation = false;
                };
            }
        }

        #endregion

        #endregion
    }
}
