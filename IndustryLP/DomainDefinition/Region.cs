namespace IndustryLP.DomainDefinition
{
    public class Region
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string[,] Parcels { get; set; }
    }
}
