using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using HardShellStudios.InputManager;


public class SettingsMenu : FadeInOut2DMenu
{
    // ===========================================================================================
    public InputField NameInput;
    public Slider VolumeSlider;
    public Dropdown ResolutionDropdown;
    public Toggle FullscreenToggle;
    public Dropdown QualityDropdown;
    public Toggle EffectsToggle;
    public Toggle ShadowToggle;
    public Toggle GridToggle;
    public Dropdown HideCursorDropdown;

    private LevelSettings _levelSettings;
    private BestScore _bestScore;
    

    // ===========================================================================================
    public override void Show()
    {
        if (base.Show(LevelSettings.Instance.GlobalEffectsSwitch))
            LevelSettings.Instance.Pause();
    }

    public override void Hide()
    {
        base.Hide(LevelSettings.Instance.GlobalEffectsSwitch);
    }

    public override void OnHided()
    {
        base.OnHided();
        if (OpenedMenus.Count == 0)
            LevelSettings.Instance.Resume();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = Mathf.Clamp01(VolumeSlider.value);
    }

    public void ChangeName()
    {
        _levelSettings.PlayerName = NameInput.text;
    }

    public void ChangeResolution()
    {
        var text = ResolutionDropdown.options[ResolutionDropdown.value].text.Split('x');
        var width = int.Parse(text[0]);
        var height = int.Parse(text[1]);
        if (width != Screen.width || height != Screen.height)
            Screen.SetResolution(width, height, FullscreenToggle.isOn);
    }

    public void ChangeFullscreenMode()
    { 
        Screen.SetResolution(Screen.width, Screen.height, FullscreenToggle.isOn);
    }

    public void ChangeQuality()
    {
        QualitySettings.SetQualityLevel(QualityDropdown.value, true);
    }

    public void ChangeEffects()
    {
        _levelSettings.GlobalEffectsSwitch = EffectsToggle.isOn;
    }

    public void ChangeShadow()
    {
        _levelSettings.DrawShadow = ShadowToggle.isOn;
    }

    public void ChangeGrid()
    {
        _levelSettings.DrawGrid = GridToggle.isOn;
    }

    public void ChangeHideCursor()
    {
        switch (HideCursorDropdown.value)
        {
            case 0:
                _levelSettings.HideCursorInGame = false;
                break;
            case 1:
                _levelSettings.HideCursorInGame = true;
                _levelSettings.HideCursorDelay = 1F;
                break;
            case 2:
                _levelSettings.HideCursorInGame = true;
                _levelSettings.HideCursorDelay = 2F;
                break;
            case 3:
                _levelSettings.HideCursorInGame = true;
                _levelSettings.HideCursorDelay = 3F;
                break;
        }
    }

    public void Quit()
    {
        hardManager.singleton.SaveBindings();
        _levelSettings.Save();
        _bestScore.Add(new BestScoreRecord(_levelSettings.PlayerName, _levelSettings.Score));
        _bestScore.Save();
        Application.Quit();
    }

