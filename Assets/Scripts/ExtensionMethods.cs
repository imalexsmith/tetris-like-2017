using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public static class ObjectExtensionMethods
{
    public static UnityEngine.Object[] FindObjectsOfTypeAtSceneAll(this UnityEngine.Object target, Type type)
    {
        var result = Resources.FindObjectsOfTypeAll(type);
#if UNITY_EDITOR
        result = result.Where(x => !EditorUtility.IsPersistent(x)).ToArray();
#endif
        return result;
    }

    public static UnityEngine.Object[] FindObjectsOfTypeAtSceneAll<T>(this UnityEngine.Object target)
    {
        return FindObjectsOfTypeAtSceneAll(target, typeof (T));
    }
}
