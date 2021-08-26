namespace IndustryLP.DomainDefinition
{
    internal class Region
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public string[,] Parcels { get; set; }
    }
}
