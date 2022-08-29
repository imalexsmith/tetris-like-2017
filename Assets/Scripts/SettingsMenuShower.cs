using System.Linq;
using UnityEngine;


public class SettingsMenuShower : MenuShowerBase<SettingsMenu>
{
    // ===========================================================================================
    protected void Update()
    {
        if (Application.isPlaying && Menu != null)
        {
            if (hardInput.GetKeyDown("Cancel"))
            {
                if (MenuBase.GetOpenedMenus.Count == 0)
                    ShowMenu();

                if (MenuBase.GetOpenedMenus.Count > 0 && MenuBase.GetOpenedMenus.Last() == Menu)
                    HideMenu();
            }
        }
    }
}
