using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Text))]
public class BestScoreDrawer : MonoBehaviour
{
    // ===========================================================================================
    public Color YouAreTheBestColor;

    private Text _text;
    private Color _startColor;
    private LevelSettings _levelSettings;
    private BestScore _bestScore;


    // ===========================================================================================
    protected void Awake()
    {
        _text = GetComponent<Text>();
        _startColor = _text.color;
    }

    protected void Start()
    {
        _levelSettings = LevelSettings.Instance;
        _bestScore = BestScore.Instance;
    }

    protected void Update()
    {
        if (_levelSettings.Score < _bestScore.Records[0].Score)
        {
            _text.text = _bestScore.Records[0].Score.ToString();
            _text.color = _startColor;
        }
        else
        {
            _text.text = _levelSettings.Score.ToString();
            _text.color = YouAreTheBestColor;
        }
    }
}
