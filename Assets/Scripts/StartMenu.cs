using UnityEngine;


public class StartMenu : FadeInOut2DMenu
{
    // ========================================================================================
    public override void Show()
    {
        base.Show(LevelSettings.Instance.GlobalEffectsSwitch);

        var anim = GetComponentInChildren<ParticleSystem>();
        if (anim != null && !LevelSettings.Instance.GlobalEffectsSwitch)
            anim.Stop();

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

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

    protected void Start()
    {
        Show();
    }
    
    protected void Update()
    {
        if (Input.anyKey)
            Hide();
    }
}
