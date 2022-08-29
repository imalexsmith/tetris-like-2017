using System.Collections.Generic;
using UnityEngine;


public class Figure : UniqueComponentAtObject
{
    public enum Types
    {
        None,
        O,
        I,
        S,
        Z,
        L,
        J,
        T
    }


    // ===========================================================================================
#if UNITY_EDITOR
    [ReadOnly, SerializeField, Tooltip("Maximum number of blocks in figure")]
    private int _figureBlocksCount = FigureBlocksCount;
#endif
    public const int FigureBlocksCount = 16;
#if UNITY_EDITOR
    [ReadOnly, SerializeField, Tooltip("Maximum number of blocks in a lines, horizontal and vertical")]
    private int _lineBlocksCount = LineBlocksCount;
#endif
    public const int LineBlocksCount = 4;
    public const float HalfBoundSize = LineBlocksCount / 2F;

    [ReadOnly, SerializeField]
    private int _positionX;
    public int PositionX
    {
        get { return _positionX; }
        set
        {
            _positionX = value;
            transform.localPosition = new Vector3(_positionX, _positionY);
        }
    }
    [ReadOnly, SerializeField]
    private int _positionY;
    public int PositionY
    {
        get { return _positionY; }
        set
        {
            _positionY = value;
            transform.localPosition = new Vector3(_positionX, _positionY);
        }
    }
    [ReadOnly, SerializeField]
    private Types _figureType;
    public Types FigureType {
        get { return _figureType; }
        private set { _figureType = value; }
    }
    public float VisibleSizeX { get; private set; }
    public float VisibleSizeY { get; private set; }
    public int Rotation { get; private set; }
    public SquareBlock OTypeBlockPrefab;
    public SquareBlock ITypeBlockPrefab;
    public SquareBlock STypeBlockPrefab;
    public SquareBlock ZTypeBlockPrefab;
    public SquareBlock LTypeBlockPrefab;
    public SquareBlock JTypeBlockPrefab;
    public SquareBlock TTypeBlockPrefab;
    public SquareBlock ShadowBlockPrefab;
    public SquareBlock[] Blocks = new SquareBlock[LineBlocksCount * LineBlocksCount];

    private SquareBlock _blockPrefab;



    // ===========================================================================================
    public Figure Destroy()
    {
        if (Application.isEditor && !Application.isPlaying)
            DestroyImmediate(gameObject);
        else
            Destroy(gameObject);

        return null;
    }

    public void Clear(bool animate)
    {
        _blockPrefab = null;
        Rotation = 0;

        for (int i = 0; i < FigureBlocksCount; i++)
            Blocks[i] = null;

        var b = GetComponentsInChildren<SquareBlock>(true);
        for (int i = b.Length - 1; i >= 0; i--)
            b[i].Destroy(animate);

        FigureType = Types.None;
    }

    public void CreateRandom()
    {
        var t = 1 + new System.Random().Next() % 7;
        Create((Types)t);
    }

