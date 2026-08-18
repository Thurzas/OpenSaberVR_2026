using System;
using System.Collections.Generic;

[Serializable]
public class LeaderboardEntry
{
    public string PlayerName;
    public int Score;
}

[Serializable]
public class DifficultyLeaderboard
{
    public string Difficulty;
    public List<LeaderboardEntry> Entries = new List<LeaderboardEntry>();
}

[Serializable]
public class SongLeaderboard
{
    public string SongId;
    public List<DifficultyLeaderboard> Difficulties = new List<DifficultyLeaderboard>();
}