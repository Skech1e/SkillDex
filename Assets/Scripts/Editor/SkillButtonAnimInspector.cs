using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillButtonAnim))]
public class SkillButtonAnimInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SkillButtonAnim skillButtonAnim = (SkillButtonAnim)target;
        if(GUILayout.Button("Record Button Position", GUILayout.Height(30)))
            skillButtonAnim.targetPosition = skillButtonAnim.transform.localPosition;
    }
}
