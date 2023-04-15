/*
Copyright (c) Luchunpen.
Date: 02.04.2023
*/

using System;
using System.Collections.Generic;
using Nano3.Path.Collection;

namespace Nano3.Path
{
    public interface IAStarHeuristic<T> where T : struct, IEquatable<T>
    {
        float ImpassableCost { get; }
        List<T> GetNeighbors(T point);
        float GetEstimatedCost(T from, T to);
    }

    public class AStarCalculator<T> where T : struct, IEquatable<T>
    {
        private class Node : IComparable<Node>
        {
            public T Index;
            public T ParentIndex;

            //Real cost form start point
            public float G = 0;

            //estimated cost to goal point
            public float H = 0;

            //lowest total cost
            public float F = 0;

            public bool IsClosed = false;

            public Node(T index)
            {
                Index = index;
            }

            public override int GetHashCode()
            {
                return Index.GetHashCode();
            }

            public int CompareTo(Node other)
            {
                if (F < other.F) return -1;
                if (F > other.F) return 1;

                return 0;
            }
        }

        private FastDictionaryM2<T, Node> _nodesStorage = new FastDictionaryM2<T, Node>();
        private HeapMin<Node> _openHeap = new HeapMin<Node>();

        public List<T> CalculatePath<H>(T start, T goal, H heuristic, int maxIterations) where H : IAStarHeuristic<T>
        {
            List<T> result = null;
            if (start.Equals(goal)) { return result; }

            _nodesStorage.Clear();
            _openHeap.Clear();

            Node startNode = new Node(start);
            startNode.H = heuristic.GetEstimatedCost(start, goal);
            _nodesStorage.Add(startNode.Index, startNode);
            _openHeap.Add(startNode);

            Node best = null;
            int iterations = 0;

            while (true)
            {
                if (iterations > maxIterations) { goto CREATE_PATH; }
                Node currentNode = (_openHeap.Count > 0) ? _openHeap.GetBest() : null;

                if (currentNode == null) { goto CREATE_PATH; }
                if (currentNode.Index.Equals(goal)) { goto CREATE_PATH; }
                if (currentNode.IsClosed) { continue; }

                if (currentNode != startNode)
                {
                    if (best == null) { best = currentNode; }
                    else if (currentNode.H < best.H) { best = currentNode; }
                }

                List<T> _neighbors = heuristic.GetNeighbors(currentNode.Index);
                for (int i = 0; i < _neighbors.Count; i++)
                {
                    T neibIndex = _neighbors[i];
                    Node neibNode;
                    _nodesStorage.TryGetValue(neibIndex, out neibNode);
                    if (neibNode != null && neibNode.IsClosed) { continue; }

                    float stepCost = heuristic.GetEstimatedCost(currentNode.Index, neibIndex);
                    if (stepCost == heuristic.ImpassableCost) { continue; }

                    float g = currentNode.G + stepCost;
                    if (neibNode == null)
                    {
                        neibNode = new Node(neibIndex);
                        neibNode.ParentIndex = currentNode.Index;
                        neibNode.G = g;
                        neibNode.H = heuristic.GetEstimatedCost(neibIndex, goal);
                        neibNode.F = neibNode.G + neibNode.H;
                        _nodesStorage.Add(neibIndex, neibNode);
                        _openHeap.Add(neibNode);
                    }
                    else if (g < neibNode.G)
                    {
                        neibNode.G = g;
                        neibNode.F = neibNode.G + neibNode.H;
                        neibNode.ParentIndex = currentNode.Index;
                        _openHeap.Add(neibNode);
                    }
                }

                currentNode.IsClosed = true;
                iterations++;
            }

        CREATE_PATH:

            result = new List<T>();
            if (startNode == null || best == null) { return result; }

            Node n;
            T cur_index = best.Index;

            do
            {
                _nodesStorage.TryGetAndRemove(cur_index, out n);
                if (n == null) { return null; }
                result.Add(n.Index);
                cur_index = n.ParentIndex;
            }
            while (!cur_index.Equals(startNode.Index));

            return result;
        }

        public List<T> CalculateArea<H>(T start, float maxCost, H heuristic) where H : IAStarHeuristic<T>
        {
            List<T> result = new List<T>();
            if (maxCost <= 0) { return result; }

            _nodesStorage.Clear();
            _openHeap.Clear();

            Node startNode = new Node(start);
            _nodesStorage.Add(startNode.Index, startNode);
            _openHeap.Add(startNode);

            while (true)
            {
                Node currentNode = (_openHeap.Count > 0) ? _openHeap.GetBest() : null;

                if (currentNode == null) { return result; }
                if (currentNode.IsClosed) { continue; }

                List<T> _neighbors = heuristic.GetNeighbors(currentNode.Index);
                for (int i = 0; i < _neighbors.Count; i++)
                {
                    T neibIndex = _neighbors[i];
                    Node neibNode;
                    _nodesStorage.TryGetValue(neibIndex, out neibNode);
                    if (neibNode != null && neibNode.IsClosed) { continue; }

                    float stepCost = heuristic.GetEstimatedCost(currentNode.Index, neibIndex);
                    if (stepCost == heuristic.ImpassableCost) { continue; }

                    float g = currentNode.G + stepCost;
                    if (g > maxCost) { continue; }

                    if (neibNode == null)
                    {
                        neibNode = new Node(neibIndex);
                        neibNode.ParentIndex = currentNode.Index;
                        neibNode.G = g;
                        neibNode.F = neibNode.G + neibNode.H;
                        _nodesStorage.Add(neibIndex, neibNode);
                        _openHeap.Add(neibNode);
                    }
                    else if (g < neibNode.G)
                    {
                        neibNode.G = g;
                        neibNode.F = neibNode.G + neibNode.H;
                        neibNode.ParentIndex = currentNode.Index;
                        _openHeap.Add(neibNode);
                    }
                }

                currentNode.IsClosed = true;
                result.Add(currentNode.Index);
            }
        }
    }
}
