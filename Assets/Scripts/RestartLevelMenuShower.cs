using System.Linq;
using UnityEngine;


public class RestartLevelMenuShower : MenuShowerBase<RestartLevelMenu>
{
    // ===========================================================================================
    protected void Update()
    {
        if (Application.isPlaying && Menu != null)
        {
            if (hardInput.GetKeyDown("Restart"))
            {
                if (!Menu.gameObject.activeSelf)
                    ShowMenu();
                else
                    HideMenu();
            }

            if (hardInput.GetKeyDown("Cancel"))
            {
                if (MenuBase.GetOpenedMenus.Count > 0 && MenuBase.GetOpenedMenus.Last() == Menu)
                    HideMenu(); 
            }
        }
    }
}
