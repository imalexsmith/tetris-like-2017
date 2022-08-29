using System.Linq;


public abstract class MenuShowerBase<T> : UniqueComponentAtObject where T : MenuBase
{
    // ========================================================================================
    public T Menu;


    // ========================================================================================
    public virtual void ShowMenu()
    {
        if (Menu != null)
            Menu.Show();
    }

    public virtual void HideMenu()
    {
        if (Menu != null)
            Menu.Hide();
    }

    protected override void Awake()
    {
        base.Awake();

        if (Menu == null)
        {
            var menus = this.FindObjectsOfTypeAtSceneAll<T>().Cast<T>().ToArray();
            if (menus.Length == 1)
                Menu = menus[0];
        }
    }
}
