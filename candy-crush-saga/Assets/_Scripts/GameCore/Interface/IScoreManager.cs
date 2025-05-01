using System;

namespace Interface
{
    public interface IScoreManager
    {
        event Action<int> OnScoreUpdated;
    }

}
