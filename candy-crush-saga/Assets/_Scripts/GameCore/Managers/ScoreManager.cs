using System.Collections.Generic;
using GameCore.Managers;
using UnityEngine;
using System;
using Interface;

public class ScoreManager : MonoBehaviour, IScoreManager
{
    #region Actions
    public event Action<int> OnScoreUpdated;

    #endregion

    #region Serialized Fields

    [SerializeField] private GridManager gridManager;

    #endregion

    #region Private Fields

    private const int BaseScore = 100;

    private int _score;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        gridManager.OnMatchFound += HandleMatchFound;
    }

    private void OnDisable()
    {
        gridManager.OnMatchFound -= HandleMatchFound;
    }

    #endregion

    #region Private Methods

    private void HandleMatchFound(List<HashSet<(int, int)>> matchesGroup)
    {
        matchesGroup.ForEach(matches => _score += CalculateScore(matches.Count));

        OnScoreUpdated?.Invoke(_score);
    }

    private int CalculateScore(int matchSize)
    {
        return matchSize * BaseScore;
    }

    #endregion

}