using UnityEngine;
using ColossalFramework.UI;
using IndustryLP.Utils.Constants;
using System.Collections.Generic;
using IndustryLP.Tools;
using IndustryLP.UI.Buttons;
using IndustryLP.Utils;

namespace IndustryLP.UI
{
    /// <summary>
    /// Defines the main toolbar of the mod
    /// </summary>
    internal class ToolPanel : UIPanel
    {
        public struct ToolAction
        {
            public ToolActionController Controller;
            public ToolButton.OnButtonClickedDelegate Callback;
        }

        #region Attributes

        private UILabel m_title = null;
        private SelectionButton m_selectionButton = null;
        private GenerateOptionsButton m_generatorButton = null;
        private bool m_selectionDone = false;

        #endregion

        #region Properties

        public static string ObjectName => LibraryConstants.UIPrefix + "_ToolWindow";

        public List<ToolAction> ToolActions { get; set; }

        public bool IsSelectionDone
        {
            get
            {
                return m_selectionDone;
            }
            set
            {
                m_selectionDone = value;

                if (m_generatorButton != null)
                    if (m_selectionDone) m_generatorButton.Enable();
                    else m_generatorButton.Disable();
            }
        }

        #endregion

        #region Unity Behaviour

        /// <summary>
        /// Invoked when the panel is created
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Set the toolbar values
            name = ObjectName;
            backgroundSprite = "SubcategoriesPanel";
            size = new Vector2(200, 64);
            opacity = 0.8f;

            // Set the title
            SetupTitle();

            // Defines a drag handler of the toolbar
            GUIUtils.CreateDragHandle(this, size);

            // set the tool buttons
            SetupTools();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (m_title != null)
            {
                Destroy(m_title.gameObject);
                m_title = null;
            }

            if (m_selectionButton != null)
            {
                Destroy(m_selectionButton.gameObject);
                m_selectionButton = null;
            }

            if (m_generatorButton != null)
            {
                Destroy(m_generatorButton.gameObject);
                m_generatorButton = null;
            }
        }

        #endregion

        #region Panel Behaviour

        /// <summary>
        /// Creates a label as the title
        /// </summary>
        private void SetupTitle()
        {
            m_title = GUIUtils.CreateLabel(this, ModInfo.ModName);
            m_title.relativePosition = new Vector2(5f, 5f);
        }
        
        /// <summary>
        /// Creates the tool buttons
        /// </summary>
        private void SetupTools()
        {
            var x = 5f;
            foreach (var tool in ToolActions)
            {
                if (tool.Controller != null)
                {
                    var button = tool.Controller.CreateButton(isChecked => tool.Callback?.Invoke(isChecked));
                    AttachUIComponent(button.gameObject);
                    button.transform.parent = transform;
                    button.transform.localPosition = Vector3.zero;
                    button.relativePosition = new Vector3(x, m_title.height + 10f);

                    if (button is SelectionButton)
                    {
                        m_selectionButton = button as SelectionButton;
                    }
                    else
                    {
                        m_generatorButton = button as GenerateOptionsButton;
                        m_generatorButton.Disable();
                    }

                    x += 40f;
                }
            }
        }

        /// <summary>
        /// Set disabled all buttons
        /// </summary>
        public void DisableAllButtons()
        {
            if (m_generatorButton != null)
                m_generatorButton.IsChecked = false;

            if (m_selectionButton != null)
                m_selectionButton.IsChecked = false;
        }

        /// <summary>
        /// Set disable a specified button
        /// </summary>
        /// <param name="name"></param>
        public void DisableButton(string name)
        {
            ToolButton button;

            if (m_generatorButton.name.Equals(name))
                button = m_generatorButton;
            else
                button = m_selectionButton;

            button.IsChecked = false;
        }

        #endregion
    }
}
