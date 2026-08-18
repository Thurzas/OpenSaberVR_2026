using System.IO;
using System.Linq;
using UnityEngine;

public static class LeaderboardRepository
{
    private const int MaxStoredEntriesPerDifficulty = 10;
    private static readonly string LeaderboardsFolder = Path.Combine(Application.dataPath, "Leaderboards");

    public static System.Collections.Generic.List<LeaderboardEntry> GetTopScores(string songId, string difficulty, int count)
    {
        var songBoard = Load(songId);
        var difficultyBoard = songBoard.Difficulties.FirstOrDefault(d => d.Difficulty == difficulty);

        if (difficultyBoard == null)
        {
            return new System.Collections.Generic.List<LeaderboardEntry>();
        }

        return difficultyBoard.Entries
            .OrderByDescending(e => e.Score)
            .Take(count)
            .ToList();
    }

    public static bool IsNewRecord(string songId, string difficulty, int score, int count)
    {
        var scores = GetTopScores(songId, difficulty, count);
        return scores.Count < count || score > scores.Min(e => e.Score);
    }

    public static void SubmitScore(string songId, string difficulty, string playerName, int score)
    {
        var songBoard = Load(songId);
        var difficultyBoard = songBoard.Difficulties.FirstOrDefault(d => d.Difficulty == difficulty);

        if (difficultyBoard == null)
        {
            difficultyBoard = new DifficultyLeaderboard { Difficulty = difficulty };
            songBoard.Difficulties.Add(difficultyBoard);
        }

        difficultyBoard.Entries.Add(new LeaderboardEntry { PlayerName = playerName, Score = score });
        difficultyBoard.Entries = difficultyBoard.Entries
            .OrderByDescending(e => e.Score)
            .Take(MaxStoredEntriesPerDifficulty)
            .ToList();

        Save(songBoard);
    }

    private static SongLeaderboard Load(string songId)
    {
        var path = GetFilePath(songId);

        if (!File.Exists(path))
        {
            return new SongLeaderboard { SongId = songId };
        }

        var json = File.ReadAllText(path);
        return JsonUtility.FromJson<SongLeaderboard>(json);
    }

    private static void Save(SongLeaderboard songBoard)
    {
        Directory.CreateDirectory(LeaderboardsFolder);
        var json = JsonUtility.ToJson(songBoard, true);
        File.WriteAllText(GetFilePath(songBoard.SongId), json);
    }

    private static string GetFilePath(string songId)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeName = string.Join("_", songId.Split(invalidChars));
        return Path.Combine(LeaderboardsFolder, safeName + ".json");
    }
}