using UnityEngine;

namespace IndustryLP.Utils.Wrappers
{
    /// <summary>
    /// This class wrappers about a zone parcel in in Cities: Skylines
    /// </summary>
    public class ParcelWrapper
    {
        /// <summary>
        /// The id in the grid distribution
        /// </summary>
        public ushort GridId { get; set; }

        /// <summary>
        /// The position of the parcel
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The rotation of the parcel
        /// </summary>
        public float Rotation { get; set; }
    }
}
