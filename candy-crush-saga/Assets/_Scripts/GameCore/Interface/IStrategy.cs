using System.Collections.Generic;
using UnityEngine;

public interface IStrategy
{
    public List<HashSet<(int, int)>> Execute(GameObject[,] grid, int rows, int columns);
}
