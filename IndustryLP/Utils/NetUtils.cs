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
        /// <summary>
        /// Exception that will be throw in <see cref="NetUtils"/> methods.
        /// </summary>
        public class NetCreationException : Exception
        {
            public NetCreationException(string message) : base(message) { }
        }

        /// <summary>
        /// Gets the next build index and update the simulation manager build index
        /// </summary>
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

        /// <summary>
        /// Tries to create a new node.
        /// </summary>
        /// <param name="position">Node position</param>
        /// <param name="netPrefab">A <see cref="NetInfo"/> object</param>
        /// <param name="nodeId">The id of new node</param>
        /// <returns>True if the node is created, false otherwise.</returns>
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

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="position">Node position</param>
        /// <param name="netPrefab">A <see cref="NetInfo"/> object</param>
        /// <returns>A new <see cref="NodeWrapper"/> object</returns>
        /// <exception cref="NetCreationException">When the node is not be created</exception>
        public static NodeWrapper CreateNode(Vector3 position, NetInfo netPrefab)
        {
            if (!TryCreateNode(position, netPrefab, out ushort nodeId))
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

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="position">Node position</param>
        /// <param name="netPrefabName">The name of the road prefab</param>
        /// <returns>A new <see cref="NodeWrapper"/> object</returns>
        /// <exception cref="NetCreationException">When the node is not be created</exception>
        public static NodeWrapper CreateNode(Vector3 position, string netPrefabName)
        {
            // Gets net prefab
            var netPrefab = PrefabCollection<NetInfo>.FindLoaded(netPrefabName);

            return CreateNode(position, netPrefab);
        }

        /// <summary>
        /// Tries to create a new segment
        /// </summary>
        /// <param name="idStartNode">Id of start node</param>
        /// <param name="startPosition">Position of start node</param>
        /// <param name="idEndNode">Id of end node</param>
        /// <param name="endPosition">Position of end node</param>
        /// <param name="netPrefab">A <see cref="NetInfo"/> object</param>
        /// <param name="segmentId">The id of new segment</param>
        /// <returns>True if the segment is created, false otherwise.</returns>
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

        /// <summary>
        /// Creates a new segment. See <see cref="CreateNode(Vector3, NetInfo)"/>, <see cref="CreateNode(Vector3, string)"/>
        /// </summary>
        /// <param name="startNode">A created start <see cref="NetNode"/></param>
        /// <param name="endNode">A end start <see cref="NetNode"/></param>
        /// <param name="netPrefab">A <see cref="NetInfo"/> object</param>
        /// <returns>A new <see cref="SegmentWrapper"/> object</returns>
        /// <exception cref="NetCreationException">When the node is not be created</exception>
        public static SegmentWrapper CreateSegment(NodeWrapper startNode, NodeWrapper endNode, NetInfo netPrefab)
        {
            if (!TryCreateSegment(startNode.Id, startNode.Position, endNode.Id, endNode.Position, netPrefab, out ushort segmentId))
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

        /// <summary>
        /// Creates a new segment. See <see cref="CreateNode(Vector3, NetInfo)"/>, <see cref="CreateNode(Vector3, string)"/>
        /// </summary>
        /// <param name="startPosition">The start position</param>
        /// <param name="endPosition">The end position</param>
        /// <param name="netPrefabName">The name of the road</param>
        /// <returns>A new <see cref="SegmentWrapper"/> object</returns>
        /// <exception cref="NetCreationException">When the node is not be created</exception>
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

        /// <summary>
        /// Creates a new segment. See <see cref="CreateNode(Vector3, NetInfo)"/>, <see cref="CreateNode(Vector3, string)"/>
        /// </summary>
        /// <param name="attachedNode">The node to attach the segment</param>
        /// <param name="endPosition">The end position</param>
        /// <param name="netPrefab">A <see cref="NetInfo"/> object</param>
        /// <returns>A new <see cref="SegmentWrapper"/> object</returns>
        /// <exception cref="NetCreationException">When the node is not be created</exception>
        public static SegmentWrapper CreateSegment(NodeWrapper attachedNode, Vector3 endPosition, NetInfo netPrefab)
        {
            // Creates end node
            var endNode = CreateNode(endPosition, netPrefab);

            return CreateSegment(attachedNode, endNode, netPrefab);
        }

        /// <summary>
        /// Creates a new road.
        /// </summary>
        /// <param name="positions">A list of positions when the road will pass through</param>
        /// <param name="netPrefab">A <see cref="NetInfo"/> object</param>
        /// <returns>A new <see cref="RoadWrapper"/> object</returns>
        /// <exception cref="NetCreationException">When the node is not be created</exception>
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

        /// <summary>
        /// Creates a new road.
        /// </summary>
        /// <param name="attachedNode">A <see cref="NodeWrapper"/> to attach the road</param>
        /// <param name="positions">A list of positions when the road will pass through</param>
        /// <param name="netPrefab">A <see cref="NetInfo"/> object</param>
        /// <returns>A new <see cref="RoadWrapper"/> object</returns>
        /// <exception cref="NetCreationException">When the node is not be created</exception>
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

        /// <summary>
        /// Creates a new road.
        /// </summary>
        /// <param name="positions">A list of positions when the road will pass through</param>
        /// <param name="netPrefabName">The name of the road</param>
        /// <returns>A new <see cref="RoadWrapper"/> object</returns>
        /// <exception cref="NetCreationException">When the node is not be created</exception>
        public static RoadWrapper CreateRoad(List<Vector3> positions, string netPrefabName)
        {
            // Gets net prefab
            var netPrefab = PrefabCollection<NetInfo>.FindLoaded(netPrefabName);

            return CreateRoad(positions, netPrefab);
        }

        /// <summary>
        /// Creates a new road.
        /// </summary>
        /// <param name="attachedNode">A <see cref="NodeWrapper"/> to attach the road</param>
        /// <param name="positions">A list of positions when the road will pass through</param>
        /// <param name="netPrefabName">The name of the road</param>
        /// <returns>A new <see cref="RoadWrapper"/> object</returns>
        /// <exception cref="NetCreationException">When the node is not be created</exception>
        public static RoadWrapper CreateRoad(NodeWrapper attachedNode, List<Vector3> positions, string netPrefabName)
        {
            // Gets net prefab
            var netPrefab = PrefabCollection<NetInfo>.FindLoaded(netPrefabName);

            return CreateRoad(attachedNode, positions, netPrefab);
        }

        /// <summary>
        /// Creates a straight road.
        /// </summary>
        /// <param name="startPosition">The start position of the road</param>
        /// <param name="endPosition">The end position of the road</param>
        /// <param name="netPrefabName">The name of the road</param>
        /// <returns>A new <see cref="RoadWrapper"/> object</returns>
        /// <exception cref="NetCreationException">When the node is not be created</exception>
        public static RoadWrapper CreateStraightRoad(Vector3 startPosition, Vector3 endPosition, string netPrefabName)
        {
            // Gets net prefab
            var netPrefab = PrefabCollection<NetInfo>.FindLoaded(netPrefabName);

            // Calculates the subdivisions
            var length = (endPosition - startPosition).magnitude;
            var subdivisionLength = netPrefab.m_segmentLength;
            var subdivisions = new List<Vector3>() { startPosition };

            var position = startPosition;
            for (var remainded = length; remainded - subdivisionLength > 0; remainded -= subdivisionLength)
            {
                var nextPosition = Vector3.MoveTowards(position, endPosition, subdivisionLength);
                subdivisions.Add(nextPosition);
                position = nextPosition;
            }

            subdivisions.Add(endPosition);

            return CreateRoad(subdivisions, netPrefab);
        }

        /// <summary>
        /// Creates a straight road.
        /// </summary>
        /// <param name="attachedNode">A <see cref="NodeWrapper"/> to attach the road</param>
        /// <param name="endPosition">The end position of the road</param>
        /// <param name="netPrefabName">The name of the road</param>
        /// <returns>A new <see cref="RoadWrapper"/> object</returns>
        /// <exception cref="NetCreationException">When the node is not be created</exception>
        public static RoadWrapper CreateStraightRoad(NodeWrapper attachedNode, Vector3 endPosition, string netPrefabName)
        {
            // Gets net prefab
            var netPrefab = PrefabCollection<NetInfo>.FindLoaded(netPrefabName);

            // Calculates the subdivisions
            var length = (endPosition - attachedNode.Position).magnitude;
            var subdivisionLength = netPrefab.m_segmentLength;
            var subdivisions = new List<Vector3>() { attachedNode.Position };

            var position = attachedNode.Position;
            for (var remainded = length; remainded - subdivisionLength > 0; remainded -= subdivisionLength)
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
