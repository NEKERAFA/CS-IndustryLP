using IndustryLP.Utils.Enums;
using IndustryLP.Utils.Wrappers;

namespace IndustryLP.DistributionDefinition
{
    internal class GridDistributionInfo : DistributionInfo
    {
        public int Rows { get; set; }

        public int Columns { get; set; }

        private int FindByGridId(ushort gridId)
        {
            int pos = 0;
            foreach (var cell in Cells)
            {
                if (cell.GridId == gridId)
                {
                    return pos;
                }

                pos++;
            }

            return -1;
        }

        public override ParcelWrapper GetNext(CellNeighbour direction, ushort gridId)
        {
            int pos = FindByGridId(gridId);

            int row = pos / Columns;
            int column = pos % Columns;

            if (pos >= 0)
            {
                if (direction == CellNeighbour.UP && row < Rows)
                {
                    return Cells[pos + Rows];
                }

                if (direction == CellNeighbour.DOWN && row > 0)
                {
                    return Cells[pos - Columns];
                }

                if (direction == CellNeighbour.RIGHT && column < Columns)
                {
                    return Cells[pos + 1];
                }

                if (direction == CellNeighbour.LEFT && column > 0)
                {
                    return Cells[pos - 1];
                }
            }

            return null;
        }
    }
}
