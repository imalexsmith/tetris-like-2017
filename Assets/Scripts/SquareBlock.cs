using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class SquareBlock : UniqueComponentAtObject
{
    // ===========================================================================================
    [ReadOnly, SerializeField]
    private int _localPositionX;
    public int LocalPositionX
    {
        get { return _localPositionX; }
        set {
            _localPositionX = value;
            transform.localPosition = new Vector3(_localPositionX + 0.5F, _localPositionY + 0.5F);
        }
    }
    [ReadOnly, SerializeField]
    private int _localPositionY;
    public int LocalPositionY
    {
        get { return _localPositionY; }
        set {
            _localPositionY = value;
            transform.localPosition = new Vector3(_localPositionX + 0.5F, _localPositionY + 0.5F);
        }
    }
    public bool ColoredAnimation;
    public ParticleSystem RemoveAnimation;
    [Range(0F, 2F)]
    public float RemoveAnimationTime = 0.33F;
    public ParticleSystem PlacedAnimation;
    [Range(0F, 2F)]
    public float PlacedAnimationTime = 0.33F;
    public ParticleSystem DropAnimation;
    [Range(0F, 2F)]
    public float DropAnimationTime = 0.33F;

    private SpriteRenderer _renderer;


    // ===========================================================================================
    public SquareBlock Destroy(bool animate)
    {
        if (Application.isEditor && !Application.isPlaying)
            DestroyImmediate(gameObject);
        else
        {
            if (animate && RemoveAnimation != null)
            {
                var animationGameObj = Instantiate(RemoveAnimation.gameObject);
                animationGameObj.transform.position = transform.position;

                var animationSystem = animationGameObj.GetComponent<ParticleSystem>();
                var animationRenderer = animationSystem.GetComponent<ParticleSystemRenderer>();
                if (ColoredAnimation) animationSystem.startColor = _renderer.color;
                animationRenderer.sortingLayerName = _renderer.sortingLayerName;
                animationRenderer.sortingOrder = _renderer.sortingOrder + 1;

                animationSystem.Play();
                Destroy(animationGameObj, RemoveAnimationTime);
            }

            Object.Destroy(gameObject);
        }

        return null;
    }

    public void Place(bool animate)
    {
        if (Application.isEditor && !Application.isPlaying) return;

        if (animate && PlacedAnimation != null)
        {
            var animationGameObj = Instantiate(PlacedAnimation.gameObject);
            animationGameObj.transform.SetParent(transform, false);

            var animationSystem = animationGameObj.GetComponent<ParticleSystem>();
            var animationRenderer = animationSystem.GetComponent<ParticleSystemRenderer>();
            if (ColoredAnimation) animationSystem.startColor = _renderer.color;
            animationRenderer.sortingLayerName = _renderer.sortingLayerName;
            animationRenderer.sortingOrder = _renderer.sortingOrder + 1;

            animationSystem.Play();
            Destroy(animationGameObj, PlacedAnimationTime);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _renderer = GetComponent<SpriteRenderer>();
    }

    protected void Start()
    {
        _renderer.sortingLayerName = "Figures";
    }
}
