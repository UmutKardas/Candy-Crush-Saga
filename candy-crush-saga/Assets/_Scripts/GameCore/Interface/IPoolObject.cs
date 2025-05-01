using System;

namespace Interface
{
    public interface IPoolObject
    {
        public bool IsActive { get; }
        public Action OnReturnToPool { get; set; }
        public void Reset();
    }
}