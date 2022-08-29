using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;


[AddComponentMenu("Unique/LevelSettings")]
public class LevelSettings : UniqueComponentAtScene<LevelSettings>
{
    // ===========================================================================================
    public const int MinLevel = 1;
    public const int MaxLevel = 10;

    public string PlayerName = "Player";
    public bool IsPause { get; private set; }
    public bool DrawShadow = true;
    public bool GlobalEffectsSwitch = true;
    private bool _lastGlobalEffectsSwitch = true;
    public bool DrawGrid = true;
    private bool _lastDrawGrid = true;
    public SpriteRenderer FieldGrid;
    public bool HideCursorInGame = true;
    public float HideCursorDelay = 2F;
    [Range(MinLevel, MaxLevel)]
    public int LevelNumber = 1;
    private int _lastLevelNumber = 1;
    [ReadOnly, SerializeField]
    public float TimeToMove = 0.5F;
    public float TimeToFinalize = 0.7F;
    public float TimeToRotate = 0.2F;
    public float TimeToMoveSide = 0.05F;
    public float TimeToSlideDown = 0.05F;
    public float DelayBeforeStartRepeating = 0.3F;
    [ReadOnly, SerializeField]
    private int _nextLevelLinesLimit = 10;
    public int NextLevelLinesLimit
    {
        get { return _nextLevelLinesLimit; }
    }
    public int LinesRemoved;
    [ReadOnly, SerializeField]
    public int Score;
    public int RemoveSingleLineScore = 100;
    public int RemoveTwoLineScore = 220;
    public int RemoveThreeLineScore = 360;
    public int RemoveFourLineScore = 600;
    public int DropFigureScoreMultiplier = 2;
    public BlurOptimized BlurOnCamera;
    public float WaitTime = 3F;
    public AnimationCurve WaitNumberShowCurve;
    public Text WaitNumberLabel;
    public AudioClip LevelUpSound;

    private AudioSource _levelUpSoundPlayer;

    private Vector3 _lastMousePosition = Vector3.zero;
    private float _lastCursorMoveTime;
    private bool _isWaitBeforeResume;
    private float _currentWaitTime;


    // ===========================================================================================
    public void Save()
    {
        PlayerPrefs.SetFloat("volume", AudioListener.volume);
        PlayerPrefs.SetString("player_name", PlayerName);
        PlayerPrefs.SetString("draw_shadow", DrawShadow.ToString());
        PlayerPrefs.SetString("global_effects_switch", GlobalEffectsSwitch.ToString());
        PlayerPrefs.SetString("draw_grid", DrawGrid.ToString());
        PlayerPrefs.SetString("hide_cursor_in_game", HideCursorInGame.ToString());
        PlayerPrefs.SetFloat("hide_cursor_delay", HideCursorDelay);

        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("volume"))
            AudioListener.volume = PlayerPrefs.GetFloat("volume", 1F);

        if (PlayerPrefs.HasKey("player_name"))
            PlayerName = PlayerPrefs.GetString("player_name", BestScoreRecord.NoNameCap);

        if (PlayerPrefs.HasKey("draw_shadow"))
            bool.TryParse(PlayerPrefs.GetString("draw_shadow"), out DrawShadow);

        if (PlayerPrefs.HasKey("global_effects_switch"))
            bool.TryParse(PlayerPrefs.GetString("global_effects_switch"), out GlobalEffectsSwitch);

        if (PlayerPrefs.HasKey("draw_grid"))
            bool.TryParse(PlayerPrefs.GetString("draw_grid"), out DrawGrid);

        if (PlayerPrefs.HasKey("hide_cursor_in_game"))
            bool.TryParse(PlayerPrefs.GetString("hide_cursor_in_game"), out HideCursorInGame);

