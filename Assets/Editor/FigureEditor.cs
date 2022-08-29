using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/// <summary>
/// Extend standart editor with additional functionality. "Random type" button will allow change figure type at runtime. 
/// "Center to parent origin" button will allow move visual center of figure to parent transform point (0,0,0).
/// </summary>
[CustomEditor(typeof (Figure), true)]
[CanEditMultipleObjects]
public class FigureEditor : Editor
{
    // ========================================================================================
    private List<Figure> _targets;


    // ========================================================================================
    public override void OnInspectorGUI()
    {
        if (_targets.Count > 0)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Random type", GUILayout.Height(24)))
                foreach (var t in _targets)
                    t.CreateRandom();

            if (GUILayout.Button("Center to parent origin", GUILayout.Height(24)))
                foreach (var t in _targets)
                    t.MoveCenterToParentOrigin();

            GUILayout.EndHorizontal();
        }

        DrawDefaultInspector();
    }

    protected void OnEnable()
    {
        _targets = serializedObject.targetObjects.Select(x => x as Figure).ToList();
    }
}
