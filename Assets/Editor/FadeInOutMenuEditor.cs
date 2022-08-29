using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(FadeInOut2DMenu), true)]
public class FadeInOutMenuEditor : Editor
{
    // ========================================================================================
    private List<FadeInOut2DMenu> _targets;


    // ========================================================================================
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);
        GUI.enabled = true;

        var menusCount = MenuBase.GetOpenedMenus.Count;
        var menus = "Opened menus: " + menusCount;

        if (menusCount > 0)
        {
            for (int i = 0; i < menusCount; i++)
                menus += string.Format("\n   - {0};", MenuBase.GetOpenedMenus[i].GetType());
        }

        GUILayout.Space(8);
        EditorGUILayout.LabelField(menus, EditorStyles.helpBox);
        GUILayout.Space(8);

        if (_targets.Count == 1)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("NeedFadeIn"));
            if (_targets[0].NeedFadeIn)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FadeInCurve"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("NeedFadeOut"));
            if (_targets[0].NeedFadeOut)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FadeOutCurve"));

            var sp = serializedObject.GetIterator();
            sp.Next(true);
            while (true)
            {
                var spn = sp.name;
                if (!spn.StartsWith("m_") && spn != "NeedFadeIn" && spn != "FadeInCurve" && spn != "NeedFadeOut" && spn != "FadeOutCurve")
                    EditorGUILayout.PropertyField(sp);

                if (!sp.Next(false))
                    break;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    protected void OnEnable()
    {
        _targets = serializedObject.targetObjects.Select(x => x as FadeInOut2DMenu).ToList();
    }
}
