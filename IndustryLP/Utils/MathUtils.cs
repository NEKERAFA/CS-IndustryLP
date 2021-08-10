using System.Collections.Generic;
using UnityEngine;

namespace IndustryLP.Utils
{
    public class MathUtils
    {
        public abstract class EntityPosition
        {
            public Vector3 Position { get; set; }
        }

        public static T FindNeighbour<T>(IEnumerable<T> entities, Vector3 position, double? minDistance) where T : EntityPosition
        {
            float currentDistance = float.MaxValue;
            T selectedEntity = null;

            foreach (var entity in entities)
            {
                if (entity != null)
                {
                    var distance = Vector3.Distance(position, entity.Position);

                    if ((minDistance == null) || (minDistance != null && distance <= minDistance))
                    {
                        if (distance < currentDistance)
                        {
                            selectedEntity = entity;
                            currentDistance = distance;
                        }
                    }
                }
            }

            return selectedEntity;
        }
    }
}