    public void Create(Types fType, bool isShadow = false)
    {
        Clear(!isShadow);

        switch (fType)
        {
            case Types.O:
                _blockPrefab = !isShadow ? OTypeBlockPrefab : ShadowBlockPrefab;
                Blocks[5] = NewBlock(1, 1);
                Blocks[6] = NewBlock(2, 1);
                Blocks[9] = NewBlock(1, 2);
                Blocks[10] = NewBlock(2, 2);
                VisibleSizeX = 2F; VisibleSizeY = 2F;
                break;
            case Types.I:
                _blockPrefab = !isShadow ? ITypeBlockPrefab : ShadowBlockPrefab;
                Blocks[8] = NewBlock(0, 2);
                Blocks[9] = NewBlock(1, 2);
                Blocks[10] = NewBlock(2, 2);
                Blocks[11] = NewBlock(3, 2);
                VisibleSizeX = 4F; VisibleSizeY = 1F;
                break;
            case Types.S:
                _blockPrefab = !isShadow ? STypeBlockPrefab : ShadowBlockPrefab;
                Blocks[5] = NewBlock(1, 1);
                Blocks[6] = NewBlock(2, 1);
                Blocks[10] = NewBlock(2, 2);
                Blocks[11] = NewBlock(3, 2);
                VisibleSizeX = 3F; VisibleSizeY = 2F;
                break;
            case Types.Z:
                _blockPrefab = !isShadow ? ZTypeBlockPrefab : ShadowBlockPrefab;
                Blocks[6] = NewBlock(2, 1);
                Blocks[7] = NewBlock(3, 1);
                Blocks[9] = NewBlock(1, 2);
                Blocks[10] = NewBlock(2, 2);
                VisibleSizeX = 3F; VisibleSizeY = 2F;
                break;
            case Types.L:
                _blockPrefab = !isShadow ? LTypeBlockPrefab : ShadowBlockPrefab;
                Blocks[5] = NewBlock(1, 1);
                Blocks[9] = NewBlock(1, 2);
                Blocks[10] = NewBlock(2, 2);
                Blocks[11] = NewBlock(3, 2);
                VisibleSizeX = 3F; VisibleSizeY = 2F;
                break;
            case Types.J:
                _blockPrefab = !isShadow ? JTypeBlockPrefab : ShadowBlockPrefab;
                Blocks[7] = NewBlock(3, 1);
                Blocks[9] = NewBlock(1, 2);
                Blocks[10] = NewBlock(2, 2);
                Blocks[11] = NewBlock(3, 2);
                VisibleSizeX = 3F; VisibleSizeY = 2F;
                break;
            case Types.T:
                _blockPrefab = !isShadow ? TTypeBlockPrefab : ShadowBlockPrefab;
                Blocks[6] = NewBlock(2, 1);
                Blocks[9] = NewBlock(1, 2);
                Blocks[10] = NewBlock(2, 2);
                Blocks[11] = NewBlock(3, 2);
                VisibleSizeX = 3F; VisibleSizeY = 2F;
                break;
        }

        FigureType = fType;
    }

    public void RotateClockwise()
    {
        switch (FigureType)
        {
            case Types.O:
                return;
            case Types.I:
                RotateAnticlockwise();
                return;
            case Types.S:
                RotateAnticlockwise();
                return;
            case Types.Z:
                RotateAnticlockwise();
                return;
            case Types.L:
                switch (Rotation)
                {
                    case 0:
                        Blocks[13] = Blocks[5];
                        Blocks[5] = null;
                        Blocks[14] = Blocks[9];
                        Blocks[9] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[6] = Blocks[11];
                        Blocks[11] = null;
                        Rotation = 3;
                        break;
                    case 1:
                        Blocks[5] = Blocks[7];
                        Blocks[7] = null;
                        Blocks[9] = Blocks[6];
                        Blocks[6] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[11] = Blocks[14];
                        Blocks[14] = null;
                        Rotation = 0;
                        break;
                    case 2:
                        Blocks[7] = Blocks[15];
                        Blocks[15] = null;
                        Blocks[6] = Blocks[11];
                        Blocks[11] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[14] = Blocks[9];
                        Blocks[9] = null;
                        Rotation = 1;
                        break;
                    case 3:
                        Blocks[15] = Blocks[13];
                        Blocks[13] = null;
                        Blocks[11] = Blocks[14];
                        Blocks[14] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[9] = Blocks[6];
                        Blocks[6] = null;
                        Rotation = 2;
                        break;
                }
                break;
            case Types.J:
                switch (Rotation)
                {
                    case 0:
                        Blocks[5] = Blocks[7];
                        Blocks[7] = null;
                        Blocks[6] = Blocks[11];
                        Blocks[11] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[14] = Blocks[9];
                        Blocks[9] = null;
                        Rotation = 3;
                        break;
                    case 1:
                        Blocks[7] = Blocks[15];
                        Blocks[15] = null;
                        Blocks[11] = Blocks[14];
                        Blocks[14] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[9] = Blocks[6];
                        Blocks[6] = null;
                        Rotation = 0;
                        break;
                    case 2:
                        Blocks[15] = Blocks[13];
                        Blocks[13] = null;
                        Blocks[14] = Blocks[9];
                        Blocks[9] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[6] = Blocks[11];
                        Blocks[11] = null;
                        Rotation = 1;
                        break;
                    case 3:
                        Blocks[13] = Blocks[5];
                        Blocks[5] = null;
                        Blocks[9] = Blocks[6];
                        Blocks[6] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[11] = Blocks[14];
                        Blocks[14] = null;
                        Rotation = 2;
                        break;
                }
                break;
            case Types.T:
                switch (Rotation)
                {
                    case 0:
                        Blocks[6] = Blocks[6];
                        Blocks[14] = Blocks[11];
                        Blocks[11] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[9] = Blocks[9];
                        Rotation = 3;
                        break;
                    case 1:
                        Blocks[9] = Blocks[14];
                        Blocks[14] = null;
                        Blocks[6] = Blocks[6];
                        Blocks[10] = Blocks[10];
                        Blocks[11] = Blocks[11];
                        Rotation = 0;
                        break;
                    case 2:
                        Blocks[11] = Blocks[11];
                        Blocks[14] = Blocks[14];
                        Blocks[10] = Blocks[10];
                        Blocks[6] = Blocks[9];
                        Blocks[9] = null;
                        Rotation = 1;
                        break;
                    case 3:
                        Blocks[14] = Blocks[14];
                        Blocks[9] = Blocks[9];
                        Blocks[11] = Blocks[6];
                        Blocks[6] = null;
                        Blocks[10] = Blocks[10];
                        Rotation = 2;
                        break;
                }
                break;
        }

        UpdateBlocks();
        SwitchVisibleSizes();
    }

