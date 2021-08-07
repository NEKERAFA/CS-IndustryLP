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
    internal class MovingZoneAction : ToolAction
    {
        #region Attributes

        private const float k_minDistante = 1f;

        private ZoningState m_state = ZoningState.None;
        private IMainTool m_mainTool = null;
        private Quad3? m_selection = null;
        private float? m_cameraAngle = 0;
        private float m_rotation = 0;
        private Vector3? m_referenceVector;

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

                    // Rotamos el cuadrado
                    quad = RotateQuad(quad, m_rotation, Vector3.up);

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

        #region Action Behaviour method

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
            m_selection = m_mainTool.Selection;
            m_cameraAngle = m_mainTool.SelectionAngle;
        }

        public override void OnLeftController()
        {
#if DEBUG
            debug_m_showPositionA.Hide();
            debug_m_showPositionB.Hide();
            debug_m_showPositionC.Hide();
            debug_m_showPositionD.Hide();
#endif
        }

        public override void OnLeftMouseIsDown(Vector3 mousePosition)
        {
            if (m_state == ZoningState.None)
            {
                var quad = m_selection.Value;
                var pressPoint = false;

                if (Vector3.Distance(quad.a, mousePosition) <= 10)
                {
                    pressPoint = true;
                    m_selection = new Quad3(quad.c, quad.b, quad.a, quad.d);
                }

                if (Vector3.Distance(quad.c, mousePosition) <= 10)
                {
                    pressPoint = true;
                }

                if (Vector3.Distance(quad.b, mousePosition) <= 10)
                {
                    pressPoint = true;
                    m_selection = new Quad3(quad.d, quad.c, quad.b, quad.a);
                }

                if (Vector3.Distance(quad.d, mousePosition) <= 10)
                {
                    pressPoint = true;
                    m_selection = new Quad3(quad.b, quad.a, quad.d, quad.c);
                }

                if (pressPoint)
                {
                    m_state = ZoningState.MovingVertex;
                }
                else
                {
                    var midPoint = Vector3.Lerp(quad.a, quad.c, 0.5f);
                    if (Vector3.Distance(midPoint, mousePosition) <= 10)
                    {
                        m_state = ZoningState.MovingZone;
                    }
                }
            }
        }

        public override void OnRightMouseIsDown(Vector3 mousePosition)
        {
            if (m_state == ZoningState.None)
            {
                m_state = ZoningState.RotatingZone;

                var quad = m_selection.Value;
                var midPoint = Vector3.Lerp(quad.a, quad.c, 0.5f);

                m_referenceVector = mousePosition - midPoint;
            }
        }

        public override void OnLeftMouseIsPressed(Vector3 mousePosition)
        {
            if (m_state == ZoningState.MovingVertex)
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
            else if (m_state == ZoningState.MovingZone)
            {
                var quad = m_selection.Value;
                var midPoint = Vector3.Lerp(quad.a, quad.c, 0.5f);

                var delta = mousePosition - midPoint;

                quad.a += delta;
                quad.b += delta;
                quad.c += delta;
                quad.d += delta;

                m_selection = quad;
            }
        }

        public override void OnRightMouseIsPressed(Vector3 mousePosition)
        {
            if (m_state == ZoningState.RotatingZone)
            {
                var quad = m_selection.Value;
                var midPoint = Vector3.Lerp(quad.a, quad.c, 0.5f);

                var currentVector = mousePosition - midPoint;
                var angle = Vector3.Angle(m_referenceVector.Value, currentVector);
                var sign = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(m_referenceVector.Value, currentVector)));

                m_rotation = angle * sign;
            }
        }

        public override void OnLeftMouseIsUp(Vector3 mousePosition)
        {
            var angle = m_cameraAngle.Value;

            if (m_state == ZoningState.MovingVertex)
            {
                m_selection = ProyectToTerrain(Selection);

                var diagonal = Vector3.Distance(m_selection.Value.a, m_selection.Value.c);

                m_state = ZoningState.None;
                if (diagonal > k_minDistante && Rows > 1 && Columns > 1)
                {
                    m_mainTool.DoZoning(m_selection.Value, angle);
                }
                else
                {
                    m_mainTool.CancelZoning();
                }
            }
            else if (m_state == ZoningState.MovingZone)
            {
                m_state = ZoningState.None;
                m_mainTool.DoZoning(m_selection.Value, angle);
            }
        }

        public override void OnRightMouseIsUp(Vector3 mousePosition)
        {
            if (m_state == ZoningState.RotatingZone)
            {
                m_selection = ProyectToTerrain(Selection);
                m_cameraAngle += m_rotation * Mathf.Deg2Rad;
                m_rotation = 0;
                m_state = ZoningState.None;
                m_mainTool.DoZoning(m_selection.Value, m_cameraAngle.Value);
            }
        }

        public override void OnRenderOverlay(RenderManager.CameraInfo cameraInfo, Vector3 mousePosition)
        {
            var quad = Selection;
            var angle = m_cameraAngle.Value + m_rotation * Mathf.Deg2Rad;
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
                Color32 color;
                if (Rows > LibraryConstants.MinRows && Columns > LibraryConstants.MinColumns)
                    color = ColorConstants.SelectedColor;
                else
                    color = ColorConstants.BadSelectionColor;

                RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, color, quad, -1f, 1280f, false, false);
                RenderPoints(cameraInfo, quad, mousePosition);

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

        #endregion

        #region Private methods

        /// <summary>
        /// Renderiza los puntos del cuadrado
        /// </summary>
        /// <param name="cameraInfo"></param>
        /// <param name="quad"></param>
        /// <param name="mouse"></param>
        private void RenderPoints(RenderManager.CameraInfo cameraInfo, Quad3 quad, Vector3 mouse)
        {
            var size = Vector3.Distance(quad.a, mouse) <= 10f ? 20f : 10f;
            RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.PointerColor, quad.a, size, -1f, 1280f, false, true);

            size = Vector3.Distance(quad.b, mouse) <= 10f ? 20f : 10f;
            RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.PointerColor, quad.b, size, -1, 1280f, false, true);

            size = Vector3.Distance(quad.c, mouse) <= 10f ? 20f : 10f;
            RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.PointerColor, quad.c, size, -1, 1280f, false, true);

            size = Vector3.Distance(quad.d, mouse) <= 10f ? 20f : 10f;
            RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.PointerColor, quad.d, size, -1, 1280f, false, true);

            var midPoint = Vector3.Lerp(quad.a, quad.c, 0.5f);
            size = Vector3.Distance(midPoint, mouse) <= 10 ? 20 : 10;
            RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.PointerColor, midPoint, size, -1, 1000, true, true);
        }

        /// <summary>
        /// Obtiene la proyección del quad con el terreno
        /// </summary>
        /// <param name="quad"></param>
        /// <returns></returns>
        private Quad3 ProyectToTerrain(Quad3 quad)
        {
            var raycast = new Ray(quad.a, Vector3.up);
            if (!m_mainTool.GetColisingWithTerrain(raycast, out Vector3 a1))
            {
                raycast = new Ray(quad.a, Vector3.down);
                m_mainTool.GetColisingWithTerrain(raycast, out a1);
            }

            raycast = new Ray(quad.b, Vector3.up);
            if (!m_mainTool.GetColisingWithTerrain(raycast, out Vector3 b1))
            {
                raycast = new Ray(quad.b, Vector3.down);
                m_mainTool.GetColisingWithTerrain(raycast, out b1);
            }

            raycast = new Ray(quad.c, Vector3.up);
            if (!m_mainTool.GetColisingWithTerrain(raycast, out Vector3 c1))
            {
                raycast = new Ray(quad.c, Vector3.down);
                m_mainTool.GetColisingWithTerrain(raycast, out c1);
            }

            raycast = new Ray(quad.d, Vector3.up);
            if (!m_mainTool.GetColisingWithTerrain(raycast, out Vector3 d1))
            {
                raycast = new Ray(quad.d, Vector3.down);
                m_mainTool.GetColisingWithTerrain(raycast, out d1);
            }

            return new Quad3(a1, b1, c1, d1);
        }


        /// <summary>
        /// Rota un quad
        /// </summary>
        /// <param name="quad"></param>
        /// <param name="rotation"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        private Quad3 RotateQuad(Quad3 quad, float rotation, Vector3 axis)
        {
            var midPoint = Vector3.Lerp(quad.a, quad.c, 0.5f);
            var rotate = Quaternion.AngleAxis(rotation, axis);

            var dirA = quad.a - midPoint;
            var a1 = rotate * dirA + midPoint;
            var dirB = quad.b - midPoint;
            var b1 = rotate * dirB + midPoint;
            var dirC = quad.c - midPoint;
            var c1 = rotate * dirC + midPoint;
            var dirD = quad.d - midPoint;
            var d1 = rotate * dirD + midPoint;

            return new Quad3(a1, b1, c1, d1);
        }

        #endregion
    }
}
