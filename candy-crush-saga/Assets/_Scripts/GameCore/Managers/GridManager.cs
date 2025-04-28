using System;
using Cysharp.Threading.Tasks;
using Interface;
using MyBox;
using UnityEngine;

namespace GameCore.Managers
{
    public class GridManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private InputManager inputManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private StrategyManager strategyManager;

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
            var nodeComponent = node.GetComponent<INode>();
            nodeComponent.Initialize(rows, columns);
            nodeComponent.OnReturnToPool = () =>
            {
                PoolManager.ReturnPoolObject(PoolType.Candy, pooledObject);
            };

            Grid[rows, columns] = node;
        }

        private async void SwapNodes(INode firstNode, INode secondNode)
        {
            try
            {
                if (!IsNeighbor(firstNode.Row, firstNode.Column, secondNode.Row, secondNode.Column))
                {

                    Debug.Log($"Not a neighbor: {firstNode.Row}, {firstNode.Column} - {secondNode.Row}, {secondNode.Column}");
                    return;
                }

                await UniTask.WhenAll(DOTweenHelpers.WaitForSequenceCompletion(firstNode.Swap(secondNode)),
                    DOTweenHelpers.WaitForSequenceCompletion(secondNode.Swap(firstNode)));

                ToggleNodes(firstNode, secondNode);

                var matches = strategyManager.CheckMatches(Grid, Rows, Columns);

                if (matches is not { Count: > 0 })
                {
                    RevertSwap(firstNode, secondNode);
                    return;
                }

                matches.ForEach(x =>
                {
                    var (row, column) = x;
                    var node = Grid[row, column].GetComponent<INode>();
                    node.SetMatch();
                });

            }
            catch (Exception e)
            {
                Debug.LogError($"Error swapping nodes: {e.Message}");
            }
        }

        private void ToggleNodes(INode firstNode, INode secondNode)
        {
            var firstNodeContent = Grid[firstNode.Row, firstNode.Column];
            var secondNodeContent = Grid[secondNode.Row, secondNode.Column];

            var firstNodeGridPosition = (firstNode.Row, firstNode.Column);
            var secondNodeGridPosition = (secondNode.Row, secondNode.Column);

            Grid[firstNode.Row, firstNode.Column] = secondNodeContent;
            Grid[secondNode.Row, secondNode.Column] = firstNodeContent;

            firstNode.SetGridPosition(secondNodeGridPosition.Row, secondNodeGridPosition.Column);
            secondNode.SetGridPosition(firstNodeGridPosition.Row, firstNodeGridPosition.Column);




            // Grid[firstNode.Row, firstNode.Column] = secondNode.NodeTransform.gameObject;
            // Grid[secondNode.Row, secondNode.Column] = firstNode.NodeTransform.gameObject;
        }


        private void RevertSwap(INode firstNode, INode secondNode)
        {
            firstNode.Swap(firstNode);
            secondNode.Swap(secondNode);
            ToggleNodes(firstNode, secondNode);
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

        public bool HasObjectAt(int row, int column)
        {
            return IsValidCell(row, column) && Grid[row, column] != null;
        }

        #endregion
    }
}