using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;


[AddComponentMenu("Unique/SlowMo")]
public class SlowMo : UniqueComponentAtScene<SlowMo>
{
    // ===========================================================================================
    public float SlowMoBlocksEffect = 0.25F;
    public float SlowMoAudioEffect = 0.8F;
    [ReadOnly]
    public float SlowMoTime = 5F;
    public float SlowMoMaxTime = 5F;
    public float TimeToFullSlowMo = 0.09F;
    public float TimeToFullRestoreSlowMo = 60F;
    public Bloom BloomOnCamera;
    public float BloomThresholdMultiplier = 1.9F;

    private SpriteRenderer _renderer;

    private float _slowMoPressedTimer;
    private AudioSource[] _allAffectedAudioSources = new AudioSource[0];
    private float _defaultBloomThreshold;
    private LevelSettings _levelSettings;


    // ===========================================================================================
    public void Clear()
    {
        SlowMoTime = SlowMoMaxTime;
        _slowMoPressedTimer = 0F;
        Time.timeScale = 1F;

        if (_allAffectedAudioSources.Length > 0)
        {
            for (int i = 0; i < _allAffectedAudioSources.Length; i++)
                _allAffectedAudioSources[i].pitch = 1F;
        }
        _allAffectedAudioSources = new AudioSource[0];

        if (BloomOnCamera != null)
            BloomOnCamera.bloomThreshold = _defaultBloomThreshold;
    }

    protected override void Awake()
    {
        base.Awake();

        _renderer = GetComponentInChildren<SpriteRenderer>();

        if (BloomOnCamera == null)
        {
            var blooms = new List<Bloom>();
            for (int i = 0; i < Camera.allCamerasCount; i++)
            {
                var bloom = Camera.allCameras[i].GetComponent<Bloom>();
                if (bloom != null)
                    blooms.Add(bloom);
            }
            if (blooms.Count == 1)
                BloomOnCamera = blooms[0];
        }

        if (BloomOnCamera != null)
            _defaultBloomThreshold = BloomOnCamera.bloomThreshold;
    }

    protected void Start()
    {
        _levelSettings = LevelSettings.Instance;

        Clear();
    }

    protected void Update()
    {
        if (!Application.isPlaying || _levelSettings.IsPause) return;

        if (SlowMoTime < 0)
            _slowMoPressedTimer = 0F;
        else
        {
            if (hardInput.GetKeyDown("Slowmo"))
            {
                _slowMoPressedTimer = 0F;
                Time.timeScale = 1F;

                if (_allAffectedAudioSources.Length > 0)
                {
                    for (int i = 0; i < _allAffectedAudioSources.Length; i++)
                        _allAffectedAudioSources[i].pitch = 1F;
                }
                _allAffectedAudioSources = FindObjectsOfType<AudioSource>();

                if (BloomOnCamera != null)
                    BloomOnCamera.bloomThreshold = _defaultBloomThreshold;
            }
        }

        if (hardInput.GetKey("Slowmo") && SlowMoTime > 0)
        {
            SlowMoTime -= Time.deltaTime;
            _slowMoPressedTimer += Time.deltaTime;
        }

        if (!hardInput.GetKey("Slowmo"))
        {
            SlowMoTime += SlowMoMaxTime * (Time.deltaTime / TimeToFullRestoreSlowMo);
            SlowMoTime = Mathf.Clamp(SlowMoTime, 0F, SlowMoMaxTime);
            _slowMoPressedTimer -= Time.deltaTime;
        }

        _slowMoPressedTimer = Mathf.Clamp(_slowMoPressedTimer, 0F, TimeToFullSlowMo);
        UpdateSlowMo();

        if (_renderer != null)
        {
            var scale = SlowMoTime/SlowMoMaxTime;
            transform.localScale = new Vector3(scale, 1F, 1F);

            var red = scale > 0.5F ? 1F - 2F*(scale - 0.5F) : 1.0F;
            var green = scale > 0.5F ? 1.0F : 2F*scale;
            _renderer.color = new Color(red, green, 0F);
        }
    }


    // ===========================================================================================
    private void UpdateSlowMo()
    {
        var percent = _slowMoPressedTimer/TimeToFullSlowMo;

        Time.timeScale = 1F - (1F - SlowMoBlocksEffect)*percent;

        for (int i = 0; i < _allAffectedAudioSources.Length; i++)
            _allAffectedAudioSources[i].pitch = 1F - (1F - SlowMoAudioEffect) * percent;

        if (BloomOnCamera != null)
        {
            BloomOnCamera.bloomThreshold
                = _defaultBloomThreshold + (_defaultBloomThreshold/BloomThresholdMultiplier - _defaultBloomThreshold)*percent;
        }
    }
}
