using System;
using Cysharp.Threading.Tasks;
using Interface;
using UnityEngine;

namespace GameCore.Managers
{
    public class GridManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private InputManager inputManager;
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
            inputManager.OnNodeSwap += SwapNodes;
        }

        private void OnDisable()
        {
            gameManager.OnGameStart -= CreateGrid;
            inputManager.OnNodeSwap -= SwapNodes;
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

        private async void SwapNodes(INode firstNode, INode secondNode)
        {
            if (!IsNeighbor(firstNode.Row, firstNode.Column, secondNode.Row, secondNode.Column))
            {
                Debug.LogError("Nodes are not neighbors");
                return;
            }

            var firstNodeContent = Grid[firstNode.Row, firstNode.Column];
            var secondNodeContent = Grid[secondNode.Row, secondNode.Column];

            await UniTask.WhenAll(DOTweenHelpers.WaitForSequenceCompletion(firstNode.Swap(secondNode)),
                DOTweenHelpers.WaitForSequenceCompletion(secondNode.Swap(firstNode)));

            if (ShouldRevertSwap())
            {
                RevertSwap(firstNode, secondNode);
                return;
            }

            firstNode.SetGridPosition(GetGridPosition(secondNodeContent).row,
                GetGridPosition(secondNodeContent).column);
            secondNode.SetGridPosition(GetGridPosition(firstNodeContent).row, GetGridPosition(firstNodeContent).column);

            Grid[firstNode.Row, firstNode.Column] = secondNodeContent;
            Grid[secondNode.Row, secondNode.Column] = firstNodeContent;
        }

        private void RevertSwap(INode firstNode, INode secondNode)
        {
            firstNode.Swap(firstNode);
            secondNode.Swap(secondNode);
        }

        #endregion

        #region Public Methods

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

        public INode GetNodeAt(int row, int column)
        {
            return IsValidCell(row, column) ? Grid[row, column].GetComponent<INode>() : null;
        }

        private Vector3 CalculateNodePosition(int row, int column)
        {
            return new Vector3(row - (Rows - 1) / 2f, column - (Columns - 1) / 2f, 0);
        }

        private (int row, int column) GetGridPosition(GameObject targetObject)
        {
            for (var rowIndex = 0; rowIndex < Rows; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < Columns; columnIndex++)
                {
                    if (Grid[rowIndex, columnIndex] == targetObject)
                    {
                        return (rowIndex, columnIndex);
                    }
                }
            }

            return (-1, -1);
        }

        public GameObject GetObjectAt(int row, int column)
        {
            return IsValidCell(row, column) ? Grid[row, column] : null;
        }

        private bool IsNeighbor(int firstRow, int firstColumn, int secondRow, int secondColumn)
        {
            return (Mathf.Abs(firstRow - secondRow) == 1 && firstColumn == secondColumn) ||
                (Mathf.Abs(firstColumn - secondColumn) == 1 && firstRow == secondRow);
        }

        private bool IsValidCell(int row, int column)
        {
            return row >= 0 && row < Rows && column >= 0 && column < Columns;
        }

        private bool ShouldRevertSwap()
        {
            return false;
        }

        public bool HasObjectAt(int row, int column)
        {
            return IsValidCell(row, column) && Grid[row, column] != null;
        }

        #endregion
    }
}
