using Interface;
using Structure;
using UnityEngine;
using VContainer;

namespace GameCore.UI.Score
{
    public class ScoreArea : Entity
    {
        #region  Private Fields
        private IScoreManager _scoreManager;
        private const string ScoreValue = "ScoreValue";


        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (_scoreManager != null)
            {
                _scoreManager.OnScoreUpdated += UpdateScore;
            }
        }

        private void OnDisable()
        {
            if (_scoreManager != null)
            {
                _scoreManager.OnScoreUpdated -= UpdateScore;
            }
        }

        #endregion


        #region Private Methods

        [Inject]
        private void Initialize(IScoreManager scoreManager)
        {
            _scoreManager = scoreManager;
        }

        private void UpdateScore(int value)
        {
            SetText(ScoreValue, value.ToString());
            StylizeColorFlash(ScoreValue, Color.yellow, Color.white);

        }

        #endregion
    }
}

