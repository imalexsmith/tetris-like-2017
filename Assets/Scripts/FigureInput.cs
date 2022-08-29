using System.Linq;
using UnityEngine;


[AddComponentMenu("Unique/FigureInput")]
public class FigureInput : UniqueComponentAtScene<FigureInput>
{
    // ===========================================================================================
    public AudioClip DropFigureSound;
    
    internal AudioSource DropFigureSoundPlayer;

    private float _lastMoveTimer;
    private float _finalizeTimer;

    private float _rotateClockwisePressedTimer;
    private float _rotateAnticlockwisePressedTimer;
    private float _lastRotateClockwiseTimer;
    private float _lastRotateAnticlockwiseTimer;

    private float _moveRightPressedTimer;
    private float _moveLeftPressedTimer;
    private float _lastMoveRightTimer;
    private float _lastMoveLeftTimer;

    private float _moveDownPressedTimer;
    private float _lastMoveDownTimer;

    private float _deltaTime;
    private float _lastRealTime;

    private LevelSettings _levelSettings;


    // ===========================================================================================
    public static bool RotateClockwise(Figure figure)
    {
        figure.RotateClockwise();
        if (CheckAfterRotation(figure))
            return true;

        figure.RotateAnticlockwise();
        return false;
    }

    public static bool RotateAnticlockwise(Figure figure)
    {
        figure.RotateAnticlockwise();
        if (CheckAfterRotation(figure))
            return true;

        figure.RotateClockwise();
        return false;
    }

    public static bool MoveRight(Figure figure)
    {
        figure.PositionX++;
        if (CheckAfterMoveSide(figure))
            return true;

        figure.PositionX--;
        return false;
    }

    public static bool MoveLeft(Figure figure)
    {
        figure.PositionX--;
        if (CheckAfterMoveSide(figure))
            return true;

        figure.PositionX++;
        return false;
    }

    public static bool MoveDown(Figure figure)
    {
        figure.PositionY--;
        if (CheckAfterMoveDown(figure))
            return true;

        figure.PositionY++;
        return false;
    }

    public static int Drop(Figure figure)
    {
        var lines = -1;
        do
        {
            figure.PositionY--;
            lines++;
        }
        while (CheckAfterMoveDown(figure));
        figure.PositionY++;

        return lines;
    }

    private static bool CheckAfterRotation(Figure figure)
    {
        var currentFigureBlocks = figure.Blocks.Where(x => x != null).ToArray();

        var count = 0;
        var direction = 0;

        while (currentFigureBlocks.Any(x => figure.PositionX + x.LocalPositionX >= GameField.BlocksXCount))
        {
            figure.PositionX--;
            count++;
            direction = 1;
        }

        while (currentFigureBlocks.Any(x => figure.PositionX + x.LocalPositionX < 0))
        {
            figure.PositionX++;
            count++;
            direction = -1;
        }

        while (currentFigureBlocks.Any(x => figure.PositionY + x.LocalPositionY < 0))
            figure.PositionY++;

        for (int j = 0; j < currentFigureBlocks.Length; j++)
        {
            for (int i = 0; i < GameField.Instance.Blocks.Length; i++)
            {
                if (GameField.Instance.Blocks[i] != null)
                    if (GameField.Instance.Blocks[i].LocalPositionX == figure.PositionX + currentFigureBlocks[j].LocalPositionX
                        && GameField.Instance.Blocks[i].LocalPositionY == figure.PositionY + currentFigureBlocks[j].LocalPositionY)
                    {
                        for (int k = 0; k < count; k++)
                        {
                            switch (direction)
                            {
                                case 1:
                                    figure.PositionX++;
                                    break;
                                case -1:
                                    figure.PositionX--;
                                    break;
                            }
                        }
                        return false;
                    }
            }
        }

        return true;
    }

    private static bool CheckAfterMoveSide(Figure figure)
    {
        var currentFigureBlocks = figure.Blocks.Where(x => x != null).ToArray();

        if (currentFigureBlocks.Any(x => (figure.PositionX + x.LocalPositionX >= GameField.BlocksXCount) || (figure.PositionX + x.LocalPositionX < 0)))
            return false;

        for (int j = 0; j < currentFigureBlocks.Length; j++)
        {
            for (int i = 0; i < GameField.Instance.Blocks.Length; i++)
            {
                if (GameField.Instance.Blocks[i] != null)
                    if (GameField.Instance.Blocks[i].LocalPositionX == figure.PositionX + currentFigureBlocks[j].LocalPositionX
                        && GameField.Instance.Blocks[i].LocalPositionY == figure.PositionY + currentFigureBlocks[j].LocalPositionY)
                        return false;
            }
        }

        return true;
    }

