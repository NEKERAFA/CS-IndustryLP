using ColossalFramework.Math;
using IndustryLP.Utils.Constants;
using IndustryLP.Utils.Enums;
using IndustryLP.UI;
using IndustryLP.UI.Buttons;
using IndustryLP.Utils;
using UnityEngine;
using ColossalFramework.UI;
using System.Reflection;
using System;

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

        internal Quad3? m_selection;
        internal float m_cameraAngle = 0;
        internal float m_rotation = 0;

        private Vector3? m_referenceVector;

        private MainTool m_mainTool;
        private GUIUtils.UITextDebug showSize;

#if DEBUG
        private GUIUtils.UITextDebug debug_m_showPositionA;
        private GUIUtils.UITextDebug debug_m_showPositionB;
        private GUIUtils.UITextDebug debug_m_showPositionC;
        private GUIUtils.UITextDebug debug_m_showPositionD;
#endif

        #endregion

        #region Properties

        private SelectionState CurrentState { get; set; } = SelectionState.None;

        /// <summary>
        /// The number of rows
        /// </summary>
        public int Rows
        {
            get
            {
                if (m_selection.HasValue)
                {
                    var downDistance = Vector3.Distance(m_selection.Value.a, m_selection.Value.d);
                    return Convert.ToInt32(Math.Floor(downDistance / 40f + 0.5));
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
                    var rightDistance = Vector3.Distance(m_selection.Value.a, m_selection.Value.b);
                    return Convert.ToInt32(Math.Floor(rightDistance / 40f + 0.5));
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
                    var quad = m_selection.Value;

                    // Obtenemos las distancias reales y las aproximadas
                    var columns = Vector3.Distance(quad.a, quad.d) / 40f;
                    var roundedColumns = Math.Floor(columns + 0.5);
                    var rows = Vector3.Distance(quad.a, quad.b) / 40f;
                    var roundedRows = Math.Floor(rows + 0.5);

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

        #region Controller Behaviour

        public override void OnStart(MainTool mainTool)
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

        public override void OnDestroy()
        {
            UnityEngine.Object.Destroy(showSize);
            showSize = null;
#if DEBUG
            UnityEngine.Object.Destroy(debug_m_showPositionA);
            debug_m_showPositionA = null;
            UnityEngine.Object.Destroy(debug_m_showPositionB);
            debug_m_showPositionB = null;
            UnityEngine.Object.Destroy(debug_m_showPositionC);
            debug_m_showPositionC = null;
            UnityEngine.Object.Destroy(debug_m_showPositionD);
            debug_m_showPositionD = null;
#endif
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
            if (CurrentState == SelectionState.None)
            {
                m_selection = new Quad3(mousePosition, mousePosition, mousePosition, mousePosition);
                m_cameraAngle = Camera.main.transform.localEulerAngles.y * Mathf.Deg2Rad;
                CurrentState = SelectionState.CreatingSelection;
            }
            else if (CurrentState == SelectionState.SelectionCreated)
            {
                var quad = m_selection.Value;
                var mouse = m_mainTool.GetTerrainMousePosition();
                var pressPoint = false;
                
                if (Vector3.Distance(quad.a, mouse) <= 10)
                {
                    pressPoint = true;
                    m_selection = new Quad3(quad.c, quad.b, quad.a, quad.d);
                }

                if (Vector3.Distance(quad.c, mouse) <= 10)
                {
                    pressPoint = true;
                }

                if (Vector3.Distance(quad.b, mouse) <= 10)
                {
                    pressPoint = true;
                    m_selection = new Quad3(quad.d, quad.c, quad.b, quad.a);
                }

                if (Vector3.Distance(quad.d, mouse) <= 10)
                {
                    pressPoint = true;
                    m_selection = new Quad3(quad.b, quad.a, quad.d, quad.c);
                }

                if (pressPoint)
                {
                    CurrentState = SelectionState.MovingPointSelection;
                }

                var midPoint = Vector3.Lerp(quad.a, quad.c, 0.5f);
                if (Vector3.Distance(midPoint, mouse) <= 10)
                {
                    CurrentState = SelectionState.MovingSelection;
                }
            }
        }

        public override void OnRightMouseIsDown(Vector3 mousePosition)
        {
            if (CurrentState == SelectionState.SelectionCreated)
            {
                CurrentState = SelectionState.RotatingSelection;

                var quad = m_selection.Value;
                var midPoint = Vector3.Lerp(quad.a, quad.c, 0.5f);

                m_referenceVector = mousePosition - midPoint;
            }
        }

        public override void OnLeftMouseIsPressed(Vector3 mousePosition)
        {
            if (CurrentState == SelectionState.CreatingSelection || CurrentState == SelectionState.MovingPointSelection)
            {
                var quad = m_selection.Value;

                quad.c = mousePosition;

                var down = new Vector3(Mathf.Cos(m_cameraAngle), 0, -Mathf.Sin(m_cameraAngle));
                var right = new Vector3(-down.z, 0, down.x);

                var diagonal = quad.c - quad.a;
                var dotDown = Vector3.Dot(diagonal, down);
                var dotRight = Vector3.Dot(diagonal, right);

                quad.b = quad.a + dotRight * right;
                quad.d = quad.a + dotDown * down;

                m_selection = new Quad3(quad.a, quad.b, quad.c, quad.d);
            }
            else if (CurrentState == SelectionState.MovingSelection)
            {
                var quad = m_selection.Value;
                var midPoint = Vector3.Lerp(quad.a, quad.c, 0.5f);

                var delta = mousePosition - midPoint;

                quad.a = quad.a + delta;
                quad.b = quad.b + delta;
                quad.c = quad.c + delta;
                quad.d = quad.d + delta;

                m_selection = quad;
            }
        }

        public override void OnRightMouseIsPressed(Vector3 mousePosition)
        {
            if (CurrentState == SelectionState.RotatingSelection)
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
            if (CurrentState == SelectionState.CreatingSelection || CurrentState == SelectionState.MovingPointSelection)
            {
                m_selection = Selection;

                var diagonal = Vector3.Distance(m_selection.Value.a, m_selection.Value.c);

                if (diagonal > kMinDistance && Rows > 1 && Columns > 1)
                {
                    CurrentState = SelectionState.SelectionCreated;
                    m_mainTool.FinishSelection();
                }
                else
                {
                    CurrentState = SelectionState.None;
                    m_selection = null;
                    m_mainTool.CancelSelection();
                }
            }
            else if (CurrentState == SelectionState.MovingSelection)
            {
                CurrentState = SelectionState.SelectionCreated;
            }
        }

        public override void OnRightMouseIsUp(Vector3 mousePosition)
        {
            if (CurrentState == SelectionState.RotatingSelection)
            {
                m_selection = ProyectToTerrain(Selection);
                LoggerUtils.Log(m_rotation);
                LoggerUtils.Log(m_cameraAngle);
                m_cameraAngle += m_rotation * Mathf.Deg2Rad;
                m_rotation = 0;
                CurrentState = SelectionState.SelectionCreated;
            }
        }

        public override void OnRenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (CurrentState != SelectionState.None)
            {
                var quad = Selection;

                var diagonal = Vector3.Distance(quad.a, quad.c);

                if (diagonal > kMinDistance)
                {
                    Color32 color;
                    if (Rows > 1 && Columns > 1)
                        if (CurrentState == SelectionState.CreatingSelection)
                            color = ColorConstants.SelectionColor;
                        else
                            color = ColorConstants.SelectedColor;
                    else
                        color = ColorConstants.BadSelectionColor;

                    NetUtils.RenderRoadGrid(cameraInfo, quad, Rows, Columns, color);

                    if (CurrentState == SelectionState.SelectionCreated || CurrentState == SelectionState.MovingPointSelection)
                    {
                        var mouse = m_mainTool.GetTerrainMousePosition();
                        RenderPoints(cameraInfo, quad, mouse);
                    }

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

        #region Utils

        /// <summary>
        /// Renderiza los puntos del cuadrado
        /// </summary>
        /// <param name="cameraInfo"></param>
        /// <param name="quad"></param>
        /// <param name="mouse"></param>
        private void RenderPoints(RenderManager.CameraInfo cameraInfo, Quad3 quad, Vector3 mouse)
        {
            var size = Vector3.Distance(quad.a, mouse) <= 10 ? 20 : 10;
            RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.PointerColor, quad.a, size, -1, 1000, true, true);

            size = Vector3.Distance(quad.b, mouse) <= 10 ? 20 : 10;
            RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.PointerColor, quad.b, size, -1, 1000, true, true);

            size = Vector3.Distance(quad.c, mouse) <= 10 ? 20 : 10;
            RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.PointerColor, quad.c, size, -1, 1000, true, true);

            size = Vector3.Distance(quad.d, mouse) <= 10 ? 20 : 10;
            RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.PointerColor, quad.d, size, -1, 1000, true, true);
            
            var midPoint = Vector3.Lerp(quad.a, quad.c, 0.5f);
            size = Vector3.Distance(midPoint, mouse) <= 10 ? 20 : 10;
            RenderManager.instance.OverlayEffect.DrawCircle(cameraInfo, ColorConstants.PointerColor, midPoint, size, -1, 1000, true, true);
        }

        /// <summary>
        /// Rota un quad
        /// </summary>
        /// <param name="quad"></param>
        /// <param name="rotation"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public Quad3 RotateQuad(Quad3 quad, float rotation, Vector3 axis)
        {
            var midPoint = Vector3.Lerp(quad.a, quad.c, 0.5f);
            var rotate = Quaternion.AngleAxis(rotation, Vector3.up);

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
        
        #endregion
    }
}
