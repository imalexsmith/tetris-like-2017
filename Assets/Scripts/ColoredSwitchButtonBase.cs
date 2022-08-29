using UnityEngine;
using UnityEngine.UI;


public interface ISwitchButton
{
    void Switch();
}

[RequireComponent(typeof(Selectable)), DisallowMultipleComponent]
public abstract class ColoredSwitchButtonBase : MonoBehaviour, ISwitchButton
{
    // ===========================================================================================
    public Color SwitchedOnColor = Color.green;
    public Color SwitchedOffColor = Color.red;
    public bool SwitchedOn = true;
    private bool _lastSwitchedOn = true;

    private Selectable _selectable;


    // ===========================================================================================
    public virtual void Switch()
    {
        SwitchedOn = !SwitchedOn;
    }

    protected void Awake()
    {
        _selectable = GetComponent<Selectable>();
    }

    protected virtual void Update()
    {
        if (SwitchedOn != _lastSwitchedOn)
        {
            if (_selectable != null && _selectable.targetGraphic != null)
                _selectable.targetGraphic.color = SwitchedOn ? SwitchedOnColor : SwitchedOffColor;
        }
        _lastSwitchedOn = SwitchedOn;
    }
}
