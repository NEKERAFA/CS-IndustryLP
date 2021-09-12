using ColossalFramework.Math;
using IndustryLP.Utils;
using IndustryLP.Utils.Wrappers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndustryLP.DistributionDefinition
{
    internal class GridDistributionThread : DistributionThread
    {
        private Bezier3 GenerateSegment(Vector3 startPos, Vector3 startDir, Vector3 endPos, Vector3 endDir)
        {
            Bezier3 segment;
            segment.a = startPos;
            segment.d = endPos;

            NetSegment.CalculateMiddlePoints(segment.a, startDir, endPos, endDir, false, false, out segment.b, out segment.c);

            return segment;
        }

        private List<Bezier3> GenerateRoundabout(Vector3 center, Vector3 down, Vector3 right, float size = 40f)
        {
            var subdivisions = new List<Bezier3>();

            var pos1 = center - right * size;
            var pos2 = pos1 - down * size;
            var pos3 = center - down * size;

            var dir12 = (pos2 - pos1).normalized;
            var dir32 = (pos2 - pos3).normalized;

            subdivisions.Add(GenerateSegment(pos1, dir12, pos3, dir32));

            var pos5 = center + right * size;
            var pos4 = pos5 - down * size;

            var dir34 = (pos4 - pos3).normalized;
            var dir54 = (pos4 - pos5).normalized;

            subdivisions.Add(GenerateSegment(pos3, dir34, pos5, dir54));

            var pos7 = center + down * size;
            var pos6 = pos7 + right * size;

            var dir56 = (pos6 - pos5).normalized;
            var dir76 = (pos6 - pos7).normalized;

            subdivisions.Add(GenerateSegment(pos5, dir56, pos7, dir76));

            var pos8 = pos1 + down * size;

            var dir87 = (pos8 - pos7).normalized;
            var dir81 = (pos8 - pos1).normalized;

            subdivisions.Add(GenerateSegment(pos7, dir87, pos1, dir81));

            return subdivisions;
        }

        private List<Bezier3> GenerateRoad(Vector3 start, Vector3 end)
        {
            var length = (end - start).magnitude;
            var subdivisionsCount = Mathf.Floor(length / 80f);
            var subdivisions = new List<Bezier3>() { };
            var startDir = (end - start).normalized;
            var endDir = (start - end).normalized;

            var position = start;
            for (int i = 1; i < subdivisionsCount; i++)
            {
                var next = Vector3.Lerp(start, end, Convert.ToSingle(i) / Convert.ToSingle(subdivisionsCount));
                var segment = GenerateSegment(position, startDir, next, endDir);
                subdivisions.Add(segment);
                position = next;
            }

            if ((end - position).magnitude > 0.1)
            {
                var segment = GenerateSegment(position, startDir, end, endDir);
                subdivisions.Add(segment);
            }

            return subdivisions;
        }

        private List<Bezier3> GenerateDistributionRoads(Quad3 selection, float margin = 40f)
        {
            var segments = new List<Bezier3>() { };
            var down = (selection.d - selection.a).normalized;
            var right = (selection.b - selection.a).normalized;

            // Create roundabout
            var midPointAB = Vector3.Lerp(selection.a, selection.b, 0.5f);
            var centerRoundabout = midPointAB + down * margin;
            var roundabout = GenerateRoundabout(centerRoundabout, down, right);
            segments.AddRange(roundabout);

            // Generate distribution roads
            var leftPos = selection.a + down * margin;
            var leftRoad = GenerateRoad(leftPos, roundabout[0].a);
            segments.AddRange(leftRoad);

            var rightPos = selection.b + down * margin;
            var rightRoad = GenerateRoad(roundabout[2].a, rightPos);
            segments.AddRange(rightRoad);

            return segments;
        }

        private List<ParcelWrapper> GenerateParcels(Quad3 plot, int offsetX, int offsetY, int columns)
        {
            var parcels = new List<ParcelWrapper>();

            var a1 = Vector3.Lerp(plot.a, plot.b, 1f/4f) - plot.a;
            var b1 = Vector3.Lerp(plot.a, plot.b, 3f/4f) - plot.a;
            var c1 = Vector3.Lerp(plot.a, plot.d, 1f/4f) - plot.a;
            var d1 = Vector3.Lerp(plot.a, plot.d, 3f/4f) - plot.a;

            // Generate the cells
            parcels.Add(new ParcelWrapper
            {
                GridId = Convert.ToUInt16(offsetX + offsetY * columns),
                Position = a1 + c1 + plot.a,
                Rotation = Vector3Extension.SignedAngle((plot.a - plot.b).normalized, Vector3.forward, Vector3.up) * Mathf.Deg2Rad
            });

            parcels.Add(new ParcelWrapper
            {
                GridId = Convert.ToUInt16(offsetX + 1 + offsetY * columns),
                Position = b1 + c1 + plot.a,
                Rotation = Vector3Extension.SignedAngle((plot.b - plot.a).normalized, Vector3.forward, Vector3.up) * Mathf.Deg2Rad
            });

            parcels.Add(new ParcelWrapper
            {
                GridId = Convert.ToUInt16(offsetX + offsetY * columns + columns),
                Position = a1 + d1 + plot.a,
                Rotation = Vector3Extension.SignedAngle((plot.a - plot.b).normalized, Vector3.forward, Vector3.up) * Mathf.Deg2Rad
            });

            parcels.Add(new ParcelWrapper
            {
                GridId = Convert.ToUInt16(offsetX + 1 + offsetY * columns + columns),
                Position = b1 + d1 + plot.a,
                Rotation = Vector3Extension.SignedAngle((plot.b - plot.a).normalized, Vector3.forward, Vector3.up) * Mathf.Deg2Rad
            });

            return parcels;
        }

        private struct BranchWapper
        {
            public List<Bezier3> BranchRoad { get; }
            public List<Bezier3> Roads { get; }
            public List<ParcelWrapper> Parcels { get; }

            public BranchWapper(List<Bezier3> branchRoad, List<Bezier3> roads, List<ParcelWrapper> parcels)
            {
                BranchRoad = branchRoad;
                Roads = roads;
                Parcels = parcels;
            }
        }

        private BranchWapper GenerateLeftBranch(Quad3 selection, Vector3 start, int branch, List<Bezier3> lastBranchRoad)
        {
            var left = (selection.a - selection.b).normalized;
            var roadLength = Vector3.Distance(selection.a, selection.b) / 2;

            var roads = new List<Bezier3>();
            var parcels = new List<ParcelWrapper>();

            // Generate the main road
            var branchRoad = GenerateRoad(start + left * roadLength, start);
            if (lastBranchRoad != null)
            {
                for (var cell = 0; cell < lastBranchRoad.Count; cell++)
                {
                    var startPos = lastBranchRoad[cell].a;
                    var endPos = branchRoad[cell].a;
                    var startDir = (endPos - startPos).normalized;

                    // Generate perpendicular segments
                    roads.Add(GenerateSegment(startPos, startDir, endPos, -startDir));

                    var startNextPos = lastBranchRoad[cell].d;
                    var endNextPos = lastBranchRoad[cell].d;
                    var columns = lastBranchRoad.Count * 4;

                    // Generate parcels
                    var localParcels = GenerateParcels(new Quad3(startPos, startNextPos, endNextPos, endPos), cell * 2, (branch - 1) * 2, columns);
                    parcels.AddRange(localParcels);
                }
            }

            return new BranchWapper(branchRoad, roads, parcels);
        }

        private BranchWapper GenerateRightBranch(Quad3 selection, Vector3 start, int branch, List<Bezier3> lastBranchRoad)
        {
            var right = (selection.b - selection.a).normalized;
            var roadLength = Vector3.Distance(selection.a, selection.b) / 2;

            var roads = new List<Bezier3>();
            var parcels = new List<ParcelWrapper>();

            // Generate the main road
            var branchRoad = GenerateRoad(start, start + right * roadLength);
            if (lastBranchRoad != null)
            {
                Vector3 startPos, endPos, startDir;

                for (var cell = 0; cell < lastBranchRoad.Count; cell++)
                {
                    startPos = lastBranchRoad[cell].d;
                    endPos = branchRoad[cell].d;
                    startDir = (endPos - startPos).normalized;
                    // Generate perpendicular segments
                    roads.Add(GenerateSegment(startPos, startDir, endPos, -startDir));

                    var startLastPos = lastBranchRoad[cell].a;
                    var endLastPos = branchRoad[cell].a;
                    var columns = lastBranchRoad.Count * 4;

                    // Generate parcels
                    var localParcels = GenerateParcels(new Quad3(startLastPos, startPos, endPos, endLastPos), cell * 2 + Mathf.FloorToInt(columns / 2), (branch - 1) * 2, columns);
                    parcels.AddRange(localParcels);
                }
            }

            return new BranchWapper(branchRoad, roads, parcels);
        }

        public override DistributionInfo Generate(Quad3 selection)
        {
            // Create info value
            var info = new GridDistributionInfo
            {
                Parcels = new List<ParcelWrapper>(),
                Road = new List<Bezier3>(),
            };

            // Generate distribution roads
            var distributionRoads = GenerateDistributionRoads(selection);
            info.Road.AddRange(distributionRoads);

            // Generate main avenue
            var upPos = Vector3.Lerp(selection.c, selection.d, 0.5f);
            var upRoad = GenerateRoad(distributionRoads[3].a, upPos);
            info.Road.AddRange(upRoad);

            info.Rows = (upRoad.Count - 1) * 2;

            // Generate secondary roads
            List<Bezier3> lastLeftRoadBranch = null, lastRightRoadBranch = null;

            for (var branch = 0; branch < upRoad.Count; branch++) // columns
            {
                var position = upRoad[branch];

                // Generate left branch
                var leftBranch = GenerateLeftBranch(selection, position.d, branch, lastLeftRoadBranch);
                info.Road.AddRange(leftBranch.BranchRoad);
                if (branch == 0)
                {
                    info.Columns = leftBranch.BranchRoad.Count * 4;
                }
                if (leftBranch.Roads.Count > 0)
                {
                    info.Road.AddRange(leftBranch.Roads);
                }
                if (leftBranch.Parcels.Count > 0)
                {
                    info.Parcels.AddRange(leftBranch.Parcels);
                }
                lastLeftRoadBranch = leftBranch.BranchRoad;

                // Generate right branch
                var rightBranch = GenerateRightBranch(selection, position.d, branch, lastRightRoadBranch);
                info.Road.AddRange(rightBranch.BranchRoad);
                if (rightBranch.Roads.Count > 0)
                {
                    info.Road.AddRange(rightBranch.Roads);
                }
                if (rightBranch.Parcels.Count > 0)
                {
                    info.Parcels.AddRange(rightBranch.Parcels);
                }
                lastRightRoadBranch = rightBranch.BranchRoad;
            }

            return info;
        }
    }
}
