public class EffectsSwitchButton : ColoredSwitchButtonBase
{
    // ===========================================================================================
    private LevelSettings _levelSettings;


    // ===========================================================================================
    public void Start()
    {
        _levelSettings = LevelSettings.Instance;
    }

    public override void Switch()
    {
        _levelSettings.GlobalEffectsSwitch = !_levelSettings.GlobalEffectsSwitch;
    }

    protected override void Update()
    {
        SwitchedOn = _levelSettings.GlobalEffectsSwitch;

        base.Update();
    }
}
