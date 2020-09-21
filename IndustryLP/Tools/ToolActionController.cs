using IndustryLP.UI;
using UnityEngine;

namespace IndustryLP.Tools
{
    internal abstract class ToolActionController
    {
        /// <summary>
        /// Invoked when the main tool is going to create a button of this controller
        /// </summary>
        /// <param name="callback">The callback when the button is pressed</param>
        /// <returns></returns>
        public abstract ToolButton CreateButton(ToolButton.OnButtonClickedDelegate callback);

        /// <summary>
        /// Invoked the first time that the main tool creates the controller
        /// </summary>
        /// <param name="mainTool"></param>
        public virtual void OnStart(MainTool mainTool) { }

        /// <summary>
        /// Invoked when the main tool is going to be destroyed
        /// </summary>
        public virtual void OnDestroy() { }

        /// <summary>
        /// Invoked when the main tool changes the current controller (user press in another button or the tool lose the control)
        /// </summary>
        /// <param name="oldController"></param>
        public virtual void OnChangeController(ToolActionController oldController) { }

        /// <summary>
        /// Invoked when the controller is going to set the current controller
        /// </summary>
        public virtual void OnEnterController() { }

        /// <summary>
        /// Invoked when the controller will be change.
        /// </summary>
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
        /// Invoked when a new geometry effect is going to be updated 
        /// </summary>
        public virtual void OnRenderGeometry(RenderManager.CameraInfo cameraInfo) { }

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
