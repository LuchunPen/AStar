/*
Copyright (c) Luchunpen.
Date: 02.04.2023
*/

using System;
using System.Collections.Generic;

namespace Nano3.Path.Example
{
    public class AStar2D : AStarCalculator<XYZ64>
    {
        private Map2D<bool> _map;
        public AStar2D(Map2D<bool> map)
        {
            _map = map;
        }

        protected override float GetEstimatedCost(XYZ64 from, XYZ64 to)
        {
            if (_map[to.X, to.Y] == true) { return ImpassableCost; }

            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        protected override bool GetNeighbors(XYZ64 point, List<XYZ64> neighbors)
        {
            if (neighbors == null) { return false; }
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (!_map.IsBound(point.X + x, point.Y + y)) { continue; }
                    neighbors.Add(new XYZ64(point.X + x, point.Y + y, 0));
                }
            }
            return true;
        }
    }
}
