using ColossalFramework.Math;
using ColossalFramework.UI;
using IndustryLP.Tools;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using IndustryLP.Utils.Enums;
using System;
using UnityEngine;

namespace IndustryLP.Actions
{
    /// <summary>
    /// Represents the zoning action
    /// </summary>
    internal class ZoningAction : ToolAction
    {
        #region Attributes

        private const float k_minDistante = 1f;

        private ZoningState m_state = ZoningState.None;
        private IMainTool m_mainTool = null;
        private Quad3? m_selection = null;
        private float? m_cameraAngle = null;

        GUIUtils.UITextDebug showSize = null;

#if DEBUG
        GUIUtils.UITextDebug debug_m_showPositionA = null;
        GUIUtils.UITextDebug debug_m_showPositionB = null;
        GUIUtils.UITextDebug debug_m_showPositionC = null;
        GUIUtils.UITextDebug debug_m_showPositionD = null;
#endif

        #endregion

        #region Properties

        /// <summary>
        /// The number of rows
        /// </summary>
        public int Rows
        {
            get
            {
                if (m_selection.HasValue)
                {
                    var quad = m_selection.Value;
                    var downDistance = Vector3.Distance(quad.a, quad.d);
                    return Math.Max(Convert.ToInt32(Math.Floor(downDistance / 40f + 0.5)), 1);
                }

                return 0;
            }
        }

