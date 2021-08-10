using UnityEngine;
using static IndustryLP.Utils.MathUtils;

namespace IndustryLP.Entities
{
    public class Cell : EntityPosition
    {
        public float Rotation { get; set; }
        public BuildingInfo Building { get; set; }
    }
}
