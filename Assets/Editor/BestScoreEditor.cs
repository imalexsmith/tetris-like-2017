using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof (BestScore))]
public class BestScoreEditor : Editor
{
    // ========================================================================================
    private BestScore _target;
    private string _newName = BestScoreRecord.NoNameCap;
    private string _newScore = "0";


    // ========================================================================================
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);
        GUI.enabled = true;
        GUILayout.Space(8F);

        if (_target != null)
        {
            for (var i = 0; i < _target.Records.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label((i+1).ToString(), GUILayout.Width(32));
                GUI.enabled = false;
                GUILayout.TextField(_target.Records[i].PlayerName);
                GUILayout.TextField(_target.Records[i].Score.ToString());
                GUI.enabled = true;
                if (GUILayout.Button("-", GUILayout.Width(22)))
                    _target.RemoveAt(i);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(8F);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Add ", GUILayout.Width(32));
            _newName = GUILayout.TextField(_newName);
            _newScore = GUILayout.TextField(_newScore);
            _newScore = Regex.Replace(_newScore, "[^0-9]", "");
            if (GUILayout.Button("+", GUILayout.Width(22)))
            {
                int newScoreInt;
                int.TryParse(_newScore, out newScoreInt);
                _target.Add(new BestScoreRecord(_newName, newScoreInt));
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8F);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save")) _target.Save();
            if (GUILayout.Button("Load")) _target.Load();
            if (GUILayout.Button("Clear"))
            {
                if (EditorUtility.DisplayDialog("Warning", "Do you really want to clear best score table?", "Yes", "No"))
                    _target.Clear();
            }
            GUILayout.EndHorizontal();
        }
    }

    protected void OnEnable()
    {
        _target = (BestScore)serializedObject.targetObject;
    }
}