        /// <summary>
        /// The number of columns
        /// </summary>
        public int Columns
        {
            get
            {
                if (m_selection.HasValue)
                {
                    var quad = m_selection.Value;
                    var rightDistance = Vector3.Distance(quad.a, quad.b);
                    return Math.Max(Convert.ToInt32(Math.Floor(rightDistance / 40f + 0.5)), 1);
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the quad selection
        /// </summary>
        public Quad3 Selection
        {
            get
            {
                if (m_selection.HasValue)
                {
                    var quad = new Quad3(m_selection.Value.a, m_selection.Value.b, m_selection.Value.c, m_selection.Value.d);

                    // Obtenemos las distancias reales y las aproximadas
                    var columns = Vector3.Distance(quad.a, quad.d) / 40f;
                    var roundedColumns = Math.Max(Math.Floor(columns + 0.5), 1);
                    var rows = Vector3.Distance(quad.a, quad.b) / 40f;
                    var roundedRows = Math.Max(Math.Floor(rows + 0.5), 1);

                    // Obtenemos los puntos clippeados
                    var b1 = Vector3.LerpUnclamped(quad.a, quad.b, Convert.ToSingle(roundedRows / rows));
                    var d1 = Vector3.LerpUnclamped(quad.a, quad.d, Convert.ToSingle(roundedColumns / columns));
                    var c1 = quad.a + (b1 - quad.a) + (d1 - quad.a);
                    quad.b = b1; quad.c = c1; quad.d = d1;

                    // Devolvemos el nuevo rectángulo
                    return quad;
                }

                return default;
            }
        }

        #endregion

        #region Action Behaviour Methods

        public override void OnStart(IMainTool mainTool)
        {
            m_mainTool = mainTool;

            showSize = GameObjectUtils.AddUIComponent<GUIUtils.UITextDebug>();
            showSize.Hide();

#if DEBUG
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

        public override void OnEnterController()
        {
            m_state = ZoningState.None;
            m_selection = null;
            m_cameraAngle = null;
            m_mainTool.CancelZoning();
        }

        public override void OnLeftController()
        {
            showSize.Hide();
#if DEBUG
            debug_m_showPositionA.Hide();
            debug_m_showPositionB.Hide();
            debug_m_showPositionC.Hide();
            debug_m_showPositionD.Hide();
#endif
        }

        public override void OnLeftMouseIsDown(Vector3 mousePosition)
        {
            var angle = Camera.main.transform.localEulerAngles.y * Mathf.Deg2Rad;
            m_cameraAngle = angle;

            var down = new Vector3(Mathf.Cos(angle), 0, -Mathf.Sin(angle));
            var right = new Vector3(-down.z, 0, down.x);

            m_selection = new Quad3(
                mousePosition + 20f * down + 20f * right,
                mousePosition - 20f * down + 20f * right,
                mousePosition - 20f * down - 20f * right,
                mousePosition + 20f * down - 20f * right
            );

            m_state = ZoningState.CreatingZone;
        }

        public override void OnLeftMouseIsPressed(Vector3 mousePosition)
        {
            if (m_state == ZoningState.CreatingZone)
            {
                var quad = m_selection.Value;
                var angle = m_cameraAngle.Value;

                var down = new Vector3(Mathf.Cos(angle), 0, -Mathf.Sin(angle));
                var right = new Vector3(-down.z, 0, down.x);

                var minC = quad.a - 40f * down - 40 * right;
                var minDiagonal = minC - quad.a;

                var diagonal = mousePosition - quad.a;

                if (diagonal.magnitude > minDiagonal.magnitude)
                {
                    quad.c = mousePosition;

                    var dotDown = Vector3.Dot(diagonal, down);
                    var dotRight = Vector3.Dot(diagonal, right);

                    quad.b = quad.a + dotDown * down;
                    quad.d = quad.a + dotRight * right;

                    m_selection = quad;
                }
            }
        }

        public override void OnLeftMouseIsUp(Vector3 mousePosition)
        {
            if (m_state == ZoningState.CreatingZone)
            {
                var quad = Selection;
                var angle = m_cameraAngle.Value;
                var diagonal = Vector3.Distance(quad.a, quad.c);

                if (diagonal > k_minDistante && Rows > 1 && Columns > 1)
                {
                    m_selection = quad;
                    m_state = ZoningState.ZoneCreated;
                    m_mainTool.DoZoning(m_selection.Value, angle);
                }
                else
                {
                    m_selection = null;
                    m_state = ZoningState.None;
                    m_mainTool.CancelZoning();
                }
            }
        }

        public override void OnRenderOverlay(RenderManager.CameraInfo cameraInfo, Vector3 mousePosition)
        {
            if (m_state != ZoningState.None)
            {
                var quad = Selection;
                var angle = m_cameraAngle.Value;
                var down = new Vector3(Mathf.Cos(angle), 0, -Mathf.Sin(angle));
                var right = new Vector3(-down.z, 0, down.x);
                var diagonal = quad.c - quad.a;
                var dotDown = Vector3.Dot(diagonal, down);
                var dotRight = Vector3.Dot(diagonal, right);

                if ((dotDown > 0 && dotRight > 0) || (dotDown <= 0 && dotRight <= 0))
                {
                    quad.b = quad.a + dotDown * down;
                    quad.d = quad.a + dotRight * right;
                }
                else
                {
                    quad.b = quad.a + dotRight * right;
                    quad.d = quad.a + dotDown * down;
                }

                if (diagonal.magnitude > k_minDistante)
                {
                    Color color = (Rows > LibraryConstants.MinRows && Columns > LibraryConstants.MinColumns) ? ColorConstants.SelectionColor : ColorConstants.BadSelectionColor;

                    RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, color, quad, -1f, 1280f, false, false);

                    if (!showSize.isVisible) showSize.Show();
                    showSize.SetText(string.Format("({0}, {1})", Rows, Columns));

                    var mainView = UIView.GetAView();
                    var midPoint = Vector3.Lerp(quad.a, quad.c, 0.5f);
                    var newPosition = Camera.main.WorldToScreenPoint(midPoint) / mainView.inputScale;
                    showSize.relativePosition = mainView.ScreenPointToGUI(newPosition) - new Vector2(showSize.width / 2f, showSize.height / 2f);

#if DEBUG
                    if (!debug_m_showPositionA.isVisible) debug_m_showPositionA.Show();
                    if (!debug_m_showPositionB.isVisible) debug_m_showPositionB.Show();
                    if (!debug_m_showPositionC.isVisible) debug_m_showPositionC.Show();
                    if (!debug_m_showPositionD.isVisible) debug_m_showPositionD.Show();

                    quad = Selection;

                    debug_m_showPositionA.SetText(string.Format("A ({0:0.##}, {1:0.##}, {2:0.##})", quad.a.x, quad.a.y, quad.a.z));
                    debug_m_showPositionB.SetText(string.Format("B ({0:0.##}, {1:0.##}, {2:0.##})", quad.b.x, quad.b.y, quad.b.z));
                    debug_m_showPositionC.SetText(string.Format("C ({0:0.##}, {1:0.##}, {2:0.##})", quad.c.x, quad.c.y, quad.c.z));
                    debug_m_showPositionD.SetText(string.Format("D ({0:0.##}, {1:0.##}, {2:0.##})", quad.d.x, quad.d.y, quad.d.z));

                    var newPositionA = Camera.main.WorldToScreenPoint(quad.a) / mainView.inputScale;
                    debug_m_showPositionA.relativePosition = mainView.ScreenPointToGUI(newPositionA) - new Vector2(debug_m_showPositionA.width / 2f, debug_m_showPositionA.height / 2f);

                    var newPositionB = Camera.main.WorldToScreenPoint(quad.b) / mainView.inputScale;
                    debug_m_showPositionB.relativePosition = mainView.ScreenPointToGUI(newPositionB) - new Vector2(debug_m_showPositionB.width / 2f, debug_m_showPositionB.height / 2f);

                    var newPositionC = Camera.main.WorldToScreenPoint(quad.c) / mainView.inputScale;
                    debug_m_showPositionC.relativePosition = mainView.ScreenPointToGUI(newPositionC) - new Vector2(debug_m_showPositionC.width / 2f, debug_m_showPositionC.height / 2f);

                    var newPositionD = Camera.main.WorldToScreenPoint(quad.d) / mainView.inputScale;
                    debug_m_showPositionD.relativePosition = mainView.ScreenPointToGUI(newPositionD) - new Vector2(debug_m_showPositionD.width / 2f, debug_m_showPositionD.height / 2f);
#endif
                }
            }
            else
            {
                var angle = Camera.main.transform.localEulerAngles.y * Mathf.Deg2Rad;
                var down = new Vector3(Mathf.Cos(angle), 0, -Mathf.Sin(angle));
                var right = new Vector3(-down.z, 0, down.x);

                var quad = new Quad3(
                    mousePosition + 20f * down + 20f * right,
                    mousePosition - 20f * down + 20f * right,
                    mousePosition - 20f * down - 20f * right,
                    mousePosition + 20f * down - 20f * right
                );

                RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, ColorConstants.PointerColor, quad, -1f, 1280f, false, false);

                if (showSize.isVisible) showSize.Hide();
#if DEBUG
                if (debug_m_showPositionA.isVisible) debug_m_showPositionA.Hide();
                if (debug_m_showPositionB.isVisible) debug_m_showPositionB.Hide();
                if (debug_m_showPositionC.isVisible) debug_m_showPositionC.Hide();
                if (debug_m_showPositionD.isVisible) debug_m_showPositionD.Hide();
#endif
            }
        }

        #endregion
    }
}
