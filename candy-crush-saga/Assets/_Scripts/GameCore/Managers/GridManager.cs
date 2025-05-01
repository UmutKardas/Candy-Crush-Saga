using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Interface;
using UnityEngine;

namespace GameCore.Managers
{
    public class GridManager : MonoBehaviour
    {
        #region Actions
        public event Action<List<HashSet<(int, int)>>> OnMatchFound;
        #endregion

        #region Serialized Fields

        [SerializeField] private InputManager inputManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private StrategyManager strategyManager;
        [SerializeField] private GameObject gridPrefab;

        #endregion

        #region Properties

        public GameObject[,] Grid { get; private set; }
        public int Rows => 10;
        public int Columns => 10;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            gameManager.OnGameStart += InitializeGrid;
            inputManager.OnNodeSwap += HandleNodeSwap;
        }

        private void OnDisable()
        {
            gameManager.OnGameStart -= InitializeGrid;
            inputManager.OnNodeSwap -= HandleNodeSwap;
        }

        #endregion

        #region Private Methods

        private void InitializeGrid()
        {
            Grid = new GameObject[Rows, Columns];

            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    SpawnGridCell(i, j);
                    SpawnNodeAt(i, j);
                }
            }

            ValidateMatches().Forget();
        }

        private void SpawnGridCell(int row, int column)
        {
            Instantiate(gridPrefab, CalculateNodeWorldPosition(row, column), Quaternion.identity, transform);
        }

        private void SpawnNodeAt(int rows, int columns, bool isAnimate = false)
        {
            var pooledObject = PoolManager.GetPoolObject(PoolType.Candy);
            var node = pooledObject.Instance;
            node.transform.position = CalculateNodeWorldPosition(rows, columns);
            var nodeComponent = node.GetComponent<INode>();
            nodeComponent.Initialize(rows, columns, isAnimate);
            nodeComponent.OnReturnToPool = () =>
            {
                PoolManager.ReturnPoolObject(PoolType.Candy, pooledObject);
            };

            Grid[rows, columns] = node;
        }

        private async void HandleNodeSwap(INode firstNode, INode secondNode)
        {
            try
            {
                if (!AreNodesAdjacent(firstNode.Row, firstNode.Column, secondNode.Row, secondNode.Column))
                {

                    Debug.Log($"Not a neighbor: {firstNode.Row}, {firstNode.Column} - {secondNode.Row}, {secondNode.Column}");
                    return;
                }

                await UniTask.WhenAll(DOTweenHelpers.WaitForSequenceCompletion(firstNode.Swap(secondNode.Position)),
                    DOTweenHelpers.WaitForSequenceCompletion(secondNode.Swap(firstNode.Position)));

                SwapNodeReferences(firstNode, secondNode);

                await ValidateMatches(null, () => { RevertSwap(firstNode, secondNode); });

            }
            catch (Exception e)
            {
                Debug.LogError($"Error swapping nodes: {e.Message}");
            }
        }


        private async UniTask ValidateMatches(Action onComplete = null, Action onFailed = null)
        {
            var matchesGroup = strategyManager.CheckMatches(Grid, Rows, Columns);

            if (matchesGroup is not { Count: > 0 })
            {
                onFailed?.Invoke();
                return;
            }
            OnMatchFound?.Invoke(matchesGroup);
            var tasks = matchesGroup.SelectMany(group => group).Select(pos => Grid[pos.Item1, pos.Item2].GetComponent<INode>().SetMatch()).ToList();

            await UniTask.WhenAll(tasks);

            await HandlePostMatch();
            onComplete?.Invoke();

        }

        private async UniTask HandlePostMatch()
        {
            await DropInactiveNodes();
            await SpawnNewNodesForEmptySpots();
            ValidateMatches().Forget();
        }

        private void RevertSwap(INode firstNode, INode secondNode)
        {
            firstNode.Swap(secondNode.Position);
            secondNode.Swap(firstNode.Position);
            SwapNodeReferences(firstNode, secondNode);
        }

        private void SwapNodeReferences(INode firstNode, INode secondNode)
        {
            var firstNodeContent = Grid[firstNode.Row, firstNode.Column];
            var secondNodeContent = Grid[secondNode.Row, secondNode.Column];

            var firstNodeGridDetail = (firstNode.Row, firstNode.Column);
            var secondNodeGridDetail = (secondNode.Row, secondNode.Column);

            var firstNodeGridPosition = CalculateNodeWorldPosition(secondNodeGridDetail.Row, secondNodeGridDetail.Column);
            var secondNodeGridPosition = CalculateNodeWorldPosition(firstNodeGridDetail.Row, firstNodeGridDetail.Column);

            Grid[firstNode.Row, firstNode.Column] = secondNodeContent;
            Grid[secondNode.Row, secondNode.Column] = firstNodeContent;

            firstNode.SetGridPosition(secondNodeGridDetail.Row, secondNodeGridDetail.Column, firstNodeGridPosition);
            secondNode.SetGridPosition(firstNodeGridDetail.Row, firstNodeGridDetail.Column, secondNodeGridPosition);
        }

        private async UniTask DropInactiveNodes()
        {
            var tasks = new List<UniTask>();

            for (var x = 0; x < Rows; x++)
            {
                for (var y = 0; y < Columns; y++)
                {
                    var currentNode = Grid[x, y];
                    if (currentNode == null) { continue; }

                    if (!currentNode.TryGetComponent(out INode node)) { continue; }
                    if (!node.IsActive) { continue; }

                    var lastInactiveNode = GetLastInactiveNodeBelow(x, y);

                    if (lastInactiveNode == null)
                    {
                        continue;
                    }

                    tasks.Add(DOTweenHelpers.WaitForSequenceCompletion(node.Swap(lastInactiveNode.Position)));
                    tasks.Add(DOTweenHelpers.WaitForSequenceCompletion(lastInactiveNode.Swap(node.Position)));

                    SwapNodeReferences(node, lastInactiveNode);
                }
            }
            await UniTask.WhenAll(tasks);
        }

        private UniTask SpawnNewNodesForEmptySpots()
        {
            var destroyedNodes = GetDestroyedNodes();
            if (destroyedNodes is not { Count: > 0 }) { return UniTask.CompletedTask; }
            destroyedNodes.ForEach(destroyedNode => { SpawnNodeAt(destroyedNode.Item1, destroyedNode.Item2, true); });
            return UniTask.CompletedTask;
        }

        private List<(int, int)> GetDestroyedNodes()
        {
            var destroyedNodes = new List<(int, int)>();

            for (var x = 0; x < Rows; x++)
            {
                for (var y = 0; y < Columns; y++)
                {
                    var currentNode = Grid[x, y];
                    if (currentNode == null) { continue; }

                    if (!currentNode.TryGetComponent(out INode node)) { continue; }
                    if (node.IsActive) { continue; }
                    destroyedNodes.Add((x, y));
                }
            }

            return destroyedNodes;
        }

        private INode GetLastInactiveNodeBelow(int x, int y)
        {
            INode lastNode = null;
            for (var i = 1; i < Columns; i++)
            {
                if (y - i < 0) { break; }
                var nextNode = Grid[x, y - i];
                if (nextNode == null) { break; }

                if (!nextNode.TryGetComponent(out INode downNode)) { return lastNode; }
                if (downNode.IsActive)
                {
                    break;
                }
                lastNode = downNode;
            }

            return lastNode;
        }


        private Vector3 CalculateNodeWorldPosition(int row, int column)
        {
            return new Vector3(row - (Rows - 1) / 2f, column - (Columns - 1) / 2f, 0);
        }

        private bool AreNodesAdjacent(int firstRow, int firstColumn, int secondRow, int secondColumn)
        {
            return (Mathf.Abs(firstRow - secondRow) == 1 && firstColumn == secondColumn) ||
                   (Mathf.Abs(firstColumn - secondColumn) == 1 && firstRow == secondRow);
        }
        #endregion

        #region Public Methods

        public INode TryGetNodeFromMousePosition(Vector3 mousePosition, RaycastHit2D hit2D, LayerMask layerMask)
        {
            hit2D = Physics2D.Raycast(mousePosition, Vector2.zero, 0.1f, layerMask);
            return hit2D.collider != null ? hit2D.collider.GetComponent<INode>() : null;
        }
        #endregion
    }
}