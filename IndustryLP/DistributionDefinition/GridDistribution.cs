using ColossalFramework.Math;
using IndustryLP.Utils.Wrappers;
using System.Collections.Generic;
using UnityEngine;

namespace IndustryLP.DistributionDefinition
{
    internal class GridDistribution : DistributionThread
    {
        private Bezier3 GenerateSegment(Vector3 startPos, Vector3 startDir, Vector3 endPos, Vector3 endDir)
        {
            Bezier3 segment;
            segment.a = startPos;
            segment.d = endPos;

            NetSegment.CalculateMiddlePoints(segment.a, startDir, endPos, endDir, false, false, out segment.b, out segment.c);

            return segment;
        }

        private List<Bezier3> GenerateRoundabout(Vector3 center, Vector3 down, Vector3 right)
        {
            var subdivisions = new List<Bezier3>();

            var pos1 = center - right * 40f;
            var pos2 = pos1 - down * 40f;
            var pos3 = center - down * 40f;

            var dir12 = (pos2 - pos1).normalized;
            var dir32 = (pos2 - pos3).normalized;

            subdivisions.Add(GenerateSegment(pos1, dir12, pos3, dir32));

            var pos5 = center + right * 40f;
            var pos4 = pos5 - down * 40f;

            var dir34 = (pos4 - pos3).normalized;
            var dir54 = (pos4 - pos5).normalized;

            subdivisions.Add(GenerateSegment(pos3, dir34, pos5, dir54));

            var pos7 = center + down * 40f;
            var pos6 = pos7 + right * 40f;

            var dir56 = (pos6 - pos5).normalized;
            var dir76 = (pos6 - pos7).normalized;

            subdivisions.Add(GenerateSegment(pos5, dir56, pos7, dir76));

            var pos8 = pos1 + down * 40f;

            var dir87 = (pos8 - pos7).normalized;
            var dir81 = (pos8 - pos1).normalized;

            subdivisions.Add(GenerateSegment(pos7, dir87, pos1, dir81));

            return subdivisions;
        }

        private List<Bezier3> GenerateRoad(Vector3 start, Vector3 end)
        {
            var length = (end - start).magnitude;
            var subdivisionsCount = Mathf.Ceil(length / 80);
            var subdivisionsLength = length / subdivisionsCount;
            var subdivisions = new List<Bezier3>() { };

            var position = start;
            for (int i = 0; i <= subdivisionsCount; i++)
            {
                var next = Vector3.MoveTowards(position, end, subdivisionsLength);
                var startDir = (next - start).normalized;
                var endDir = (start - next).normalized;
                var segment = GenerateSegment(position, startDir, next, endDir);
                subdivisions.Add(segment);
                position = next;
            }

            return subdivisions;
        }

        public override DistributionInfo Generate(Quad3 selection)
        {
            // Create info value
            var info = new GridDistributionInfo
            {
                Cells = new List<ParcelWrapper>(),
                Road = new List<Bezier3>(),
            };

            var down = (selection.d - selection.a).normalized;
            var right = (selection.b - selection.a).normalized;

            // Create roundabout
            var midPointAB = Vector3.Lerp(selection.a, selection.b, 0.5f);
            var centerRoundabout = midPointAB + down * 40f;
            var roundabout = GenerateRoundabout(centerRoundabout, down, right);
            info.Road.AddRange(roundabout);

            // Generate distribution roads
            var leftPos = selection.a + down * 40f;
            var leftRoad = GenerateRoad(leftPos, roundabout[0].a);
            info.Road.AddRange(leftRoad);

            var rightPos = selection.b + down * 40f;
            var rightRoad = GenerateRoad(roundabout[2].a, rightPos);
            info.Road.AddRange(rightRoad);

            var upPos = Vector3.Lerp(selection.c, selection.d, 0.5f);
            var upRoad = GenerateRoad(roundabout[3].a, upPos);
            info.Road.AddRange(upRoad);

            var left = (selection.a - selection.b).normalized;

            // Generate secondary roads
            List<Bezier3> lastLeftRoadBranch = null, lastRightRoadBranch = null;
            
            foreach(var position in upRoad)
            {
                var roadLength = Vector3.Distance(selection.a, selection.b) / 2;

                var leftRoadBranch = GenerateRoad(position.d, position.d + left * roadLength);
                info.Road.AddRange(leftRoadBranch);

                if (lastLeftRoadBranch != null)
                {
                    for (int pos = 1; pos < leftRoadBranch.Count; pos++)
                    {
                        var startPos = lastLeftRoadBranch[pos].a;
                        var endPos = leftRoadBranch[pos].a;
                        var startDir = (endPos - startPos).normalized;
                        var endDir = (startPos - endPos).normalized;

                        info.Road.Add(GenerateSegment(startPos, startDir, endPos, endDir));
                    }
                }

                lastLeftRoadBranch = leftRoadBranch;

                var rightRoadBranch = GenerateRoad(position.d, position.d + right * roadLength);
                info.Road.AddRange(rightRoadBranch);

                if (lastRightRoadBranch != null)
                {
                    for (int pos = 1; pos < rightRoadBranch.Count; pos++)
                    {
                        var startPos = lastRightRoadBranch[pos].a;
                        var endPos = rightRoadBranch[pos].a;
                        var startDir = (endPos - startPos).normalized;
                        var endDir = (startPos - endPos).normalized;

                        info.Road.Add(GenerateSegment(startPos, startDir, endPos, endDir));
                    }
                }

                lastRightRoadBranch = rightRoadBranch;
            }

            return info;
        }
    }
}
