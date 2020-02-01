using ColossalFramework.Math;
using IndustryLP.Enums;
using IndustryLP.UI;
using IndustryLP.UI.Buttons;
using IndustryLP.Utils;
using UnityEngine;

namespace IndustryLP.Tools
{
    internal class SelectionTool : ToolActionController
    {
        #region Attributes

        internal Quad3? m_currentMouseSelection;

        #endregion

        #region Properties

        private SelectionState CurrentState { get; set; } = SelectionState.None;

        #endregion

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

        public override void OnRenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (CurrentState != SelectionState.None)
            {
                var selection = new Color32(255, 128, 0, 255);
                RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, selection, m_currentMouseSelection.Value.a, 10f, -1f, 1280f, false, true);
                RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, selection, m_currentMouseSelection.Value.c, 10f, -1f, 1280f, false, true);
                RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, selection, m_currentMouseSelection.Value, -1f, 1280f, false, true);
            }
        }

        #endregion
    }
}
