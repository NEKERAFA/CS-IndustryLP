using System.Collections.Generic;
using UnityEngine;

namespace IndustryLP.Utils
{
    public static class MathUtils
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
                    var distance = Vector2.Distance(new Vector2(position.x, position.z), new Vector3(entity.Position.x, entity.Position.z));

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

        public static List<T> GetRandom<T>(this List<T> list, int range = -1)
        {
            var chosen = new List<T>();
            var current = new List<T>(list);
            var rnd = new System.Random();

            if (range < 0 || range > list.Count)
            {
                range = list.Count;
            }

            for (int items = 0; items < range; items++)
            {
                int index = rnd.Next(current.Count);
                var item = current[index];
                current.RemoveAt(index);
                chosen.Add(item);
            }

            return chosen;
        }
    }
}
