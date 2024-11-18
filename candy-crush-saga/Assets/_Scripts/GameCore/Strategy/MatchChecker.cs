using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Strategy
{
    public class MatchChecker : IStrategy
    {
        private GameObject[,] _grid;
        private int _rows, _columns;

        public List<(int, int)> Execute(GameObject[,] grid, int rows, int columns)
        {
            return new List<(int, int)>();
        }
    }
}
