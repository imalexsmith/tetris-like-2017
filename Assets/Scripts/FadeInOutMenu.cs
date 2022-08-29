using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Canvas))]
public class FadeInOut2DMenu : MenuBase
{
    public enum AnimationDirection
    {
        Stopped = 0,
        FadeIn = 1,
        FadeOut = -1
    }

    // ========================================================================================
    public bool NeedFadeIn = true;
    public bool NeedFadeOut = false;
    public AnimationCurve FadeInCurve = AnimationCurve.Linear(0F, 0F, 0.4F, 1F);
    public AnimationCurve FadeOutCurve;
    public AnimationDirection CurrentDirection { get; private set; }

    private float _currentFadeInTime;
    private float _currentFadeOutTime;
    private CanvasRenderer[] _renderers;
    private Coroutine _showRoutine;
    private Coroutine _hideRoutine;


    // ========================================================================================
    public override void Show()
    {
        Show(NeedFadeIn);
    }

    public override void Hide()
    {
        Hide(NeedFadeOut);
    }

    protected override bool Show(bool animate)
    {
        if (CurrentDirection == AnimationDirection.FadeIn || LastFrameInputAccepted == Time.frameCount)
            return false;

        LastFrameInputAccepted = Time.frameCount;
        gameObject.SetActive(true);
        OpenedMenus.Add(this);

        StopAllCoroutines();

        if (animate && NeedFadeIn && FadeInCurve.keys.Length > 1)
        {
            _currentFadeInTime = 0F;
            _currentFadeOutTime = 0F;
            CurrentDirection = AnimationDirection.FadeIn;
            _showRoutine = StartCoroutine(Showing());
        }
        else OnShowed();

        return true;
    }

    protected override bool Hide(bool animate)
    {
        if (CurrentDirection == AnimationDirection.FadeOut || LastFrameInputAccepted == Time.frameCount)
            return false;

        LastFrameInputAccepted = Time.frameCount;
        StopAllCoroutines();

        if (animate && NeedFadeOut && FadeOutCurve.keys.Length > 1)
        {
            _currentFadeInTime = 0F;
            _currentFadeOutTime = 0F;
            CurrentDirection = AnimationDirection.FadeOut;
            _hideRoutine = StartCoroutine(Hiding());
        }
        else OnHided();

        return true;
    }

    public override void OnShowed()
    {
        CurrentDirection = AnimationDirection.Stopped;
    }

    public override void OnHided()
    {
        CurrentDirection = AnimationDirection.Stopped;
        gameObject.SetActive(false);
        OpenedMenus.Remove(this);
    }

    protected virtual void Awake()
    {
        _renderers = GetComponentsInChildren<CanvasRenderer>(true);
    }

    private IEnumerator Showing()
    {
        while (true)
        {
            if (_currentFadeInTime < FadeInCurve.keys[FadeInCurve.length - 1].time)
            {
                for (int i = 0; i < _renderers.Length; i++)
                    _renderers[i].SetAlpha(FadeInCurve.Evaluate(_currentFadeInTime));
            }
            else
            {
                for (int i = 0; i < _renderers.Length; i++)
                    _renderers[i].SetAlpha(FadeInCurve.keys[FadeInCurve.length - 1].value);
                StopCoroutine(_showRoutine);
                OnShowed();
                yield break;
            }

            _currentFadeInTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator Hiding()
    {
        while (true)
        {
            if (_currentFadeOutTime < FadeOutCurve.keys[FadeOutCurve.length - 1].time)
            {
                for (int i = 0; i < _renderers.Length; i++)
                    _renderers[i].SetAlpha(FadeOutCurve.Evaluate(_currentFadeOutTime));
            }
            else
            {
                for (int i = 0; i < _renderers.Length; i++)
                    _renderers[i].SetAlpha(FadeOutCurve.keys[FadeOutCurve.length - 1].value);
                StopCoroutine(_hideRoutine);
                OnHided();
                yield break;
            }

            _currentFadeOutTime += Time.deltaTime;
            yield return null;
        }
    }
}
