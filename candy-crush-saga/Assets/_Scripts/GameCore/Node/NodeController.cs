using System.Linq;
using Interface;
using UnityEngine;
using DG.Tweening;
using GameCore.Scriptables;

namespace _Scripts.GameCore.Node
{
    public class NodeController : MonoBehaviour, INode
    {
        #region Serialized Fields

        [SerializeField] private CandyData candyData;
        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        #region Private Fields

        private const float SwapDuration = 0.3f;

        #endregion

        #region Properties

        public Vector3 Position { get; private set; }
        public int Row { get; set; }
        public int Column { get; set; }

        #endregion

        #region Public Methods

        public void Initialize(int row, int column)
        {
            SetGridPosition(row, column);
            SetColor(candyData.GetRandomCandyType());
        }

        public void SetGridPosition(int row, int column)
        {
            Row = row;
            Column = column;
            Position = transform.position;
        }

        public Sequence Swap(INode otherNode)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(otherNode.Position, SwapDuration).SetEase(Ease.Linear));
            return sequence;
        }

        #endregion

        #region Private Methods

        private void SetColor(CandyType candyType)
        {
            spriteRenderer.sprite = GetCandyData(candyType).sprite;
        }

        private Candy GetCandyData(CandyType candyType)
        {
            return candyData.candies.FirstOrDefault(x => x.type == candyType);
        }

        #endregion
    }
}
