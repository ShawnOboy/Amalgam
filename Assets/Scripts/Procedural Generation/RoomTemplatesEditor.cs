using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomTemplates))]
public class RoomTemplatesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RoomTemplates roomTemplates = (RoomTemplates)target;

        if (GUILayout.Button("Spawn Rooms")) {
            roomTemplates.SpawnRooms();
        }
    }
}
