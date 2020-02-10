using ColossalFramework.Math;
using IndustryLP.Utils.Constants;
using IndustryLP.Utils.Enums;
using IndustryLP.UI;
using IndustryLP.UI.Buttons;
using IndustryLP.Utils;
using UnityEngine;

namespace IndustryLP.Tools
{
    /// <summary>
    /// Represents the selection tool
    /// </summary>
    internal class SelectionTool : ToolActionController
    {
        #region Attributes

        internal Quad3? m_currentMouseSelection;

#if SHOW_DEBUG
        private UIPanel m_showPositionA;
        private UIPanel m_showPositionB;
        private UIPanel m_showPositionC;
        private UIPanel m_showPositionD;
#endif

        #endregion

        #region Properties

        private SelectionState CurrentState { get; set; } = SelectionState.None;

        #endregion

        public override void OnStart(MainTool mainTool)
        {
#if SHOW_DEBUG
            var view = UIView.GetAView();
            m_showPositionA = CreatePositionPanel(view, "showPositionA");
            m_showPositionB = CreatePositionPanel(view, "showPositionB");
            m_showPositionC = CreatePositionPanel(view, "showPositionC");
            m_showPositionD = CreatePositionPanel(view, "showPositionD");

            DisableAllPositionPanels();
#endif
        }

        public override void OnDestroy()
        {
#if SHOW_DEBUG
            Object.Destroy(m_showPositionA);
            m_showPositionA = null;
            Object.Destroy(m_showPositionB);
            m_showPositionB = null;
            Object.Destroy(m_showPositionC);
            m_showPositionC = null;
            Object.Destroy(m_showPositionD);
            m_showPositionD = null;
#endif
        }

        public override void OnLeftController()
        {
#if SHOW_DEBUG
            DisableAllPositionPanels();
#endif
        }

#if SHOW_DEBUG
        private UIPanel CreatePositionPanel(UIView parent, string name)
        {
            var panel = parent.AddUIComponent<UIPanel>();

            var text = panel.AddUIComponent<UILabel>();
            text.transform.parent = panel.transform;
            text.transform.localPosition = Vector3.zero;
            text.relativePosition = new Vector3(5f, 5f);
            text.text = "(0, 0, 0)";

            panel.transform.parent = parent.transform;
            panel.transform.localPosition = Vector3.zero;
            panel.relativePosition = Vector3.zero;
            panel.backgroundSprite = "SubcategoriesPanel";
            panel.size = new Vector2(text.width+10, text.height+10);
            panel.opacity = 0.6f;

            return panel;
        }

        private void ChangeTextPosition(UIPanel panel, Vector3 pos)
        {
            var view = UIView.GetAView();
            var text = panel.GetComponentInChildren<UILabel>();
            text.text = string.Format("({0:0.##}, {1:0.##}, {2:0.##})", pos.x, pos.y, pos.z);
            panel.size = new Vector2(text.width + 10, text.height + 10);
            var newPosition = Camera.main.WorldToScreenPoint(pos) / view.inputScale;
            panel.relativePosition = view.ScreenPointToGUI(newPosition) - new Vector2(panel.width / 2.0f, panel.height / 2.0f);

        }

        private void EnableAllPositionPanels()
        {
            if (m_showPositionA != null && !m_showPositionA.isEnabled) 
                m_showPositionA.isEnabled = true;

            if (m_showPositionB != null && !m_showPositionB.isEnabled)
                m_showPositionB.isEnabled = true;

            if (m_showPositionC != null && !m_showPositionC.isEnabled)
                m_showPositionC.isEnabled = true;

            if (m_showPositionD != null && !m_showPositionD.isEnabled)
                m_showPositionD.isEnabled = true;
        }

        private void DisableAllPositionPanels()
        {
            if (m_showPositionA != null && m_showPositionA.isEnabled)
            {
                m_showPositionA.isEnabled = false;
                m_showPositionA.relativePosition = Vector3.zero;
            }

            if (m_showPositionB != null && m_showPositionB.isEnabled)
            {
                m_showPositionB.isEnabled = false;
                m_showPositionB.relativePosition = Vector3.zero;
            }

            if (m_showPositionC != null && m_showPositionC.isEnabled)
            {
                m_showPositionC.isEnabled = false;
                m_showPositionC.relativePosition = Vector3.zero;
            }

            if (m_showPositionD != null && m_showPositionD.isEnabled)
            {
                m_showPositionD.isEnabled = false;
                m_showPositionD.relativePosition = Vector3.zero;
            }
        }
#endif

#region Selection Behaviour

        public override ToolButton CreateButton(ToolButton.OnButtonPressedDelegate callback)
        {
            var selectionButton = GameObjectUtils.AddObjectWithComponent<SelectionButton>();

            if (callback != null)
                selectionButton.OnButtonPressed = isChecked => callback(isChecked);

            return selectionButton;
        }

        public override void OnLeftMouseIsDown(Vector3 mousePosition)
        {
            if (CurrentState == SelectionState.None)
            {
                m_currentMouseSelection = new Quad3(mousePosition, mousePosition, mousePosition, mousePosition);
                CurrentState = SelectionState.CreatingSelection;
            }
        }

        public override void OnLeftMouseIsPressed(Vector3 mousePosition)
        {
            if (CurrentState == SelectionState.CreatingSelection)
            {
                var selection = m_currentMouseSelection.Value;

                selection.c = mousePosition;

                var angle = Camera.main.transform.localEulerAngles.y * Mathf.Deg2Rad;
                var down = new Vector3(Mathf.Cos(angle), 0, -Mathf.Sin(angle));
                var right = new Vector3(-down.z, 0, down.x);

                var diagonal = selection.c - selection.a;
                var dotDown = Vector3.Dot(diagonal, down);
                var dotRight = Vector3.Dot(diagonal, right);

                if ((dotDown > 0 && dotRight > 0) || (dotDown <= 0 && dotRight <= 0))
                {
                    selection.b = selection.a + dotDown * down;
                    selection.d = selection.a + dotRight * right;
                }
                else
                {
                    selection.b = selection.a + dotRight * right;
                    selection.d = selection.a + dotDown * down;
                }

                m_currentMouseSelection = new Quad3(selection.a, selection.b, selection.c, selection.d);
            }
        }

        public override void OnLeftMouseIsUp(Vector3 mousePosition)
        {
            if (CurrentState == SelectionState.CreatingSelection)
            {
                CurrentState = SelectionState.SelectionCreated;
            }
        }

        public override void OnUpdate(Vector3 mousePosition)
        {
#if SHOW_DEBUG
            if (m_currentMouseSelection.HasValue)
            {
                EnableAllPositionPanels();

                var selection = m_currentMouseSelection.Value;

                ChangeTextPosition(m_showPositionA, selection.a);
                ChangeTextPosition(m_showPositionB, selection.b);
                ChangeTextPosition(m_showPositionC, selection.c);
                ChangeTextPosition(m_showPositionD, selection.d);
            }
#endif
        }

        public override void OnRenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (CurrentState != SelectionState.None)
            {
                var color = ColorConstants.SelectionColor;
                RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, color, m_currentMouseSelection.Value.a, 10f, -1f, 1280f, false, true);
                RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, color, m_currentMouseSelection.Value.c, 10f, -1f, 1280f, false, true);
                RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, color, m_currentMouseSelection.Value, -1f, 1280f, false, true);
            }
        }

#endregion
    }
}
