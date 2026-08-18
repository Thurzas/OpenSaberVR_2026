using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [System.Serializable]
    public class PlayerRow
    {
        public TextMeshProUGUI Rank;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Score;
    }

    [SerializeField] private TextMeshProUGUI DifficultyLabel;
    [SerializeField] private PlayerRow[] Rows = new PlayerRow[5];

    private static readonly string[] RankLabels = { "1st", "2nd", "3rd", "4th", "5th" };

    private SongSettings songSettings;
    private int difficultyIndex;

    private void Start()
    {
        var songSettingsObject = GameObject.FindGameObjectWithTag("SongSettings");
        if (songSettingsObject == null)
        {
            Debug.LogError("LeaderboardUI: no SongSettings found in the scene.");
            return;
        }

        songSettings = songSettingsObject.GetComponent<SongSettings>();
        OnSongChanged();
    }

    public void OnSongChanged()
    {
        if (songSettings.CurrentSong == null)
        {
            return;
        }

        var difficulties = songSettings.CurrentSong.Difficulties;
        difficultyIndex = Mathf.Max(0, difficulties.IndexOf(songSettings.CurrentSong.SelectedDifficulty));

        Refresh();
    }

    public void Before()
    {
        var difficulties = songSettings.CurrentSong.Difficulties;
        difficultyIndex = (difficultyIndex - 1 + difficulties.Count) % difficulties.Count;
        Refresh();
    }

    public void Next()
    {
        var difficulties = songSettings.CurrentSong.Difficulties;
        difficultyIndex = (difficultyIndex + 1) % difficulties.Count;
        Refresh();
    }

    public void Refresh()
    {
        var difficulty = songSettings.CurrentSong.Difficulties[difficultyIndex];
        DifficultyLabel.text = difficulty;

        var songId = System.IO.Path.GetFileName(songSettings.CurrentSong.Path);
        var scores = LeaderboardRepository.GetTopScores(songId, difficulty, Rows.Length);

        for (var i = 0; i < Rows.Length; i++)
        {
            if (i < scores.Count)
            {
                Rows[i].Rank.text = RankLabels[i];
                Rows[i].Name.text = scores[i].PlayerName;
                Rows[i].Score.text = scores[i].Score.ToString();
            }
            else
            {
                Rows[i].Rank.text = RankLabels[i];
                Rows[i].Name.text = "-";
                Rows[i].Score.text = "-";
            }
        }
    }
}