    public void RotateAnticlockwise()
    {
        switch (FigureType)
        {
            case Types.O:
                return;
            case Types.I:
                if (Rotation == 0)
                {
                    Blocks[2] = Blocks[8];
                    Blocks[8] = null;
                    Blocks[6] = Blocks[9];
                    Blocks[9] = null;
                    Blocks[10] = Blocks[10];
                    Blocks[14] = Blocks[11];
                    Blocks[11] = null;
                    Rotation = 1;
                }
                else
                {
                    Blocks[8] = Blocks[2];
                    Blocks[2] = null;
                    Blocks[9] = Blocks[6];
                    Blocks[6] = null;
                    Blocks[10] = Blocks[10];
                    Blocks[11] = Blocks[14];
                    Blocks[14] = null;
                    Rotation = 0;
                }
                break;
            case Types.S:
                if (Rotation == 0)
                {
                    Blocks[7] = Blocks[5];
                    Blocks[5] = null;
                    Blocks[10] = Blocks[10];
                    Blocks[11] = Blocks[11];
                    Blocks[14] = Blocks[6];
                    Blocks[6] = null;
                    Rotation = 1;
                }
                else
                {
                    Blocks[5] = Blocks[7];
                    Blocks[7] = null;
                    Blocks[10] = Blocks[10];
                    Blocks[11] = Blocks[11];
                    Blocks[6] = Blocks[14];
                    Blocks[14] = null;
                    Rotation = 0;
                }
                break;
            case Types.Z:
                if (Rotation == 0)
                {
                    Blocks[6] = Blocks[6];
                    Blocks[15] = Blocks[7];
                    Blocks[7] = null;
                    Blocks[10] = Blocks[10];
                    Blocks[11] = Blocks[9];
                    Blocks[9] = null;
                    Rotation = 1;
                }
                else
                {
                    Blocks[6] = Blocks[6];
                    Blocks[7] = Blocks[15];
                    Blocks[15] = null;
                    Blocks[10] = Blocks[10];
                    Blocks[9] = Blocks[11];
                    Blocks[11] = null;
                    Rotation = 0;
                }
                break;
            case Types.L:
                switch (Rotation)
                {
                    case 0:
                        Blocks[7] = Blocks[5];
                        Blocks[5] = null;
                        Blocks[6] = Blocks[9];
                        Blocks[9] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[14] = Blocks[11];
                        Blocks[11] = null;
                        Rotation = 1;
                        break;
                    case 1:
                        Blocks[15] = Blocks[7];
                        Blocks[7] = null;
                        Blocks[11] = Blocks[6];
                        Blocks[6] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[9] = Blocks[14];
                        Blocks[14] = null;
                        Rotation = 2;
                        break;
                    case 2:
                        Blocks[13] = Blocks[15];
                        Blocks[15] = null;
                        Blocks[14] = Blocks[11];
                        Blocks[11] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[6] = Blocks[9];
                        Blocks[9] = null;
                        Rotation = 3;
                        break;
                    case 3:
                        Blocks[5] = Blocks[13];
                        Blocks[13] = null;
                        Blocks[9] = Blocks[14];
                        Blocks[14] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[11] = Blocks[6];
                        Blocks[6] = null;
                        Rotation = 0;
                        break;
                }
                break;
            case Types.J:
                switch (Rotation)
                {
                    case 0:
                        Blocks[15] = Blocks[7];
                        Blocks[7] = null;
                        Blocks[14] = Blocks[11];
                        Blocks[11] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[6] = Blocks[9];
                        Blocks[9] = null;
                        Rotation = 1;
                        break;
                    case 1:
                        Blocks[13] = Blocks[15];
                        Blocks[15] = null;
                        Blocks[9] = Blocks[14];
                        Blocks[14] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[11] = Blocks[6];
                        Blocks[6] = null;
                        Rotation = 2;
                        break;
                    case 2:
                        Blocks[5] = Blocks[13];
                        Blocks[13] = null;
                        Blocks[6] = Blocks[9];
                        Blocks[9] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[14] = Blocks[11];
                        Blocks[11] = null;
                        Rotation = 3;
                        break;
                    case 3:
                        Blocks[7] = Blocks[5];
                        Blocks[5] = null;
                        Blocks[11] = Blocks[6];
                        Blocks[6] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[9] = Blocks[14];
                        Blocks[14] = null;
                        Rotation = 0;
                        break;
                }
                break;
            case Types.T:
                switch (Rotation)
                {
                    case 0:
                        Blocks[6] = Blocks[6];
                        Blocks[14] = Blocks[9];
                        Blocks[9] = null;
                        Blocks[10] = Blocks[10];
                        Blocks[11] = Blocks[11];
                        Rotation = 1;
                        break;
                    case 1:
                        Blocks[9] = Blocks[6];
                        Blocks[6] = null;
                        Blocks[14] = Blocks[14];
                        Blocks[10] = Blocks[10];
                        Blocks[11] = Blocks[11];
                        Rotation = 2;
                        break;
                    case 2:
                        Blocks[9] = Blocks[9];
                        Blocks[14] = Blocks[14];
                        Blocks[10] = Blocks[10];
                        Blocks[6] = Blocks[11];
                        Blocks[11] = null;
                        Rotation = 3;
                        break;
                    case 3:
                        Blocks[6] = Blocks[6];
                        Blocks[9] = Blocks[9];
                        Blocks[11] = Blocks[14];
                        Blocks[14] = null;
                        Blocks[10] = Blocks[10];
                        Rotation = 0;
                        break;
                }
                break;
        }

        UpdateBlocks();
        SwitchVisibleSizes();
    }

