using System;
using UnityEngine;

namespace Interface
{
    public interface IGridManager
    {
        public int Rows { get; }
        public int Columns { get; }

        public void SetObjectAt(int row, int column, GameObject obj);

        public GameObject[,] Grid { get; }

        public GameObject GetObjectAt(int row, int column);

        public bool HasObjectAt(int row, int column);

        public void RemoveObjectAt(int row, int column);

        public void ClearGrid();

        public void ForEachCell(Action<int, int, GameObject> action);
    }
}
