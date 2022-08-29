using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


[Serializable]
public class BestScoreRecord
{
    public const string NoNameCap = "NoName";

    public string PlayerName { get; private set; }
    public int Score { get; private set; }

    public BestScoreRecord() 
        : this(NoNameCap, 0) { }

    public BestScoreRecord(string name, int score)
    {
        PlayerName = name;
        Score = score;
    }
}

/// <summary>
/// Collect simple information about players name and his score automaticaly ordered by desc. Player's name not must be unique.
/// </summary>
[AddComponentMenu("Unique/BestScore")]
public class BestScore : UniqueComponentAtScene<BestScore>
{
    // ========================================================================================
    public const int RecordsCount = 7;

    private List<BestScoreRecord> _records = new List<BestScoreRecord>(RecordsCount);
    public ReadOnlyCollection<BestScoreRecord> Records {
        get { return _records.AsReadOnly(); }
    }


    // ===========================================================================================
    public void Save()
    {
        for (int i = 0; i < RecordsCount; i++)
            PlayerPrefs.SetString("best_score_" + i, _records[i].PlayerName + "`" + _records[i].Score);

        PlayerPrefs.Save();
    }

    public void Load()
    {
        _records.Clear();
        for (int i = 0; i < RecordsCount; i++)
        {
            var pName = BestScoreRecord.NoNameCap;
            var pScore = 0;

            if (PlayerPrefs.HasKey("best_score_" + i))
            {
                var fromPrefsValue = PlayerPrefs.GetString("best_score_" + i).Split('`');
                if (fromPrefsValue.Length == 2)
                {
                    pName = fromPrefsValue[0].Length > 0 ? fromPrefsValue[0] : BestScoreRecord.NoNameCap;
                    int.TryParse(fromPrefsValue[1], out pScore);
                }
            }

            _records.Add(new BestScoreRecord(pName, pScore));
        }
    }

    public int Add(BestScoreRecord item)
    {
        for (int i = 0; i < RecordsCount; i++)
        {
            if (item.Score > _records[i].Score)
            {
                _records.Insert(i, item);
                _records.RemoveAt(RecordsCount);
                return i;
            }
        }

        return -1;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= RecordsCount)
            throw new ArgumentOutOfRangeException();

        _records.RemoveAt(index);
        _records.Add(new BestScoreRecord());
    }

    public void Clear()
    {
        _records.Clear();
        for (int i = 0; i < RecordsCount; i++)
            _records.Add(new BestScoreRecord());
    }

    protected override void Awake()
    {
        base.Awake();

        Load();
    }
}
