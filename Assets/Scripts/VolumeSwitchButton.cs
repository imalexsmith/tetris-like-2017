using UnityEngine;


public class VolumeSwitchButton : ColoredSwitchButtonBase
{
    // ===========================================================================================
    private float _defaultVolume = 1F;


    // ===========================================================================================
    public override void Switch()
    {
        AudioListener.volume = AudioListener.volume != 0F 
            ? 0F 
            : _defaultVolume;
    }

    protected override void Update()
    {
        if (AudioListener.volume > 0F)
            _defaultVolume = AudioListener.volume;

        SwitchedOn = AudioListener.volume != 0F;

        base.Update();
    }
}
