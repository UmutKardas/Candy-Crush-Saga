using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Interface
{
    public interface INodeBehavior
    {
        void Initialize(int row, int column);
        void SetGridPosition(int row, int column);
        void SetMatch();
        Sequence Swap(INodeDetails otherNode);
    }
}
