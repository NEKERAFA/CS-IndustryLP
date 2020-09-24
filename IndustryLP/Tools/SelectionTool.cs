using ColossalFramework.Math;
using IndustryLP.Utils.Constants;
using IndustryLP.Utils.Enums;
using IndustryLP.UI;
using IndustryLP.UI.Buttons;
using IndustryLP.Utils;
using UnityEngine;
using ColossalFramework.UI;
using System.Reflection;

namespace IndustryLP.Tools
{
    /// <summary>
    /// Represents the selection tool
    /// </summary>
    internal class SelectionTool : ToolActionController
    {
        #region Attributes

        /// <summary>
        /// Minimun distance
        /// </summary>
        private static float kMinDistance = 1f;

        private static readonly MethodInfo RenderSegment = typeof(NetTool).GetMethod("RenderSegment", BindingFlags.NonPublic | BindingFlags.Static);

        internal Quad3? m_currentMouseSelection;
        internal float? m_angle;

        private MainTool m_mainTool;

#if DEBUG
        private GUIUtils.UITextDebug debug_m_showSize;
        private GUIUtils.UITextDebug debug_m_showPositionA;
        private GUIUtils.UITextDebug debug_m_showPositionB;
        private GUIUtils.UITextDebug debug_m_showPositionC;
        private GUIUtils.UITextDebug debug_m_showPositionD;
#endif

        #endregion

        #region Properties

        private SelectionState CurrentState { get; set; } = SelectionState.None;

        #endregion

        #region Controller Behaviour

        public override void OnStart(MainTool mainTool)
        {
            m_mainTool = mainTool;

#if DEBUG
            debug_m_showSize = GameObjectUtils.AddUIComponent<GUIUtils.UITextDebug>();
            debug_m_showSize.Hide();
            debug_m_showPositionA = GameObjectUtils.AddUIComponent<GUIUtils.UITextDebug>();
            debug_m_showPositionA.Hide();
            debug_m_showPositionB = GameObjectUtils.AddUIComponent<GUIUtils.UITextDebug>();
            debug_m_showPositionB.Hide();
            debug_m_showPositionC = GameObjectUtils.AddUIComponent<GUIUtils.UITextDebug>();
            debug_m_showPositionC.Hide();
            debug_m_showPositionD = GameObjectUtils.AddUIComponent<GUIUtils.UITextDebug>();
            debug_m_showPositionD.Hide();
#endif

        }

#if DEBUG
        public override void OnDestroy()
        {
            Object.Destroy(debug_m_showSize);
            debug_m_showSize = null;
            Object.Destroy(debug_m_showPositionA);
            debug_m_showPositionA = null;
            Object.Destroy(debug_m_showPositionB);
            debug_m_showPositionB = null;
            Object.Destroy(debug_m_showPositionC);
            debug_m_showPositionC = null;
            Object.Destroy(debug_m_showPositionD);
            debug_m_showPositionD = null;
        }

        public override void OnLeftController()
        {
            debug_m_showSize.Hide();
            debug_m_showPositionA.Hide();
            debug_m_showPositionB.Hide();
            debug_m_showPositionC.Hide();
            debug_m_showPositionD.Hide();
        }
#endif

        public override void OnLeftMouseIsDown(Vector3 mousePosition)
        {
            if (CurrentState == SelectionState.None)
            {
                m_currentMouseSelection = new Quad3(mousePosition, mousePosition, mousePosition, mousePosition);
                m_angle = Camera.main.transform.localEulerAngles.y * Mathf.Deg2Rad;
                CurrentState = SelectionState.CreatingSelection;
            }
        }

        public override void OnLeftMouseIsPressed(Vector3 mousePosition)
        {
            if (CurrentState == SelectionState.CreatingSelection)
            {
                var selection = m_currentMouseSelection.Value;

                selection.c = mousePosition;

                var down = new Vector3(Mathf.Cos(m_angle.Value), 0, -Mathf.Sin(m_angle.Value));
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
                var diagonal = Vector3.Distance(m_currentMouseSelection.Value.a, m_currentMouseSelection.Value.c);
                var rowLength = Vector3.Distance(m_currentMouseSelection.Value.a, m_currentMouseSelection.Value.b);
                var columnLength = Vector3.Distance(m_currentMouseSelection.Value.a, m_currentMouseSelection.Value.d);

                var rows = System.Convert.ToInt32(System.Math.Floor(rowLength / 40f));
                var cols = System.Convert.ToInt32(System.Math.Floor(columnLength / 40f));

                if (diagonal > kMinDistance && rows > 1 && cols > 1)
                {
                    CurrentState = SelectionState.SelectionCreated;
                    m_mainTool.FinishSelection();
                }
                else
                {
                    CurrentState = SelectionState.None;
                    m_currentMouseSelection = null;
                    m_mainTool.CancelSelection();
                }
            }
        }

