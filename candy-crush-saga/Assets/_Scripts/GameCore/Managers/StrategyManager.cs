using System.Collections.Generic;
using GameCore.Strategy;
using UnityEngine;

namespace GameCore.Managers
{
    public class StrategyManager : MonoBehaviour
    {
        private readonly IStrategy _currentStrategy = new MatchChecker();

        public List<HashSet<(int, int)>> CheckMatches(GameObject[,] grid, int rows, int columns)
        {
            return _currentStrategy.Execute(grid, rows, columns);
        }

    }
}