        if (PlayerPrefs.HasKey("hide_cursor_delay"))
            HideCursorDelay = PlayerPrefs.GetFloat("hide_cursor_delay", 2F);
    }

    public void Clear()
    {
        IsPause = true;
        LevelNumber = 1;
        _nextLevelLinesLimit = 10;
        LinesRemoved = 0;
        Score = 0;
    }

    public void Pause()
    {
        if (BlurOnCamera != null)
            BlurOnCamera.enabled = true & GlobalEffectsSwitch;
        _isWaitBeforeResume = false;
        IsPause = true;
        if (WaitNumberLabel != null)
            WaitNumberLabel.gameObject.SetActive(false);
    }

    public void Resume()
    {
        if (BlurOnCamera != null)
            BlurOnCamera.enabled = false;
        _isWaitBeforeResume = true;
        _currentWaitTime = 0F;
        if (WaitNumberLabel != null)
            WaitNumberLabel.gameObject.SetActive(true);
    }

    protected override void Awake()
    {
        base.Awake();

        if (BlurOnCamera == null)
        {
            var blurs = new List<BlurOptimized>();
            for (int i = 0; i < Camera.allCamerasCount; i++)
            {
                var blur = Camera.allCameras[i].GetComponent<BlurOptimized>();
                if (blur != null)
                    blurs.Add(blur);
            }
            if (blurs.Count == 1)
                BlurOnCamera = blurs[0];
        }

        if (Application.isEditor && !Application.isPlaying) return;

        _levelUpSoundPlayer = gameObject.AddComponent<AudioSource>();
        _levelUpSoundPlayer.clip = LevelUpSound;
        _levelUpSoundPlayer.priority = 0;

        Load();
    }

    protected void Update()
    {
        if (FieldGrid != null)
        {
            if (DrawGrid && !_lastDrawGrid)
                FieldGrid.gameObject.SetActive(true);

            if (!DrawGrid && _lastDrawGrid)
                FieldGrid.gameObject.SetActive(false);
        }
        _lastDrawGrid = DrawGrid;

        if (HideCursorInGame)
        {
            if (_lastMousePosition != Input.mousePosition)
            {
                Cursor.visible = true;
                _lastCursorMoveTime = 0;
            }
            else _lastCursorMoveTime += Time.deltaTime;

            if (_lastCursorMoveTime >= HideCursorDelay)
                Cursor.visible = false;

            _lastMousePosition = Input.mousePosition;
        }

        if (!GlobalEffectsSwitch && _lastGlobalEffectsSwitch)
        {
            for (int i = 0; i < Camera.allCameras.Length; i++)
                foreach (var postEffect in Camera.allCameras[i].GetComponents<PostEffectsBase>())
                    postEffect.enabled = false;
        }

        if (GlobalEffectsSwitch && !_lastGlobalEffectsSwitch)
        {
            for (int i = 0; i < Camera.allCameras.Length; i++)
                foreach (var postEffect in Camera.allCameras[i].GetComponents<Bloom>())
                    postEffect.enabled = true;
        }

        _lastGlobalEffectsSwitch = GlobalEffectsSwitch;

        if (!IsPause)
        {
            if (LinesRemoved >= _nextLevelLinesLimit)
            {
                LevelNumber++;
                LevelNumber = Mathf.Clamp(LevelNumber, MinLevel, MaxLevel);
            }

            if (LevelNumber != _lastLevelNumber)
            {
                _nextLevelLinesLimit = LevelNumber * 10;
                TimeToMove = (MaxLevel + 1 - LevelNumber) * 0.05F;
                if (_levelUpSoundPlayer != null)
                    _levelUpSoundPlayer.Play();
            }

            _lastLevelNumber = LevelNumber;
        }

        if (_isWaitBeforeResume)
        {
            if (_currentWaitTime < WaitTime)
            {
                if (WaitNumberLabel != null)
                {
                    WaitNumberLabel.text = ((int)(WaitTime - _currentWaitTime)).ToString();
                    var scaleFactor = WaitNumberShowCurve.Evaluate(_currentWaitTime);
                    WaitNumberLabel.gameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1F);
                }
            }
            else
            {
                _isWaitBeforeResume = false;
                IsPause = false;
                if (WaitNumberLabel != null)
                    WaitNumberLabel.gameObject.SetActive(false);
            }
            _currentWaitTime += Time.deltaTime;
        }

    }

    protected void OnApplicationFocus(bool focus)
    {
        if (!focus)
            Pause();
        else if (MenuBase.GetOpenedMenus.Count == 0)
            Resume();
    }

    protected override void OnDestroy()
    {
        if (Application.isEditor && !Application.isPlaying)
            DestroyImmediate(_levelUpSoundPlayer);
        else
            Destroy(_levelUpSoundPlayer);

        _levelUpSoundPlayer = null;

        base.OnDestroy();
    }
}
