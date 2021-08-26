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
            var subdivisionsCount = Mathf.Ceil(length / 80f);
            var subdivisionsLength = length / subdivisionsCount;
            var subdivisions = new List<Bezier3>() { };
            var startDir = (end - start).normalized;
            var endDir = (start - end).normalized;

            var position = start;
            for (int i = 0; i < subdivisionsCount; i++)
            {
                var next = Vector3.MoveTowards(position, end, subdivisionsLength);
                var segment = GenerateSegment(position, startDir, next, endDir);
                subdivisions.Add(segment);
                position = next;
            }

            subdivisions.Add(GenerateSegment(position, startDir, end, endDir));

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

            var a1 = Vector3.Lerp(plot.a, plot.b, 1f/3f) - plot.a;
            var b1 = Vector3.Lerp(plot.a, plot.b, 2f/3f) - plot.a;
            var c1 = Vector3.Lerp(plot.a, plot.d, 1f/3f) - plot.a;
            var d1 = Vector3.Lerp(plot.a, plot.d, 2f/3f) - plot.a;

            LoggerUtils.Log(offsetX, offsetY, columns);

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

        /*private List<Bezier3> GenerateBranch(Quad3 selection, Vector3 start)
        {

        }

        private List<Bezier3> GenerateBranches(Quad3 selection, List<Bezier3> avenue)
        {

        }*/

        public override DistributionInfo Generate(Quad3 selection)
        {
            // Create info value
            var info = new GridDistributionInfo
            {
                Parcels = new List<ParcelWrapper>(),
                Road = new List<Bezier3>(),
            };

            var down = (selection.d - selection.a).normalized;
            var right = (selection.b - selection.a).normalized;

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

            var left = (selection.a - selection.b).normalized;
            var roadLength = Vector3.Distance(selection.a, selection.b) / 2;

            for (var posY = 0; posY < upRoad.Count - 1; posY++) // columns
            {
                var position = upRoad[posY];

                var leftRoadBranch = GenerateRoad(position.d + left * roadLength, position.d);
                info.Road.AddRange(leftRoadBranch);

                if (posY == 0) info.Columns = (leftRoadBranch.Count - 1) * 4;

                if (lastLeftRoadBranch != null)
                {
                    for (int posX = 0; posX < lastLeftRoadBranch.Count - 1; posX++) // rows
                    {
                        //LoggerUtils.Log("Line: ", posX);

                        // Gets the vertices of the plot
                        var startPos = lastLeftRoadBranch[posX].a;
                        
                        Vector3? endPos = null;
                        if (posX < leftRoadBranch.Count)
                        {
                            endPos = leftRoadBranch[posX].a;
                        }

                        if (endPos.HasValue)
                        {
                            var startDir = (endPos.Value - startPos).normalized;
                            var endDir = (startPos - endPos.Value).normalized;
                            var columns = (lastLeftRoadBranch.Count - 1) * 4;

                            // Initialize all parcels
                            /*if (pos == 0)
                            {
                                for (var i = 0; i < row; i++)
                                {
                                    info.Cells.Add(null);
                                }
                            }*/

                            Vector3? startNextPos = null;
                            if (posX + 1 < lastLeftRoadBranch.Count)
                            {
                                startNextPos = lastLeftRoadBranch[posX + 1].a;
                            }

                            Vector3? endNextPos = null;
                            if (posX + 1 < leftRoadBranch.Count)
                            {
                                endNextPos = leftRoadBranch[posX + 1].a;
                            }

                            // Gets interpolation points
                            if (startNextPos.HasValue && endNextPos.HasValue)
                            {
                                /*
                                var a1 = Vector3.Lerp(startPos, startNextPos.Value, 0.333333333333f) - startPos;
                                var b1 = Vector3.Lerp(startPos, startNextPos.Value, 0.666666666666f) - startPos;
                                var c1 = Vector3.Lerp(startPos, endPos.Value, 0.333333333333f) - startPos;
                                var d1 = Vector3.Lerp(startPos, endPos.Value, 0.666666666666f) - startPos;

                                // Generate the cells
                                info.Cells.Add(new ParcelWrapper
                                {
                                    GridId = id++,
                                    Position = a1 + c1 + startPos,
                                    Rotation = Vector3Extension.SignedAngle((startPos - startNextPos.Value).normalized, Vector3.forward, Vector3.up) * Mathf.Deg2Rad
                                });

                                info.Cells.Add(new ParcelWrapper
                                {
                                    GridId = id++,
                                    Position = b1 + c1 + startPos,
                                    Rotation = Vector3Extension.SignedAngle((startNextPos.Value - startPos).normalized, Vector3.forward, Vector3.up) * Mathf.Deg2Rad
                                });

                                info.Cells.Add(new ParcelWrapper
                                {
                                    GridId = id++,
                                    Position = a1 + d1 + startPos,
                                    Rotation = Vector3Extension.SignedAngle((startPos - startNextPos.Value).normalized, Vector3.forward, Vector3.up) * Mathf.Deg2Rad
                                });

                                info.Cells.Add(new ParcelWrapper
                                {
                                    GridId = id++,
                                    Position = b1 + d1 + startPos,
                                    Rotation = Vector3Extension.SignedAngle((startNextPos.Value - startPos).normalized, Vector3.forward, Vector3.up) * Mathf.Deg2Rad
                                });
                                */

                                var parcels = GenerateParcels(new Quad3(startPos, startNextPos.Value, endNextPos.Value, endPos.Value), posX * 2, (posY - 1) * 2, columns);
                                info.Parcels.AddRange(parcels);

                                foreach (var parcel in parcels)
                                {
                                    LoggerUtils.Log("Parcel: ", posX * 2, posY * 2, parcel.GridId);
                                }
                            }

                            // Generate the road
                            info.Road.Add(GenerateSegment(startPos, startDir, endPos.Value, endDir));
                        }
                    }
                }

                lastLeftRoadBranch = leftRoadBranch;

                var rightRoadBranch = GenerateRoad(position.d, position.d + right * roadLength);
                info.Road.AddRange(rightRoadBranch);

                if (lastRightRoadBranch != null)
                {
                    for (int posX = 1; posX < rightRoadBranch.Count; posX++)
                    {
                        var startPos = lastRightRoadBranch[posX].a;

                        Vector3? endPos = null;
                        if (posX < rightRoadBranch.Count)
                        {
                            endPos = rightRoadBranch[posX].a;
                        }

                        if (endPos.HasValue)
                        {
                            var startDir = (endPos.Value - startPos).normalized;
                            var endDir = (startPos - endPos.Value).normalized;

                            info.Road.Add(GenerateSegment(startPos, startDir, endPos.Value, endDir));

                            var columns = (lastRightRoadBranch.Count - 1) * 4;

                            var startLastPos = lastRightRoadBranch[posX - 1].a;
                            var endLastPos = rightRoadBranch[posX - 1].a;

                            // Gets interpolation points
                            var parcels = GenerateParcels(new Quad3(startLastPos, startPos, endPos.Value, endLastPos), (posX - 1) * 2 + Mathf.FloorToInt(columns / 2), (posY - 1) * 2, columns);
                            info.Parcels.AddRange(parcels);
                        }  
                    }
                }

                lastRightRoadBranch = rightRoadBranch;
            }

            return info;
        }
    }
}
