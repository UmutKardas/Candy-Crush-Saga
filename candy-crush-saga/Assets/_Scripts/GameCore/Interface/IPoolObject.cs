using System;

namespace Interface
{
    public interface IPoolObject
    {
        public Action OnReturnToPool { get; set; }
        public void Reset();
    }
}