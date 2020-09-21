namespace IndustryLP.DomainDefinition
{
    internal class Region
    {
        public enum Parcel
        {
            None, Forest, Farm, AgriculturalProcessing, ForestryProcessing, BigFactory, Cargoyard, OilExtractor, OilProcesssing
        }

        public int Rows { get; set; }
        public int Cols { get; set; }
        public Parcel[,] Parcels { get; set; }
    }
}
