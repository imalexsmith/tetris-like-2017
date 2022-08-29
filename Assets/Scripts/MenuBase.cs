using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


[DisallowMultipleComponent]
public abstract class MenuBase : MonoBehaviour
{
    // ========================================================================================
    protected static List<MenuBase> OpenedMenus = new List<MenuBase>();
    public static ReadOnlyCollection<MenuBase> GetOpenedMenus {
        get { return OpenedMenus.AsReadOnly(); }
    }

    protected static int LastFrameInputAccepted;


    // ========================================================================================
    public static void HideAll()
    {
        for (int i = 0; i < OpenedMenus.Count; i++)
            OpenedMenus[i].Hide();
    }


    // ========================================================================================
    public bool IsOpened {
        get { return OpenedMenus.Contains(this); }
    }


    // ========================================================================================
    public abstract void Show();
    public abstract void Hide();
    protected abstract bool Show(bool animate);
    protected abstract bool Hide(bool animate);
    public abstract void OnShowed();
    public abstract void OnHided();
}
