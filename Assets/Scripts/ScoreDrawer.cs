using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof (Text))]
public class ScoreDrawer : MonoBehaviour
{
    // ===========================================================================================
    private Text _text;
    private LevelSettings _levelSettings;


    // ===========================================================================================
    protected void Awake()
    {
        _text = GetComponent<Text>();
    }

    protected void Start()
    {
        _levelSettings = LevelSettings.Instance;
    }

    protected void Update()
    {
        _text.text = _levelSettings.Score.ToString();
    }
}
