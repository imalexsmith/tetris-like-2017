using UnityEngine;
using UnityEngine.UI;


namespace HardShellStudios.InputManager
{
    ///<summary>
    ///Custom editor inspector script that actions on the InputController prefab.
    ///</summary>
    [AddComponentMenu("Hard Shell Studios/Input Manager/Rebind Button")]
    [RequireComponent(typeof(Button))]
    public class hardInputUI : MonoBehaviour
    {
        public Text displayText;
        public string keyName;
        public bool useSecondary;
        [HideInInspector]
        public bool beingBound = false;
        public int buttonAction = 0;

        void Awake()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() => remapKey());
        }

        public void remapKey()
        {
            if (buttonAction == 0)
            {
                beingBound = true;
                hardInput.HardStartRebind(keyName, useSecondary, gameObject.GetComponent<hardInputUI>());
            }
            else if (buttonAction == 1)
            {
                hardInput.ResetBinding(keyName);
            }
            else if (buttonAction == 2)
            {
                hardInput.ResetAllBindings();
            }
        }

        void OnGUI()
        {
            if (displayText != null && buttonAction == 0)
                displayText.text = beingBound ? "???" : hardInput.GetKeyName(keyName, useSecondary);
        }
    }
}