    private static bool CheckAfterMoveDown(Figure figure)
    {
        var currentFigureBlocks = figure.Blocks.Where(x => x != null).ToArray();

        if (currentFigureBlocks.Any(x => figure.PositionY + x.LocalPositionY < 0))
            return false;

        for (int j = 0; j < currentFigureBlocks.Length; j++)
        {
            for (int i = 0; i < GameField.Instance.Blocks.Length; i++)
            {
                if (GameField.Instance.Blocks[i] != null)
                    if (GameField.Instance.Blocks[i].LocalPositionX == figure.PositionX + currentFigureBlocks[j].LocalPositionX
                        && GameField.Instance.Blocks[i].LocalPositionY == figure.PositionY + currentFigureBlocks[j].LocalPositionY)
                        return false;
            }
        }

        return true;
    }


    // ===========================================================================================
    public void ResetTimers()
    {
        _lastMoveTimer = 0F;
        _finalizeTimer = 0F;
        _rotateClockwisePressedTimer = 0F;
        _rotateAnticlockwisePressedTimer = 0F;
        _lastRotateClockwiseTimer = 0F;
        _lastRotateAnticlockwiseTimer = 0F;
        _moveRightPressedTimer = 0F;
        _moveLeftPressedTimer = 0F;
        _lastMoveRightTimer = 0F;
        _lastMoveLeftTimer = 0F;
        _moveDownPressedTimer = 0F;
        _lastMoveDownTimer = 0F;
    }

    public bool ApplyInputFor(Figure figure)
    {
        if (!Application.isPlaying || _levelSettings.IsPause) return false;

        _lastMoveTimer += Time.deltaTime;
        _finalizeTimer += Time.deltaTime;

        if (InputRotationFor(figure) || InputMoveSideFor(figure) || InputMoveDownFor(figure))
            _finalizeTimer = 0F;
       
        if (_lastMoveTimer >= _levelSettings.TimeToMove)
        {
            _lastMoveTimer = 0F;
            figure.PositionY--;
            if (!CheckAfterMoveDown(figure))
                figure.PositionY++;
            else
                _finalizeTimer = 0F;
        }

        return InputDropFor(figure) || _finalizeTimer >= _levelSettings.TimeToFinalize;
    }

    protected override void Awake()
    {
        base.Awake();

        if (Application.isEditor && !Application.isPlaying) return;

        DropFigureSoundPlayer = gameObject.AddComponent<AudioSource>();
        DropFigureSoundPlayer.clip = DropFigureSound;
        DropFigureSoundPlayer.priority = 0;
    }

    protected void Start()
    {
        _levelSettings = LevelSettings.Instance;
    }

    protected void Update()
    {
        _deltaTime = Time.realtimeSinceStartup - _lastRealTime;
        _lastRealTime = Time.realtimeSinceStartup;
    }

    protected override void OnDestroy()
    {
        if (Application.isEditor && !Application.isPlaying)
            DestroyImmediate(DropFigureSoundPlayer);
        else
            Destroy(DropFigureSoundPlayer);

        DropFigureSoundPlayer = null;

        base.OnDestroy();
    }


