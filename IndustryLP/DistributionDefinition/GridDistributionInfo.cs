using IndustryLP.Entities;
using IndustryLP.Utils.Enums;
using IndustryLP.Utils.Wrappers;
using System;
using System.Linq;
using UnityEngine;

namespace IndustryLP.DistributionDefinition
{
    internal class GridDistributionInfo : DistributionInfo
    {
        public override ParcelWrapper FindById(ushort gridId)
        {
            return Parcels.FirstOrDefault(parcel => parcel.GridId == gridId);
        }

        public override ParcelWrapper GetNext(CellNeighbour direction, ushort gridId)
        {
            var pos = GetGridPosition(gridId);

            if (direction == CellNeighbour.UP && pos.First < Rows)
            {
                return FindById(Convert.ToUInt16(gridId + Columns));
            }

            if (direction == CellNeighbour.DOWN && pos.First > 0)
            {
                return FindById(Convert.ToUInt16(gridId - Columns));
            }

            if (direction == CellNeighbour.RIGHT && pos.Second < Columns)
            {
                return FindById(Convert.ToUInt16(gridId + 1));
            }

            if (direction == CellNeighbour.LEFT && pos.Second > 0)
            {
                return FindById(Convert.ToUInt16(gridId - 1));
            }

            return null;
        }

        public override Tuple<int> GetGridPosition(ushort gridId)
        {
            int row = Convert.ToInt32(Mathf.Floor(Convert.ToSingle(gridId) / Convert.ToSingle(Columns))); 
            int column = gridId % Columns;

            return new Tuple<int>(row, column);
        }

        public override ushort GetId(int row, int column)
        {
            return Convert.ToUInt16(column + row * Columns);
        }
    }
}
