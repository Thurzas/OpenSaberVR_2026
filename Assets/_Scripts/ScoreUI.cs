using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Score;
    [SerializeField] private TextMeshProUGUI ComboStack;
    [SerializeField] private TextMeshProUGUI ComboMultiplier;
    [SerializeField] private TextMeshProUGUI CutTemplate;
    [SerializeField] private float cutPopupLifetime = 1f;

    private ScoreManager scoreManager;

    private void Start()
    {
        StartCoroutine(WaitForScoreManager());
    }

    private IEnumerator WaitForScoreManager()
    {
        while (ScoreManager.Instance == null)
        {
            yield return null;
        }

        scoreManager = ScoreManager.Instance;

        Score.text = scoreManager.Score.ToString();
        ComboStack.text = scoreManager.Combo.ToString();
        ComboMultiplier.text = $"(x{scoreManager.Multiplier})";

        scoreManager.OnScoreChanged += UpdateScore;
        scoreManager.OnComboChanged += UpdateCombo;
        scoreManager.OnHitScored += SpawnCutPopup;
    }

    private void OnDestroy()
    {
        if (scoreManager == null)
        {
            return;
        }

        scoreManager.OnScoreChanged -= UpdateScore;
        scoreManager.OnComboChanged -= UpdateCombo;
        scoreManager.OnHitScored -= SpawnCutPopup;
    }

    private void UpdateScore(int score)
    {
        Score.text = score.ToString();
    }

    private void UpdateCombo(int combo, int multiplier)
    {
        ComboStack.text = combo.ToString();
        ComboMultiplier.text = $"(x{multiplier})";
    }

    private void SpawnCutPopup(int points, Vector3 worldPosition)
    {
        var popup = Instantiate(CutTemplate, CutTemplate.transform.parent);
        popup.transform.position = worldPosition;
        popup.text = $"+{points}";
        popup.gameObject.SetActive(true);

        Destroy(popup.gameObject, cutPopupLifetime);
    }
}