using DG.Tweening;
using UnityEngine;

namespace Interface
{
    public interface INode
    {
        public Vector3 Position { get; }
        public int Row { get; set; }
        public int Column { get; set; }

        public void Initialize(int row, int column);
        public void SetGridPosition(int row, int column);
        public Sequence Swap(INode otherNode);
    }
}
