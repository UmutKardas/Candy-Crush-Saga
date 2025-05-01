using System;
using System.Linq;
using Cysharp.Threading.Tasks;
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

        private const float SwapDuration = 0.2f;
        private const float DestroyDuration = 0.2f;

        #endregion

        #region INodeInfo Properties

        public CandyType CandyType { get; set; }
        public Vector3 Position { get; private set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public Action OnReturnToPool { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Position = transform.position;
        }

        #endregion

        #region Public Methods

        public void Initialize(int row, int column, bool isAnimate = false)
        {
            spriteRenderer.transform.localScale = Vector3.zero;
            CandyType = candyData.GetRandomCandyType();
            IsActive = true;
            SetGridPosition(row, column, transform.position);
            UpdateSpriteAsync(GetCandySpriteReference(CandyType), isAnimate);
        }

        public void SetGridPosition(int row, int column, Vector3 position)
        {
            Row = row;
            Column = column;
            Position = position;
        }

        public async UniTask SetMatch()
        {
            await DOTweenHelpers.WaitForSequenceCompletion(PlayScaleAnimation(Vector3.zero, Reset));
        }

        public Sequence Swap(Vector3 position, float? customDuration = null)
        {
            return DOTween.Sequence()
                .Append(transform.DOMove(position, customDuration ?? SwapDuration).SetEase(Ease.Linear));
        }


        public void Reset()
        {
            spriteRenderer.transform.localScale = Vector3.one;
            OnReturnToPool?.Invoke();
            IsActive = false;
        }

        #endregion

        #region Private Methods

        private async void UpdateSpriteAsync(UnityEngine.AddressableAssets.AssetReference assetReference, bool isAnimate = false)
        {
            var sprite = await AddressableManager.LoadAssetAsync<Sprite>(assetReference);
            spriteRenderer.sprite = sprite;
            PlayScaleAnimation(Vector2.one, null);
        }

        private UnityEngine.AddressableAssets.AssetReference GetCandySpriteReference(CandyType type)
        {
            return candyData.candies.FirstOrDefault(candy => candy.type == type).sprite;
        }


        private Sequence PlayScaleAnimation(Vector3 scale, Action onComplete = null)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(spriteRenderer.transform.DOScale(scale, DestroyDuration).OnComplete(() => { onComplete?.Invoke(); }));
            return sequence;
        }


        #endregion
    }
}