    protected override void Awake()
    {
        base.Awake();

        if (NameInput == null)
        {
            var inputs = GetComponentsInChildren<InputField>(true).Where(x => x.name.ToLower().Contains("nameinput")).ToList();
            if (inputs.Count != 1)
                Debug.LogError("SettingsMenu script: NameInput not found or found too much");
            else
                NameInput = inputs[0];
        }

        if (VolumeSlider == null)
        {
            var sliders = GetComponentsInChildren<Slider>(true).Where(x => x.name.ToLower().Contains("volumeslider")).ToList();
            if (sliders.Count != 1)
                Debug.LogError("SettingsMenu script: VolumeSlider not found or found too much");
            else
                VolumeSlider = sliders[0];
        }
    
        if (ResolutionDropdown == null)
        {
            var dropdowns = GetComponentsInChildren<Dropdown>(true).Where(x => x.name.ToLower().Contains("resolutiondropdown")).ToList();
            if (dropdowns.Count != 1)
                Debug.LogError("SettingsMenu script: ResolutionDropdown not found or found too much");
            else
                ResolutionDropdown = dropdowns[0];
        }

        if (FullscreenToggle == null)
        {
            var toggles = GetComponentsInChildren<Toggle>(true).Where(x => x.name.ToLower().Contains("fullscreentoggle")).ToList();
            if (toggles.Count != 1)
                Debug.LogError("SettingsMenu script: FullscreenToggle not found or found too much");
            else
                FullscreenToggle = toggles[0];
        }

        if (QualityDropdown == null)
        {
            var dropdowns = GetComponentsInChildren<Dropdown>(true).Where(x => x.name.ToLower().Contains("qualitydropdown")).ToList();
            if (dropdowns.Count != 1)
                Debug.LogError("SettingsMenu script: QualityDropdown not found or found too much");
            else
                QualityDropdown = dropdowns[0];
        }

        if (EffectsToggle == null)
        {
            var toggles = GetComponentsInChildren<Toggle>(true).Where(x => x.name.ToLower().Contains("effectstoggle")).ToList();
            if (toggles.Count != 1)
                Debug.LogError("SettingsMenu script: EffectsToggle not found or found too much");
            else
                EffectsToggle = toggles[0];
        }

        if (ShadowToggle == null)
        {
            var toggles = GetComponentsInChildren<Toggle>(true).Where(x => x.name.ToLower().Contains("shadowtoggle")).ToList();
            if (toggles.Count != 1)
                Debug.LogError("SettingsMenu script: ShadowToggle not found or found too much");
            else
                ShadowToggle = toggles[0];
        }

        if (GridToggle == null)
        {
            var toggles = GetComponentsInChildren<Toggle>(true).Where(x => x.name.ToLower().Contains("gridtoggle")).ToList();
            if (toggles.Count != 1)
                Debug.LogError("SettingsMenu script: GridToggle not found or found too much");
            else
                GridToggle = toggles[0];
        }

        if (HideCursorDropdown == null)
        {
            var dropdowns = GetComponentsInChildren<Dropdown>(true).Where(x => x.name.ToLower().Contains("hidecursordropdown")).ToList();
            if (dropdowns.Count != 1)
                Debug.LogError("SettingsMenu script: HideCursorDropdown not found or found too much");
            else
                HideCursorDropdown = dropdowns[0];
        }
    }

    protected void OnEnable()
    {
        _levelSettings = LevelSettings.Instance;
        _bestScore = BestScore.Instance;

        VolumeSlider.value = AudioListener.volume;

        NameInput.text = _levelSettings.PlayerName;

        ResolutionDropdown.options.Clear();
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            var text = string.Format("{0}x{1}", Screen.resolutions[i].width, Screen.resolutions[i].height);
            ResolutionDropdown.options.Add(new Dropdown.OptionData(text));
            if (Screen.width == Screen.resolutions[i].width && Screen.height == Screen.resolutions[i].height)
                ResolutionDropdown.value = i;
        }

        FullscreenToggle.isOn = Screen.fullScreen;

        QualityDropdown.options.Clear();
        for (int i = 0; i < QualitySettings.names.Length; i++)
            QualityDropdown.options.Add(new Dropdown.OptionData(QualitySettings.names[i]));
        QualityDropdown.value = QualitySettings.GetQualityLevel();

        EffectsToggle.isOn = _levelSettings.GlobalEffectsSwitch;
        ShadowToggle.isOn = _levelSettings.DrawShadow;
        GridToggle.isOn = _levelSettings.DrawGrid;

        if (!_levelSettings.HideCursorInGame)
            HideCursorDropdown.value = 0;
        else
        {
            if (_levelSettings.HideCursorDelay > 0F && _levelSettings.HideCursorDelay <= 1F)
                HideCursorDropdown.value = 1;

            if (_levelSettings.HideCursorDelay > 1F && _levelSettings.HideCursorDelay <= 2F)
                HideCursorDropdown.value = 2;

            if (_levelSettings.HideCursorDelay > 2F)
                HideCursorDropdown.value = 3;
        }
    }
}

