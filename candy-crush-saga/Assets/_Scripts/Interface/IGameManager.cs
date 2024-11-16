using System;

namespace Interface
{
    public interface IGameManager
    {
        public event Action OnGameStart;
        public event Action OnGameEnd;

        public void PauseGame();
        public void ResumeGame();
    }
}
