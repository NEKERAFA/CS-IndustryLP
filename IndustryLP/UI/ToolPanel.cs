using UnityEngine;
using ColossalFramework.UI;
using IndustryLP.Utils.Constants;
using IndustryLP.UI.Buttons;
using System;
using System.Collections.Generic;
using IndustryLP.Tools;

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
            public ToolButton.OnButtonPressedDelegate Callback;
        }

        #region Atributtes

        private UILabel m_title = null;
        private List<ToolButton> m_buttons = null;

        #endregion

        #region Properties

        public static string ObjectName => LibraryConstants.UIPrefix + "_ToolWindow";

        public List<ToolAction> ToolActions { get; set; }

        #endregion

        #region Panel Behaviour

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
            //var dragHandler = AddUIComponent<UIDragHandle>();
            //dragHandler.transform.parent = transform;
            //dragHandler.transform.localPosition = Vector3.zero;
            //dragHandler.target = this;
            //dragHandler.size = size;

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

            if (m_buttons != null)
            {
                m_buttons.ForEach(b => Destroy(b.gameObject));
                m_buttons.Clear();
                m_buttons = null;
            }
        }

        /// <summary>
        /// Creates a label as the title
        /// </summary>
        private void SetupTitle()
        {
            m_title = AddUIComponent<UILabel>();
            m_title.transform.parent = transform;
            m_title.transform.localPosition = Vector3.zero;
            m_title.relativePosition = new Vector2(5f, 5f);
            m_title.text = ModInfo.ModName;
        }
        
        /// <summary>
        /// Creates the tool buttons
        /// </summary>
        private void SetupTools()
        {
            m_buttons = new List<ToolButton>();

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
                    x += 40f;
                    m_buttons.Add(button);
                }
            }
        }

        public void DisableAllButtons()
        {
            if (m_buttons != null) m_buttons.ForEach(b => b.IsChecked = false);
        }

        public void DisableButton(string name)
        {
            foreach(var button in m_buttons)
            {
                if (button.name == name)
                    button.IsChecked = false;
            }
        }

        #endregion
    }
}
