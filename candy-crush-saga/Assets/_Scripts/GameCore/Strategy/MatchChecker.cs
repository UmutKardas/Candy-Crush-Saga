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
        private List<HashSet<(int, int)>> _matchGroups;

        private const int MinMatchLength = 3;

        public List<HashSet<(int, int)>> Execute(GameObject[,] grid, int rows, int columns)
        {
            _grid = grid;
            _rows = rows;
            _columns = columns;
            _matchGroups = new List<HashSet<(int, int)>>();

            for (var x = 0; x < rows; x++)
            {
                for (var y = 0; y < columns; y++)
                {
                    var currentNode = grid[x, y];
                    if (currentNode == null) continue;
                    if (!currentNode.TryGetComponent(out INode node)) continue;
                    if (!node.IsActive) continue;

                    var currentCandyType = node.CandyType;

                    TryAddMatchGroup(x, y, currentCandyType, MatchType.Horizontal);
                    TryAddMatchGroup(x, y, currentCandyType, MatchType.Vertical);
                }
            }

            return _matchGroups;
        }

        private void TryAddMatchGroup(int x, int y, CandyType candyType, MatchType matchType)
        {
            var match = new HashSet<(int, int)>
            {
                (x, y)
            };

            int dx = matchType == MatchType.Horizontal ? 1 : 0;
            int dy = matchType == MatchType.Vertical ? 1 : 0;

            for (int i = 1; ; i++)
            {
                int nx = x + dx * i;
                int ny = y + dy * i;

                if (nx >= _rows || ny >= _columns)
                    break;

                var nextNode = _grid[nx, ny];
                if (nextNode == null) break;

                if (!nextNode.TryGetComponent(out INode nextNodeComponent)) break;
                if (!nextNodeComponent.IsActive) break;
                if (nextNodeComponent.CandyType != candyType) break;

                match.Add((nx, ny));
            }

            if (match.Count >= MinMatchLength && !IsAlreadyAdded(match))
            {
                _matchGroups.Add(match);
            }
        }

        private bool IsAlreadyAdded(HashSet<(int, int)> match)
        {
            foreach (var existing in _matchGroups)
            {
                if (existing.Overlaps(match)) return true;
            }
            return false;
        }
    }

}