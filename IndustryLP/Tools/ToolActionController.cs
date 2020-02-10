using IndustryLP.UI;
using UnityEngine;

namespace IndustryLP.Tools
{
    internal abstract class ToolActionController
    {
        /// <summary>
        /// Create a new button
        /// </summary>
        /// <param name="callback">The callback when the button is pressed</param>
        /// <returns></returns>
        public abstract ToolButton CreateButton(ToolButton.OnButtonPressedDelegate callback);

        public virtual void OnStart(MainTool mainTool) { }

        public virtual void OnDestroy() { }

        public virtual void OnChangeController(ToolActionController oldController) { }

        public virtual void OnLeftController() { }

        /// <summary>
        /// Invoked when a new frame is going to be updated
        /// </summary>
        public virtual void OnUpdate(Vector3 mousePosition) { }

        /// <summary>
        /// Invoked when a new step in the simulation is going to happen
        /// </summary>
        public virtual void OnSimulationStep() { }
        
        /// <summary>
        /// Invoked when a new overlay effect is going to be updated 
        /// </summary>
        public virtual void OnRenderOverlay(RenderManager.CameraInfo cameraInfo) { }

        /// <summary>
        /// Invoked when the left mouse button is down
        /// </summary>
        public virtual void OnLeftMouseIsDown(Vector3 mousePosition) { }

        /// <summary>
        /// Invoked when the left mouse button is pressed
        /// </summary>
        public virtual void OnLeftMouseIsPressed(Vector3 mousePosition) { }

        /// <summary>
        /// Invoked when the left mouse button is up
        /// </summary>
        public virtual void OnLeftMouseIsUp(Vector3 mousePosition) { }

        /// <summary>
        /// Invoked when the right mouse button is down
        /// </summary>
        public virtual void OnRightMouseIsDown(Vector3 mousePosition) { }

        /// <summary>
        /// Invoked when the right mouse button is pressed
        /// </summary>
        public virtual void OnRightMouseIsPressed(Vector3 mousePosition) { }

        /// <summary>
        /// Invoked when the right mouse button is up
        /// </summary>
        public virtual void OnRightMouseIsUp(Vector3 mousePosition) { }
    }
}
