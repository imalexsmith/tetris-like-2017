using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("Unique/GameField")]
[RequireComponent(typeof(SpriteRenderer))]
public class GameField : UniqueComponentAtScene<GameField>
{
    // ===========================================================================================
#if UNITY_EDITOR
    [ReadOnly, SerializeField]
    private int _blocksXCount = BlocksXCount;
#endif
    public const int BlocksXCount = 10;
#if UNITY_EDITOR
    [ReadOnly, SerializeField]
    private int _blocksYCount = BlocksYCount;
#endif
    public const int BlocksYCount = 20;
#if UNITY_EDITOR
    [ReadOnly, SerializeField]
    private int _nextFigureCount = NextFigureCount;
#endif
    public const int NextFigureCount = 3;

    public bool IsGameOver { get; private set; }
    public Figure FigurePrefab;
    [ReadOnly, SerializeField]
    public Figure CurrentFigure;
    [ReadOnly, SerializeField]
    public List<Figure> NextFigure = new List<Figure>(NextFigureCount);
    [ReadOnly, SerializeField]
    public Figure HoldFigure;
    [ReadOnly, SerializeField]
    public Figure ShadowFigure;
    public List<Transform> NextFigureSpawnPoint = new List<Transform>(NextFigureCount);
    public Transform HoldFigurePoint;
    public RestartLevelMenu RestartMenu;
    public AudioClip MainAudioTheme;
    public AudioClip RemoveLineSound;
    public AudioClip HoldErrorSound;
    public SquareBlock[] Blocks = new SquareBlock[BlocksXCount*BlocksYCount];

    private SpriteRenderer _renderer;
    private AudioSource _mainAudioThemePlayer;
    private AudioSource _removeLineSoundPlayer;
    private AudioSource _holdErrorPlayer;

    private FigureInput _figureInput;
    private LevelSettings _levelSettings;
    private SlowMo _slowMo;
    private BestScore _bestScore;

    private bool _isReady;
    private bool _figureSwapped;


    // ===========================================================================================
    public void Clear()
    {
        _isReady = false;
        IsGameOver = false;
        _figureInput.ResetTimers();
        _levelSettings.Clear();
        _slowMo.Clear();

        for (int i = 0; i < BlocksXCount*BlocksYCount; i++)
            Blocks[i] = null;

        var b = GetComponentsInChildren<SquareBlock>(true);
        for (int i = b.Length - 1; i >= 0; i--)
            b[i].Destroy(_levelSettings.GlobalEffectsSwitch);

        for (int i = 0; i < NextFigureCount; i++)
            NextFigure[i] = NextFigure[i] != null ? NextFigure[i].Destroy() : null;

        CurrentFigure = CurrentFigure != null ? CurrentFigure.Destroy() : null;
        HoldFigure = HoldFigure != null ? HoldFigure.Destroy() : null;
        ShadowFigure = ShadowFigure != null ? ShadowFigure.Destroy() : null;

        _renderer.sortingLayerName = "Background";
    }

    public void Create()
    {
        Clear();

        for (int i = 0; i < NextFigureCount; i++)
            NextFigure[i] = NewNextFigure(i);

        _mainAudioThemePlayer.Play();
        _isReady = true;
    }

    protected override void Awake()
    {
        base.Awake();

        _renderer = GetComponent<SpriteRenderer>();

        if (RestartMenu == null)
        {
            var menus = this.FindObjectsOfTypeAtSceneAll<RestartLevelMenu>().Cast<RestartLevelMenu>().ToArray();
            if (menus.Length == 1)
                RestartMenu = menus[0];
        }

        if (Application.isEditor && !Application.isPlaying) return;

        _mainAudioThemePlayer = gameObject.AddComponent<AudioSource>();
        _mainAudioThemePlayer.clip = MainAudioTheme;
        _mainAudioThemePlayer.loop = true;
        _mainAudioThemePlayer.volume = 0.7F;

        _removeLineSoundPlayer = gameObject.AddComponent<AudioSource>();
        _removeLineSoundPlayer.clip = RemoveLineSound;
        _removeLineSoundPlayer.priority = 0;

        _holdErrorPlayer = gameObject.AddComponent<AudioSource>();
        _holdErrorPlayer.clip = HoldErrorSound;
        _holdErrorPlayer.priority = 0;
    }

