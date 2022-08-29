using UnityEngine;


[ExecuteInEditMode]
public abstract class UniqueComponentAtObject : MonoBehaviour
{
    // ========================================================================================
    /// <summary>
    /// <para> Checks uniqueness of the component for its gameObject.</para>
    /// <para> Return true - if the component is found only once at gameObject and children, otherwise false.</para>
    /// </summary>
    internal static bool CheckUniqueness(Behaviour component, bool includeChild = true)
    {
        var t = component.GetType();
        var go = component.gameObject;

        return includeChild ? go.GetComponentsInChildren(t, true).Length == 1 : go.GetComponents(t).Length == 1;
    }


    // ========================================================================================
    [SerializeField, HideInInspector]
    private bool _isFirstInstance;


    // ========================================================================================
    protected virtual void Awake()
    {
        if (CheckUniqueness(this))
        {
            _isFirstInstance = true;
#if UNITY_EDITOR
            Debug.LogFormat(string.Format("<color=green>Unique component <b>{0}</b> successfully added to the object <b>{1}</b></color>", GetType().Name, gameObject.name));
#endif
            return;
        }

        if (_isFirstInstance)
            return;

        Invoke("Delete", 0);
#if UNITY_EDITOR
        Debug.LogErrorFormat(string.Format("<color=red>Error: unique component <b>{1}</b> already at object <b>{0}</b></color>", GetType().Name, gameObject.name));
#endif
    }


    // ========================================================================================
    private void Delete()
    {
        DestroyImmediate(this);
    }
}
