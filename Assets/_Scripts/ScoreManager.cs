using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private int basePoints = 100;
    [SerializeField] private int maxDirectionBonus = 50;
    [SerializeField] private int maxCenterBonus = 50;
    [SerializeField] private float maxCenterDistance = 0.5f;
    [SerializeField] private int maxComboMultiplier = 8;

    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int Multiplier => Mathf.Clamp(Combo, 1, maxComboMultiplier);

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

    public void RegisterHit(float directionAngle, float centerDistance, Vector3 hitPosition)
    {
        Combo++;

        var directionAccuracy = Mathf.InverseLerp(130f, 180f, directionAngle);
        var directionBonus = Mathf.RoundToInt(maxDirectionBonus * Mathf.Clamp01(directionAccuracy));

        var centerAccuracy = 1f - Mathf.Clamp01(centerDistance / maxCenterDistance);
        var centerBonus = Mathf.RoundToInt(maxCenterBonus * centerAccuracy);

        var points = (basePoints + directionBonus + centerBonus) * Multiplier;
        Score += points;

        OnScoreChanged?.Invoke(Score);
        OnComboChanged?.Invoke(Combo, Multiplier);
        OnHitScored?.Invoke(points, hitPosition);
    }

    public void RegisterMiss()
    {
        Combo = 0;

        OnComboChanged?.Invoke(Combo, Multiplier);
    }
}