    protected void Start()
    {
        _figureInput = FigureInput.Instance;
        _levelSettings = LevelSettings.Instance;
        _slowMo = SlowMo.Instance;
        _bestScore = BestScore.Instance;

        if (!Application.isPlaying)
            Clear();
        else
            Create();
    }

    protected void Update()
    {
        if (!Application.isPlaying || !_isReady || IsGameOver) return;

        if (CurrentFigure == null)
        {
            if (Blocks[(BlocksYCount - 1) * BlocksXCount + BlocksXCount / 2] != null)
            {
                IsGameOver = true;
                _bestScore.Add(new BestScoreRecord(_levelSettings.PlayerName, _levelSettings.Score));
                RestartMenu.Show();
                return;
            }
            CurrentFigure = NextFigure[0];
            CurrentFigure.transform.SetParent(null, false);
            CurrentFigure.PositionX = BlocksXCount / 2 - Figure.LineBlocksCount / 2;
            CurrentFigure.PositionY = BlocksYCount - Figure.LineBlocksCount + 1;
            NextFigure[0] = null;
            _figureInput.ResetTimers();
            _figureSwapped = false;
        }

        if (NextFigure[0] == null)
        {
            for (int i = 1; i < NextFigureCount; i++)
            {
                NextFigure[i - 1] = NextFigure[i];
                if (NextFigureSpawnPoint[i - 1] != null)
                {
                    NextFigure[i - 1].transform.SetParent(NextFigureSpawnPoint[i - 1], false);
                    NextFigure[i - 1].MoveCenterToParentOrigin();
                }
            }

            NextFigure[NextFigureCount - 1] = NewNextFigure(NextFigureCount - 1);
        }

        if (_levelSettings.IsPause) return;

        if (hardInput.GetKeyDown("Hold"))
        {
            if (_figureSwapped)
            {
                _holdErrorPlayer.Play();
            }
            else
            {
                _figureSwapped = true;

                if (HoldFigure == null)
                {
                    HoldFigure = CurrentFigure;
                    HoldFigure.transform.SetParent(HoldFigurePoint, false);
                    HoldFigure.RotateTo(0);
                    HoldFigure.MoveCenterToParentOrigin();
                    CurrentFigure = null;
                    ShadowFigure = ShadowFigure != null ? ShadowFigure.Destroy() : null;
                    return;
                }

                var temp = CurrentFigure;
                CurrentFigure = HoldFigure;
                HoldFigure = temp;

                HoldFigure.transform.SetParent(HoldFigurePoint, false);
                HoldFigure.RotateTo(0);
                HoldFigure.MoveCenterToParentOrigin();
                ShadowFigure = ShadowFigure != null ? ShadowFigure.Destroy() : null;

                CurrentFigure.transform.SetParent(null, false);
                CurrentFigure.PositionX = BlocksXCount/2 - Figure.LineBlocksCount/2;
                CurrentFigure.PositionY = BlocksYCount - Figure.LineBlocksCount + 1;
                _figureInput.ResetTimers();
            }
        }

        if (ShadowFigure == null)
        {
            ShadowFigure = Instantiate(FigurePrefab);
            ShadowFigure.Create(CurrentFigure.FigureType, true);
            ShadowFigure.gameObject.SetActive(_levelSettings.DrawShadow);
        }

        if (_figureInput.ApplyInputFor(CurrentFigure))
            FinalizeCurrentFigure();
        else
            UpdateShadow();

        CheckLines();
    }

