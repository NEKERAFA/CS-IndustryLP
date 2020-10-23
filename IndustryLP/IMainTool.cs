using ColossalFramework.Math;
using UnityEngine;

namespace IndustryLP
{
    /// <summary>
    /// Represents a interface to comunicate actions with main tool
    /// </summary>
    internal interface IMainTool
    {
        Quad3 Selection { get; set; }
        float SelectionAngle { get; set; }

        bool GetColisingWithTerrain(Ray ray, out Vector3 output);
        void CancelZoning();
        void DoZoning(Quad3 zone);
    }
}
