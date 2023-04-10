/*
Copyright (c) Luchunpen.
Date: 02.04.2023
*/

using System;
using System.Collections.Generic;

namespace Nano3.Path.Example
{
    public class AStar2DMap : IStarHeuristic<XYZ64>
    {
        private Map2D<bool> _map;

        List<XYZ64> neighbors = new List<XYZ64>();
        public AStar2DMap(Map2D<bool> map)
        {
            _map = map;
        }

        public float ImpassableCost { get { return float.PositiveInfinity; } }

        public float GetEstimatedCost(XYZ64 from, XYZ64 to)
        {
            if (_map[to.X, to.Y] == true) { return ImpassableCost; }

            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        public List<XYZ64> GetNeighbors(XYZ64 point)
        {
            neighbors.Clear();

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (!_map.IsBound(point.X + x, point.Y + y)) { continue; }
                    neighbors.Add(new XYZ64(point.X + x, point.Y + y, 0));
                }
            }
            return neighbors;
        }
    }
}
