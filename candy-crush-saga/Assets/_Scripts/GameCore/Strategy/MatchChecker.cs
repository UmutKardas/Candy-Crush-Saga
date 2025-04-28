using System.Collections.Generic;
using GameCore.Scriptables;
using Interface;
using UnityEngine;

namespace GameCore.Strategy
{
    public class MatchChecker : IStrategy
    {
        private GameObject[,] _grid;
        private int _rows, _columns;
        private HashSet<(int, int)> _matchList;

        private const int MinMatchLength = 3;


        public HashSet<(int, int)> Execute(GameObject[,] grid, int rows, int columns)
        {
            _grid = grid;
            _rows = rows;
            _columns = columns;
            _matchList = new HashSet<(int, int)>();

            for (var x = 0; x < rows; x++)
            {
                for (var y = 0; y < columns; y++)
                {
                    var currentNode = grid[x, y];
                    if (currentNode == null) continue;
                    var currentCandyType = currentNode.GetComponent<INode>().CandyType;
                    CheckMatchByType(x, y, currentCandyType, MatchType.Horizontal);
                    CheckMatchByType(x, y, currentCandyType, MatchType.Vertical);
                }
            }

            return _matchList;
        }

        private void CheckMatchByType(int x, int y, CandyType currentCandyType, MatchType matchType)
        {
            var matchLength = 1;
            var matchPositions = new HashSet<(int, int)> { (x, y) };
            var primaryLength = matchType == MatchType.Horizontal ? _columns : _rows;
            var secondaryLength = matchType == MatchType.Horizontal ? _rows : _columns;

            for (var i = 1; i < primaryLength; i++)
            {
                if (matchType == MatchType.Horizontal ? x + i >= secondaryLength : y + i >= secondaryLength) break;
                var nextNodeValue = matchType == MatchType.Horizontal ? (x + i, y) : (x, y + i);
                var nextNode = _grid[nextNodeValue.Item1, nextNodeValue.Item2];
                if (nextNode == null || currentCandyType != nextNode.GetComponent<INode>().CandyType) break;

                matchLength++;
                matchPositions.Add((nextNodeValue.Item1, nextNodeValue.Item2));
            }

            if (matchLength < MinMatchLength) return;
            foreach (var pos in matchPositions)
            {
                _matchList.Add(pos);
            }
        }
    }
}