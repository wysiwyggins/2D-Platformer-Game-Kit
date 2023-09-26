using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draws the default inspector

        PlayerController controller = (PlayerController)target;

        if (GUILayout.Button("Save as Default"))
        {
            controller.SaveDefaultValues();
        }

        if (GUILayout.Button("Load Defaults"))
        {
            controller.LoadDefaultValues();
        }
    }
}
