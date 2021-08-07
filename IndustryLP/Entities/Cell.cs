using UnityEngine;

namespace IndustryLP.Entities
{
    public class Cell
    {
        public Vector3 Position { get; set; }
        public float Rotation { get; set; }
        public BuildingInfo Building { get; set; }
    }
}
