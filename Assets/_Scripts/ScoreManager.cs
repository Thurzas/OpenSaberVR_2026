using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private float maxPreSwingAngle = 100f;
    [SerializeField] private float maxPostSwingAngle = 60f;
    [SerializeField] private float maxCenterDistance = 0.15f;

    private const int MaxPreSwingPoints = 70;
    private const int MaxPostSwingPoints = 30;
    private const int MaxCenterPoints = 15;

    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int Multiplier { get; private set; } = 1;

    public event Action<int> OnScoreChanged;
    public event Action<int, int> OnComboChanged;
    public event Action<int, Vector3> OnHitScored;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void RegisterHit(float preSwingAngle, float postSwingAngle, float centerDistance, Vector3 hitPosition)
    {
        Combo++;
        Multiplier = Mathf.Max(Multiplier, GetMultiplierForCombo(Combo));

        var preSwingPoints = Mathf.RoundToInt(MaxPreSwingPoints * Mathf.Clamp01(preSwingAngle / maxPreSwingAngle));
        var postSwingPoints = Mathf.RoundToInt(MaxPostSwingPoints * Mathf.Clamp01(postSwingAngle / maxPostSwingAngle));
        var centerPoints = Mathf.RoundToInt(MaxCenterPoints * Mathf.Clamp01(1f - (centerDistance / maxCenterDistance)));

        var points = (preSwingPoints + postSwingPoints + centerPoints) * Multiplier;
        Score += points;

        OnScoreChanged?.Invoke(Score);
        OnComboChanged?.Invoke(Combo, Multiplier);
        OnHitScored?.Invoke(points, hitPosition);
    }

    public void RegisterMiss()
    {
        Multiplier = Mathf.Max(1, Multiplier / 2);
        Combo = 0;

        OnComboChanged?.Invoke(Combo, Multiplier);
    }

    private static int GetMultiplierForCombo(int combo)
    {
        if (combo >= 16)
        {
            return 8;
        }

        if (combo >= 8)
        {
            return 4;
        }

        if (combo >= 2)
        {
            return 2;
        }

        return 1;
    }
}