    public void RotateTo(int newRotation)
    {
        if (Rotation == newRotation)
            return;

        if (newRotation < 0 || newRotation > 3)
            RotateTo(newRotation % 4);
        else
        {
            var count = 4 - Mathf.Abs(Rotation - newRotation);
            for (int i = 0; i < count; i++)
                RotateAnticlockwise();
        }
    }

    public void MoveCenterToParentOrigin()
    {
        transform.localPosition = -new Vector3(HalfBoundSize + VisibleSizeX % 2F / 2F
                   , HalfBoundSize + VisibleSizeY % 2F / 2F
                   , 0F);
    }

    protected override void Awake()
    {
        base.Awake();

        name = "Figure" + GetInstanceID();
    }

    protected void OnEnable()
    {
        if (FigureType == Types.None)
        {
            CreateRandom();
            MoveCenterToParentOrigin();
        }
    }


    // ===========================================================================================
    private SquareBlock NewBlock(int x, int y)
    {
        var newBlock = _blockPrefab != null ? Instantiate(_blockPrefab) : new GameObject().AddComponent<SquareBlock>();
        newBlock.name = string.Format("B{0}{1}", x, y);
        newBlock.transform.parent = transform;
        newBlock.transform.localScale = Vector3.one;
        newBlock.LocalPositionX = x;
        newBlock.LocalPositionY = y;

        return newBlock;
    }

    private void UpdateBlocks()
    {
        int indx;
        for (int y = 0; y < LineBlocksCount; y++)
            for (int x = 0; x < LineBlocksCount; x++)
            {
                indx = y*LineBlocksCount + x;
                if (Blocks[indx] != null)
                {
                    Blocks[indx].name = string.Format("B{0}{1}", x, y);
                    Blocks[indx].LocalPositionX = x;
                    Blocks[indx].LocalPositionY = y;
                }
            }
    }

    private void SwitchVisibleSizes()
    {
        var temp = VisibleSizeX;
        VisibleSizeX = VisibleSizeY;
        VisibleSizeY = temp;
    }
}