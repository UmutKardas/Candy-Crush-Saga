using System;
using Interface;
using UnityEngine;

namespace GameCore.Managers
{
    public class GridManager : MonoBehaviour, IGridManager
    {
        #region Serialized Fields

        [SerializeField] private GameManager gameManager;

        #endregion

        #region Properties

        public GameObject[,] Grid { get; private set; }
        public int Rows => 10;
        public int Columns => 10;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            gameManager.OnGameStart += CreateGrid;
        }

        private void OnDisable()
        {
            gameManager.OnGameStart -= CreateGrid;
        }

        #endregion

        #region Private Methods

        private void CreateGrid()
        {
            Grid = new GameObject[Rows, Columns];

            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    SetupNode(i, j);
                }
            }
        }

        private void SetupNode(int rows, int columns)
        {
            var pooledObject = PoolManager.GetPoolObject(PoolType.Candy);
            var node = pooledObject.Instance;
            node.transform.position = CalculateNodePosition(rows, columns);
            node.GetComponent<INode>().Initialize(rows, columns);
            Grid[rows, columns] = node;
        }

        #endregion

        #region Public Methods

        public void SetObjectAt(int row, int column, GameObject obj)
        {
            if (IsValidCell(row, column))
                Grid[row, column] = obj;
            else
                Debug.LogError($"Invalid cell: ({row}, {column}).");
        }

        public void RemoveObjectAt(int row, int column)
        {
            if (IsValidCell(row, column)) Grid[row, column] = null;
        }

        public void ForEachCell(Action<int, int, GameObject> action)
        {
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    action?.Invoke(i, j, Grid[i, j]);
                }
            }
        }

        public void ClearGrid()
        {
            for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Columns; j++)
                Grid[i, j] = null;
        }

        private Vector3 CalculateNodePosition(int row, int column)
        {
            return new Vector3(row - (Rows - 1) / 2f, column - (Columns - 1) / 2f, 0);
        }

        public GameObject GetObjectAt(int row, int column)
        {
            return IsValidCell(row, column) ? Grid[row, column] : null;
        }

        private bool IsValidCell(int row, int column)
        {
            return row >= 0 && row < Rows && column >= 0 && column < Columns;
        }

        public bool HasObjectAt(int row, int column)
        {
            return IsValidCell(row, column) && Grid[row, column] != null;
        }

        #endregion
    }
}