    // ===========================================================================================
    private bool InputRotationFor(Figure figure)
    {
        var wasMoved = false;


        if (hardInput.GetKeyDown("RotateClockwise") && _rotateAnticlockwisePressedTimer == 0F)
            wasMoved = RotateClockwise(figure);

        if (hardInput.GetKey("RotateClockwise") && _rotateAnticlockwisePressedTimer == 0F)
        {
            _rotateClockwisePressedTimer += _deltaTime;
            _lastRotateClockwiseTimer += _deltaTime;

            if (_rotateClockwisePressedTimer >= _levelSettings.DelayBeforeStartRepeating && _lastRotateClockwiseTimer >= _levelSettings.TimeToRotate)
            {
                _lastRotateClockwiseTimer = 0F;
                wasMoved = RotateClockwise(figure);
            }
        }
        else
        {
            _rotateClockwisePressedTimer = 0F;
            _lastRotateClockwiseTimer = 0F;
        }

        if (hardInput.GetKeyDown("RotateAnticlockwise") && _rotateClockwisePressedTimer == 0F)
            wasMoved = RotateAnticlockwise(figure);

        if (hardInput.GetKey("RotateAnticlockwise") && _rotateClockwisePressedTimer == 0F)
        {
            _rotateAnticlockwisePressedTimer += _deltaTime;
            _lastRotateAnticlockwiseTimer += _deltaTime;

            if (_rotateAnticlockwisePressedTimer >= _levelSettings.DelayBeforeStartRepeating && _lastRotateAnticlockwiseTimer >= _levelSettings.TimeToRotate)
            {
                _lastRotateAnticlockwiseTimer = 0F;
                wasMoved = RotateAnticlockwise(figure);
            }
        }
        else
        {
            _rotateAnticlockwisePressedTimer = 0F;
            _lastRotateAnticlockwiseTimer = 0F;
        }


        return wasMoved;
    }

    private bool InputMoveSideFor(Figure figure)
    {
        var wasMoved = false;


        if (hardInput.GetKeyDown("MoveRight") && _moveLeftPressedTimer == 0F)
            wasMoved = MoveRight(figure);

        if (hardInput.GetKey("MoveRight") && _moveLeftPressedTimer == 0F)
        {
            _moveRightPressedTimer += _deltaTime;
            _lastMoveRightTimer += _deltaTime;

            if (_moveRightPressedTimer >= _levelSettings.DelayBeforeStartRepeating && _lastMoveRightTimer >= _levelSettings.TimeToMoveSide)
            {
                _lastMoveRightTimer = 0F;
                wasMoved = MoveRight(figure);
            }
        }
        else
        {
            _moveRightPressedTimer = 0F;
            _lastMoveRightTimer = 0F;
        }

        if (hardInput.GetKeyDown("MoveLeft") && _moveRightPressedTimer == 0F)
            wasMoved = MoveLeft(figure);

        if (hardInput.GetKey("MoveLeft") && _moveRightPressedTimer == 0F)
        {
            _moveLeftPressedTimer += _deltaTime;
            _lastMoveLeftTimer += _deltaTime;

            if (_moveLeftPressedTimer >= _levelSettings.DelayBeforeStartRepeating && _lastMoveLeftTimer >= _levelSettings.TimeToMoveSide)
            {
                _lastMoveLeftTimer = 0F;
                wasMoved = MoveLeft(figure);
            }
        }
        else
        {
            _moveLeftPressedTimer = 0F;
            _lastMoveLeftTimer = 0F;
        }


        return wasMoved;
    }

    private bool InputMoveDownFor(Figure figure)
    {
        var wasMoved = false;


        if (hardInput.GetKeyDown("MoveDown"))
        {
            wasMoved = MoveDown(figure);
            if (wasMoved)
            {
                _lastMoveTimer = 0F;
                _levelSettings.Score += _levelSettings.LevelNumber;
            }
        }

        if (hardInput.GetKey("MoveDown"))
        {
            _moveDownPressedTimer += _deltaTime;
            _lastMoveDownTimer += _deltaTime;
            _lastMoveTimer = 0F;

            if (_moveDownPressedTimer >= _levelSettings.DelayBeforeStartRepeating && _lastMoveDownTimer >= _levelSettings.TimeToSlideDown)
            {
                _lastMoveDownTimer = 0F;
                wasMoved = MoveDown(figure);
                if (wasMoved)
                    _levelSettings.Score += _levelSettings.LevelNumber;
            }
        }
        else
        {
            _moveDownPressedTimer = 0F;
            _lastMoveDownTimer = 0F;
        }


        return wasMoved;
    }

    private bool InputDropFor(Figure figure)
    {
        if (hardInput.GetKeyDown("Drop"))
        {
            DropFigureSoundPlayer.Play();
            _levelSettings.Score += Drop(figure) * _levelSettings.DropFigureScoreMultiplier * _levelSettings.LevelNumber;

            return true;
        }

        return false;
    }
}