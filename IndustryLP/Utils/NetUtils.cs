using ColossalFramework;
using IndustryLP.Utils.Wrappers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndustryLP.Utils
{
    /// <summary>
    /// This class has methods that abstracts some operations about roads creation 
    /// </summary>
    internal static class NetUtils
    {
        public class NetCreationException : Exception
        {
            public NetCreationException(string message) : base(message) { }
        }

        private static uint GetNewBuildIndex()
        {
            // Gets managers
            var simulationManager = Singleton<SimulationManager>.instance;
            // Get fresh build index
            uint newBuildIndex = simulationManager.m_currentBuildIndex++;
            // Set new build index
            simulationManager.m_currentBuildIndex = newBuildIndex;

            return newBuildIndex;
        }

        public static bool TryCreateNode(Vector3 position, NetInfo netPrefab, out ushort nodeId)
        {   
            // Gets managers
            var simulationManager = Singleton<SimulationManager>.instance;
            var netManager = Singleton<NetManager>.instance;

            // Gets default randomizer
            var randomizer = simulationManager.m_randomizer;

            LoggerUtils.Log("Creating node");

            // Create Node
            return netManager.CreateNode(out nodeId, ref randomizer, netPrefab, position, GetNewBuildIndex());
        }

        public static NodeWrapper CreateNode(Vector3 position, NetInfo netPrefab)
        {
            ushort nodeId = 0;

            if (!TryCreateNode(position, netPrefab, out nodeId))
            {
                throw new NetCreationException($"Cannot create road {netPrefab.name}. Error creating node ({position.x},{position.y},{position.z})");
            }

            return new NodeWrapper()
            {
                Id = nodeId,
                Position = position,
                Road = netPrefab
            };
        }


        public static NodeWrapper CreateNode(Vector3 position, string netPrefabName)
        {
            // Gets net prefab
            var netPrefab = PrefabCollection<NetInfo>.FindLoaded(netPrefabName);

            return CreateNode(position, netPrefab);
        }


        public static bool TryCreateSegment(ushort idStartNode, Vector3 startPosition, ushort idEndNode, Vector3 endPosition, NetInfo netPrefab, out ushort segmentId)
        {
            // Gets managers
            var simulationManager = Singleton<SimulationManager>.instance;
            var netManager = Singleton<NetManager>.instance;

            // Gets default randomizer
            var randomizer = simulationManager.m_randomizer;

            LoggerUtils.Log("Creating net");

            var startDirection = endPosition - startPosition;
            var endDirection = startPosition - endPosition;

            if (startDirection.magnitude > 80)
                LoggerUtils.Warning("The length of the segment is longer that the recommend (80 units)");

            var buildIndex = GetNewBuildIndex();

            // Create segment
            return netManager.CreateSegment(out segmentId, ref randomizer, netPrefab, idStartNode, idEndNode, startDirection.normalized, endDirection.normalized, buildIndex, buildIndex, false);
        }

        public static SegmentWrapper CreateSegment(NodeWrapper startNode, NodeWrapper endNode, NetInfo netPrefab)
        {
            ushort segmentId = 0;

            if (!TryCreateSegment(startNode.Id, startNode.Position, endNode.Id, endNode.Position, netPrefab, out segmentId))
            {
                var pos1 = $"({startNode.Position.x},{startNode.Position.y},{startNode.Position.z})";
                var pos2 = $"({endNode.Position.x},{endNode.Position.y},{endNode.Position.z})";

                throw new NetCreationException($"Cannot create road {netPrefab.name}. Error creating segment ({pos1},{pos2})");
            }

            return new SegmentWrapper()
            {
                Id = segmentId,
                StartPosition = startNode,
                EndPosition = endNode,
                Road = netPrefab
            };
        }

        public static SegmentWrapper CreateSegment(Vector3 startPosition, Vector3 endPosition, string netPrefabName)
        {
            // Gets net prefab
            var netPrefab = PrefabCollection<NetInfo>.FindLoaded(netPrefabName);

            // Creates start node
            var startNode = CreateNode(startPosition, netPrefabName);

            // Creates end node
            var endNode = CreateNode(endPosition, netPrefabName);

            return CreateSegment(startNode, endNode, netPrefab);
        }

        public static SegmentWrapper CreateSegment(NodeWrapper attachedNode, Vector3 endPosition, NetInfo netPrefab)
        {
            // Creates end node
            var endNode = CreateNode(endPosition, netPrefab);

            return CreateSegment(attachedNode, endNode, netPrefab);
        }

        public static RoadWrapper CreateRoad(List<Vector3> positions, NetInfo netPrefab)
        {
            if (positions.Count < 2)
            {
                throw new NetCreationException($"Cannot create road {netPrefab.name}. Must have 2 nodes at least");
            }

            var road = new RoadWrapper()
            {
                Segments = new List<SegmentWrapper>(),
                Road = netPrefab
            };

            var startNode = CreateNode(positions[0], netPrefab);
            for (int position = 1; position < positions.Count; position++)
            {
                var endPos = positions[position];
                var segment = CreateSegment(startNode, endPos, netPrefab);
                road.Segments.Add(segment);
                startNode = segment.EndPosition;
            }

            return road;
        }

        public static RoadWrapper CreateRoad(NodeWrapper attachedNode, List<Vector3> positions, NetInfo netPrefab)
        {
            if (positions.Count < 2)
            {
                throw new NetCreationException($"Cannot create road {netPrefab.name}. Must have 2 nodes at least");
            }

            var road = new RoadWrapper()
            {
                Segments = new List<SegmentWrapper>(),
                Road = netPrefab
            };

            var startNode = attachedNode;
            for (int position = 1; position < positions.Count; position++)
            {
                var endPos = positions[position];
                var segment = CreateSegment(startNode, endPos, netPrefab);
                road.Segments.Add(segment);
                startNode = segment.EndPosition;
            }

            return road;
        }

        public static RoadWrapper CreateRoad(List<Vector3> positions, string netPrefabName)
        {
            // Gets net prefab
            var netPrefab = PrefabCollection<NetInfo>.FindLoaded(netPrefabName);

            return CreateRoad(positions, netPrefab);
        }

        public static RoadWrapper CreateRoad(NodeWrapper attachedNode, List<Vector3> positions, string netPrefabName)
        {
            // Gets net prefab
            var netPrefab = PrefabCollection<NetInfo>.FindLoaded(netPrefabName);

            return CreateRoad(attachedNode, positions, netPrefab);
        }

        public static RoadWrapper CreateStraightRoad(Vector3 startPosition, Vector3 endPosition, string netPrefabName)
        {
            // Gets net prefab
            var netPrefab = PrefabCollection<NetInfo>.FindLoaded(netPrefabName);

            // Gets the number of subdivisions
            var length = (endPosition - startPosition).magnitude;
            var numberSubdivisions = Convert.ToSingle(Math.Ceiling(length / 80));
            var subdivisionLength = length / numberSubdivisions;

            // Calculates the subdivisions
            var subdivisions = new List<Vector3>() { startPosition };

            var position = startPosition;
            for (int subdivision = 0; subdivision+1 < numberSubdivisions; subdivision++)
            {
                var nextPosition = Vector3.MoveTowards(position, endPosition, subdivisionLength);
                subdivisions.Add(nextPosition);
                position = nextPosition;
            }

            subdivisions.Add(endPosition);

            return CreateRoad(subdivisions, netPrefab);
        }

        public static RoadWrapper CreateStraightRoad(NodeWrapper attachedNode, Vector3 endPosition, string netPrefabName)
        {
            // Gets net prefab
            var netPrefab = PrefabCollection<NetInfo>.FindLoaded(netPrefabName);

            // Gets the number of subdivisions
            var length = (endPosition - attachedNode.Position).magnitude;
            var numberSubdivisions = Convert.ToSingle(Math.Ceiling(length / 80));
            var subdivisionLength = length / numberSubdivisions;

            // Calculates the subdivisions
            var subdivisions = new List<Vector3>();

            var position = attachedNode.Position;
            for (int subdivision = 0; subdivision + 1 < numberSubdivisions; subdivision++)
            {
                var nextPosition = Vector3.MoveTowards(position, endPosition, subdivisionLength);
                subdivisions.Add(nextPosition);
                position = nextPosition;
            }

            subdivisions.Add(endPosition);

            return CreateRoad(attachedNode, subdivisions, netPrefab);
        }
    }
}
