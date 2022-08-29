using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif


[ExecuteInEditMode]
public abstract class UniqueComponentAtScene<T> : MonoBehaviour where T : UniqueComponentAtScene<T>
{
    // ========================================================================================
    /// <summary>
    /// Links to all unique components at scene.
    /// </summary>
    private static readonly List<T> _instances = new List<T>();

    /// <summary>
    /// Try get unique component of type T, which is inherited from UniqueComponentAtScene.
    /// </summary>
    public static T Instance {
        get {
            //var result = _instances.FindAll(x => x.GetType() == typeof(T));
            //var count = result.Count;
            //if (count == 1)
            //    return result[0];

            var result = new List<T>();
            for (var i = 0; i < _instances.Count; i++)
            {
                if (_instances[i].GetType() == typeof(T))
                    result.Add(_instances[i]);
            }
            if (result.Count == 1)
                return result[0];

#if UNITY_EDITOR
            Debug.LogError(result.Count == 0 ? string.Format("Error: unique component {0} not found", typeof (T).Name) : string.Format("Error: unique component {0} found more than once", typeof (T).Name));
#endif
            return null;
        }
    }


    // ========================================================================================
    /// <summary>
    /// <para> Checks uniqueness of the component for the application. It is very slow operation!</para>
    /// <para> Return true - if the component is found only once at scene, otherwise false.</para>
    /// </summary>
    internal static bool CheckUniqueness(Behaviour component)
    {
        //        var t = component.GetType();
        //        var result = Resources.FindObjectsOfTypeAll(t);
        //#if UNITY_EDITOR
        //        result = result.Where(x => !EditorUtility.IsPersistent(x)).ToArray();
        //#endif
        //return result.Length == 1;

        return component.FindObjectsOfTypeAtSceneAll(component.GetType()).Length == 1;
    }


    // ========================================================================================
    [SerializeField, HideInInspector]
    private bool _isFirstInstance;


    // ========================================================================================
    protected virtual void Awake()
    {
        // If scene has any Behaviour of current type - destroy current component, else add it to instances collection

        // First, check ref collection
        if (_instances.All(x => x.GetType() != GetType()))
        {
            // Second, check scene
            if (CheckUniqueness(this))
                _isFirstInstance = true;

            if (_isFirstInstance)
            {
                _instances.Add((T)this);
#if UNITY_EDITOR
                Debug.LogFormat(string.Format("<color=green>Singleton component <b>{0}</b> successfully added to the scene</color>", GetType().Name));
#endif
                return;
            }
        }

        if (!_isFirstInstance)
            Invoke("Delete", 0);

#if UNITY_EDITOR
        Debug.LogErrorFormat(string.Format("<color=red>Error: singleton component <b>{0}</b> already added at scene</color>", GetType().Name));
        Selection.activeGameObject = _instances.First(x => x.GetType() == GetType() && x._isFirstInstance).gameObject;
#endif
    }

    protected virtual void OnDestroy()
    {
        // After this we can add new unique component to scene again
        var t = (T)this;
        if (_instances.Contains(t))
            _instances.Remove(t);
    }


    // ========================================================================================
    private void Delete()
    {
        DestroyImmediate(this);
    }
}

#if UNITY_EDITOR
internal sealed class UniqueComponentAtSceneReloader : ScriptableObject
{
    [DidReloadScripts, InitializeOnLoadMethod]
    private static void ForceReload()
    {
        var allTypes = typeof(UniqueComponentAtScene<>).Assembly.GetTypes();
        var derivedTypes = new List<Type>();
        GetAllDerivedTypesRecursively(allTypes, typeof(UniqueComponentAtScene<>), ref derivedTypes);

        foreach (var type in derivedTypes)
        {
            var components = Resources.FindObjectsOfTypeAll(type)
                .Where(x => !EditorUtility.IsPersistent(x))
                .Cast<MonoBehaviour>()
                .ToArray();

            foreach (var c in components)
                c.Invoke("Awake", 0);
        }
    }

    private static void GetAllDerivedTypesRecursively(Type[] types, Type type1, ref List<Type> results)
    {
        if (type1.IsGenericType)
        {
            GetDerivedFromGeneric(types, type1, ref results);
        }
        else
        {
            GetDerivedFromNonGeneric(types, type1, ref results);
        }
    }

    private static void GetDerivedFromGeneric(Type[] types, Type type, ref List<Type> results)
    {
        var derivedTypes = types.Where(t => t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == type).ToList();
        results.AddRange(derivedTypes);
        foreach (Type derivedType in derivedTypes)
        {
            GetAllDerivedTypesRecursively(types, derivedType, ref results);
        }
    }

    private static void GetDerivedFromNonGeneric(Type[] types, Type type, ref List<Type> results)
    {
        var derivedTypes = types.Where(t => t != type && type.IsAssignableFrom(t)).ToList();

        results.AddRange(derivedTypes);
        foreach (Type derivedType in derivedTypes)
        {
            GetAllDerivedTypesRecursively(types, derivedType, ref results);
        }
    }
}
#endif