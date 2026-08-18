using TMPro;
using UnityEngine;

public class NewRecordPrompt : MonoBehaviour
{
    [SerializeField] private GameObject Panel;
    [SerializeField] private TMP_InputField NameInput;
    private string pendingSongId;
    private string pendingDifficulty;
    private int pendingScore;

    public bool IsShowing { get; private set; }

    public void TryShow(string songId, string difficulty, int score, int topCount)
    {
        if (!LeaderboardRepository.IsNewRecord(songId, difficulty, score, topCount))
        {
            IsShowing = false;
            return;
        }

        GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>().GetLeftSaber.SetActive(false);
        GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>().GetLeftShaft.SetActive(false);
        GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>().GetLeftModel.SetActive(true);

        GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>().GetRightSaber.SetActive(false);
        GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>().GetRightShaft.SetActive(false);
        GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>().GetRightModel.SetActive(true);

        pendingSongId = songId;
        pendingDifficulty = difficulty;
        pendingScore = score;

        NameInput.text = "Player";
        Panel.SetActive(true);
        IsShowing = true;
    }

    public void Confirm()
    {
        var playerName = string.IsNullOrWhiteSpace(NameInput.text) ? "Player" : NameInput.text;
        LeaderboardRepository.SubmitScore(pendingSongId, pendingDifficulty, playerName, pendingScore);

        Panel.SetActive(false);
        IsShowing = false;

        var leaderboard = FindAnyObjectByType<LeaderboardUI>();
        if (leaderboard != null)
        {
            leaderboard.Refresh();
        }
    }
}