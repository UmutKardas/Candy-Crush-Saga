using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Interface
{
    public interface INodeBehavior
    {
        public void Initialize(int row, int column, bool isAnimate = false);
        public void SetGridPosition(int row, int column, Vector3 position);
        public UniTask SetMatch();
        Sequence Swap(Vector3 position, float? customDuration = null);
    }
}
