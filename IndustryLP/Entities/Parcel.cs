using static IndustryLP.Utils.MathUtils;

namespace IndustryLP.Entities
{
    public class Parcel : EntityPosition
    {
        public ushort GridId { get; set; }
        public float Rotation { get; set; }
        public BuildingInfo Building { get; set; }
    }
}
