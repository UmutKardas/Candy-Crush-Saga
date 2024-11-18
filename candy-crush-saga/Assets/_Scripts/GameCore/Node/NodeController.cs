using System.Linq;
using GameCore.Managers;
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

        public CandyType CandyType { get; set; }
        public Vector3 Position { get; private set; }
        public int Row { get; set; }
        public int Column { get; set; }

        #endregion

        #region Public Methods

        public void Initialize(int row, int column)
        {
            CandyType = candyData.GetRandomCandyType();
            SetGridPosition(row, column);
            SetColor();
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

        private async void SetColor()
        {
            var assetReference = GetCandyData(CandyType).sprite;
            var sprite = await AddressableManager.LoadAssetAsync<Sprite>(assetReference);
            spriteRenderer.sprite = sprite;
        }

        private Candy GetCandyData(CandyType candyType)
        {
            return candyData.candies.FirstOrDefault(x => x.type == candyType);
        }

        #endregion
    }
}