    protected override void OnDestroy()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            DestroyImmediate(_mainAudioThemePlayer);
            DestroyImmediate(_removeLineSoundPlayer);
            DestroyImmediate(_holdErrorPlayer);
        }
        else
        {
            Destroy(_mainAudioThemePlayer);
            Destroy(_removeLineSoundPlayer);
            Destroy(_holdErrorPlayer);
        }

        _mainAudioThemePlayer = null;
        _removeLineSoundPlayer = null;
        _holdErrorPlayer = null;

        base.OnDestroy();
    }


    // ===========================================================================================
    private Figure NewNextFigure(int index)
    {
        var result = Instantiate(FigurePrefab);

        if (NextFigureSpawnPoint[index] != null)
            result.transform.SetParent(NextFigureSpawnPoint[index], false);

        return result;
    }

    private void UpdateShadow()
    {
        if (ShadowFigure == null)
            return;

        if (_levelSettings.DrawShadow)
        {
            if (!ShadowFigure.gameObject.activeSelf)
                ShadowFigure.gameObject.SetActive(true);

            ShadowFigure.PositionX = CurrentFigure.PositionX;
            ShadowFigure.PositionY = CurrentFigure.PositionY;
            ShadowFigure.RotateTo(CurrentFigure.Rotation);
            FigureInput.Drop(ShadowFigure);
        }
        else
        {
            if (ShadowFigure.gameObject.activeSelf)
                ShadowFigure.gameObject.SetActive(false);
        }
    }

    private void FinalizeCurrentFigure()
    {
        if (CurrentFigure == null) return;

        var currentFigureBlocks = CurrentFigure.Blocks.Where(z => z != null).ToArray();
        int x;
        int y;
        int indx;
        for (int i = 0; i < currentFigureBlocks.Length; i++)
        {
            x = CurrentFigure.PositionX + currentFigureBlocks[i].LocalPositionX;
            y = CurrentFigure.PositionY + currentFigureBlocks[i].LocalPositionY;
            indx = y*BlocksXCount + x;
            Blocks[indx] = currentFigureBlocks[i];
            Blocks[indx].transform.parent = null;
            Blocks[indx].LocalPositionX = x;
            Blocks[indx].LocalPositionY = y;
            Blocks[indx].transform.SetParent(transform, true);
            Blocks[indx].Place(_levelSettings.GlobalEffectsSwitch);
        }

        CurrentFigure = CurrentFigure != null ? CurrentFigure.Destroy() : null;
        ShadowFigure = ShadowFigure != null ? ShadowFigure.Destroy() : null;
    }

    private void CheckLines()
    {
        var linesToRemove = new List<int>();
        for (int y = 0; y < BlocksYCount; y++)
        {
            var isRemoved = true;
            for (int x = 0; x < BlocksXCount; x++)
            {
                if (Blocks[y*BlocksXCount + x] == null)
                {
                    isRemoved = false;
                    break;
                }
            }
            if (isRemoved)
                linesToRemove.Add(y);
        }

        if (linesToRemove.Count > 0)
        {
            _removeLineSoundPlayer.Play();

            for (int i = linesToRemove.Count - 1; i >= 0; i--)
                RemoveLine(linesToRemove[i]);

            switch (linesToRemove.Count)
            {
                case 1:
                    _levelSettings.Score += _levelSettings.RemoveSingleLineScore * _levelSettings.LevelNumber;
                    break;
                case 2:
                    _levelSettings.Score += _levelSettings.RemoveTwoLineScore * _levelSettings.LevelNumber;
                    break;
                case 3:
                    _levelSettings.Score += _levelSettings.RemoveThreeLineScore * _levelSettings.LevelNumber;
                    break;
                case 4:
                    _levelSettings.Score += _levelSettings.RemoveFourLineScore * _levelSettings.LevelNumber;
                    break;
            }
        }
    }

    private void RemoveLine(int lineNumber)
    {
        if (lineNumber < 0 || lineNumber >= BlocksYCount) return;

        var y = lineNumber*BlocksXCount;
        for (int x = 0; x < BlocksXCount; x++)
        {
            if (Blocks[y + x] != null)
            {
                Blocks[y + x].Destroy(_levelSettings.GlobalEffectsSwitch);
                Blocks[y + x] = null;
            }
        }

        int indx;
        for (int yy = lineNumber + 1; yy < BlocksYCount; yy++)
        {
            for (int xx = 0; xx < BlocksXCount; xx++)
            {
                indx = yy*BlocksXCount + xx;
                if (Blocks[indx] != null)
                {
                    Blocks[indx].transform.parent = null;
                    Blocks[indx].LocalPositionY--;
                    Blocks[indx].transform.SetParent(transform, true);

                    Blocks[(yy - 1)*BlocksXCount + xx] = Blocks[indx];
                    Blocks[indx] = null;
                }
            }
        }

        _levelSettings.LinesRemoved++;
    }
}