        public override void OnRenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (CurrentState != SelectionState.None)
            {
                var diagonal = Vector3.Distance(m_currentMouseSelection.Value.a, m_currentMouseSelection.Value.c);

                if (diagonal > kMinDistance)
                {
                    var rowLength = Vector3.Distance(m_currentMouseSelection.Value.a, m_currentMouseSelection.Value.d);
                    var columnLength = Vector3.Distance(m_currentMouseSelection.Value.a, m_currentMouseSelection.Value.b);
                    var diagonalLength = Vector3.Distance(m_currentMouseSelection.Value.a, m_currentMouseSelection.Value.c);

                    var rows = System.Convert.ToInt32(System.Math.Floor(rowLength / 40f));
                    var cols = System.Convert.ToInt32(System.Math.Floor(columnLength / 40f));

                    Color32 color;
                    if (rows > 1 && cols > 1)
                        color = ColorConstants.SelectionColor;
                    else
                        color = ColorConstants.BadSelectionColor;

                    NetUtils.RenderRoadGrid(cameraInfo, m_currentMouseSelection.Value, rows, cols, color);

#if DEBUG
                    if (!debug_m_showSize.isVisible) debug_m_showSize.Show();
                    if (!debug_m_showPositionA.isVisible) debug_m_showPositionA.Show();
                    if (!debug_m_showPositionB.isVisible) debug_m_showPositionB.Show();
                    if (!debug_m_showPositionC.isVisible) debug_m_showPositionC.Show();
                    if (!debug_m_showPositionD.isVisible) debug_m_showPositionD.Show();

                    debug_m_showSize.SetText(string.Format("({0}, {1}) {2}", rows, cols, diagonalLength));
                    debug_m_showPositionA.SetText(string.Format("A ({0:0.##}, {1:0.##}, {2:0.##})", m_currentMouseSelection.Value.a.x, m_currentMouseSelection.Value.a.y, m_currentMouseSelection.Value.a.z));
                    debug_m_showPositionB.SetText(string.Format("B ({0:0.##}, {1:0.##}, {2:0.##})", m_currentMouseSelection.Value.b.x, m_currentMouseSelection.Value.b.y, m_currentMouseSelection.Value.b.z));
                    debug_m_showPositionC.SetText(string.Format("C ({0:0.##}, {1:0.##}, {2:0.##})", m_currentMouseSelection.Value.c.x, m_currentMouseSelection.Value.c.y, m_currentMouseSelection.Value.c.z));
                    debug_m_showPositionD.SetText(string.Format("D ({0:0.##}, {1:0.##}, {2:0.##})", m_currentMouseSelection.Value.d.x, m_currentMouseSelection.Value.d.y, m_currentMouseSelection.Value.d.z));

                    var mainView = UIView.GetAView();
                    var midPoint = Vector3.Lerp(m_currentMouseSelection.Value.a, m_currentMouseSelection.Value.c, 0.5f);
                    var newPosition = Camera.main.WorldToScreenPoint(midPoint) / mainView.inputScale;
                    debug_m_showSize.relativePosition = mainView.ScreenPointToGUI(newPosition) - new Vector2(debug_m_showSize.width / 2f, debug_m_showSize.height / 2f);

                    var newPositionA = Camera.main.WorldToScreenPoint(m_currentMouseSelection.Value.a) / mainView.inputScale;
                    debug_m_showPositionA.relativePosition = mainView.ScreenPointToGUI(newPositionA) - new Vector2(debug_m_showPositionA.width / 2f, debug_m_showPositionA.height / 2f);

                    var newPositionB = Camera.main.WorldToScreenPoint(m_currentMouseSelection.Value.b) / mainView.inputScale;
                    debug_m_showPositionB.relativePosition = mainView.ScreenPointToGUI(newPositionB) - new Vector2(debug_m_showPositionB.width / 2f, debug_m_showPositionB.height / 2f);

                    var newPositionC = Camera.main.WorldToScreenPoint(m_currentMouseSelection.Value.c) / mainView.inputScale;
                    debug_m_showPositionC.relativePosition = mainView.ScreenPointToGUI(newPositionC) - new Vector2(debug_m_showPositionC.width / 2f, debug_m_showPositionC.height / 2f);

                    var newPositionD = Camera.main.WorldToScreenPoint(m_currentMouseSelection.Value.d) / mainView.inputScale;
                    debug_m_showPositionD.relativePosition = mainView.ScreenPointToGUI(newPositionD) - new Vector2(debug_m_showPositionD.width / 2f, debug_m_showPositionD.height / 2f);
#endif
                    }
            }
        }

        #endregion

        #region Selection Behaviour

        public override ToolButton CreateButton(ToolButton.OnButtonClickedDelegate callback)
        {
            var selectionButton = GameObjectUtils.AddObjectWithComponent<SelectionButton>();

            if (callback != null)
                selectionButton.OnButtonClicked = isChecked => callback(isChecked);

            return selectionButton;
        }

        #endregion
    }
}
