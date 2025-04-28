using System.Collections.Generic;
using UnityEngine;

public interface IStrategy
{
    public HashSet<(int, int)> Execute(GameObject[,] grid, int rows, int columns);
}
