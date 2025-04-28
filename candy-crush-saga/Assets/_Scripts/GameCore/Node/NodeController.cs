using System;
using System.Linq;
using DG.Tweening;
using GameCore.Managers;
using GameCore.Scriptables;
using Interface;
using UnityEngine;

namespace _Scripts.GameCore.Node
{
    public class NodeController : MonoBehaviour, INode
    {
        #region Serialized Fields

        [SerializeField] private CandyData candyData;
        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        #region Constants

        private const float SwapDuration = 0.3f;
        private const float DestroyDuration = 0.2f;

        #endregion

        #region INodeInfo Properties

        public CandyType CandyType { get; set; }
        public Vector3 Position { get; private set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsMatched { get; private set; }
        public Action OnReturnToPool { get; set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Position = transform.position;
        }

        #endregion

        #region INodeAction Methods

        public void Initialize(int row, int column)
        {
            CandyType = candyData.GetRandomCandyType();
            IsMatched = false;
            SetGridPosition(row, column);
            UpdateSpriteAsync(GetCandySpriteReference(CandyType));
        }

        public void SetGridPosition(int row, int column)
        {
            Row = row;
            Column = column;
            Position = transform.position;
        }

        public void SetMatch()
        {
            PlayDestroyAnimation(Reset);
        }
        public Sequence Swap(INodeDetails otherNode)
        {
            return DOTween.Sequence()
                .Append(transform.DOMove(otherNode.Position, SwapDuration).SetEase(Ease.Linear));
        }


        public void Reset()
        {
            spriteRenderer.transform.localScale = Vector3.one;
            OnReturnToPool?.Invoke();
        }

        #endregion

        #region Private Methods

        private async void UpdateSpriteAsync(UnityEngine.AddressableAssets.AssetReference assetReference)
        {
            var sprite = await AddressableManager.LoadAssetAsync<Sprite>(assetReference);
            spriteRenderer.sprite = sprite;
            spriteRenderer.transform.localScale = Vector3.one;
        }

        private UnityEngine.AddressableAssets.AssetReference GetCandySpriteReference(CandyType type)
        {
            return candyData.candies.FirstOrDefault(candy => candy.type == type).sprite;
        }


        private void PlayDestroyAnimation(Action onComplete = null)
        {
            spriteRenderer.transform.DOScale(Vector3.zero, DestroyDuration).OnComplete(() => { onComplete?.Invoke(); });
        }
        #endregion
    }
}