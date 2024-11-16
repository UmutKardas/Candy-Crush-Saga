using System;
using Interface;
using UnityEngine;

namespace GameCore.Managers
{
    public class GameManager : MonoBehaviour, IGameManager
    {
        #region Serialized Fields

        [SerializeField] private PoolManager poolManager;

        #endregion

        #region Actions

        public event Action OnGameStart;
        public event Action OnGameEnd;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Initialize();
        }

        #endregion

        #region Private Methods

        private async void Initialize()
        {
            await poolManager.PoolCreationCompletionSource.Task;
            OnGameStart?.Invoke();
        }

        #endregion


        #region Public Methods

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }

        #endregion
    }
}
