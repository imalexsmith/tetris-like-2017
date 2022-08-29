using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class RestartLevelMenu : FadeInOut2DMenu
{
    // ===========================================================================================
    public Text PlayerNameLabel;
    public Text PlayerScoreLabel;
    public Text BestScoreLabel;

    private LevelSettings _levelSettings;
    private BestScore _bestScore;
    private GameField _gameField;


    // ===========================================================================================
    public override void Show()
    {
        if (base.Show(LevelSettings.Instance.GlobalEffectsSwitch))
            LevelSettings.Instance.Pause();
    }

    public override void Hide()
    {
        base.Hide(LevelSettings.Instance.GlobalEffectsSwitch);
    }

    public override void OnHided()
    {
        base.OnHided();
        if (OpenedMenus.Count == 0)
            LevelSettings.Instance.Resume();
    }

    public void RestartLevel()
    {
        if (!_gameField.IsGameOver)
            _bestScore.Add(new BestScoreRecord(_levelSettings.PlayerName, _levelSettings.Score));

        _bestScore.Save();
        _gameField.Create();
        
        Hide();
    }
    
    protected override void Awake()
    { 
        base.Awake();

        if (PlayerNameLabel == null)
        {
            var texts = GetComponentsInChildren<Text>(true).Where(x => x.name.ToLower().Contains("playernamelabel")).ToList();
            if (texts.Count != 1)
                Debug.LogError("RestartLevelMenu script: PlayerNameLabel not found or found too much");
            else
                PlayerNameLabel = texts[0];
        }

        if (PlayerScoreLabel == null)
        {
            var texts = GetComponentsInChildren<Text>(true).Where(x => x.name.ToLower().Contains("playerscorelabel")).ToList();
            if (texts.Count != 1)
                Debug.LogError("RestartLevelMenu script: PlayerScoreLabel not found or found too much");
            else
                PlayerScoreLabel = texts[0];
        }

        if (BestScoreLabel == null)
        {
            var texts = GetComponentsInChildren<Text>(true).Where(x => x.name.ToLower().Contains("bestscorelabel")).ToList();
            if (texts.Count != 1)
                Debug.LogError("RestartLevelMenu script: BestScoreLabel not found or found too much");
            else
                BestScoreLabel = texts[0];
        }
    }

    protected void OnEnable()
    {
        _levelSettings = LevelSettings.Instance;
        _bestScore = BestScore.Instance;
        _gameField = GameField.Instance;

        PlayerNameLabel.text = _levelSettings.PlayerName;
        PlayerScoreLabel.text = _levelSettings.Score.ToString();
        BestScoreLabel.text = "";
        for (int i = 0; i < BestScore.RecordsCount; i++)
        {
            BestScoreLabel.text += string.Format("{0}) {1} - {2}\n", i+1, _bestScore.Records[i].PlayerName, _bestScore.Records[i].Score);
        }
    }
}