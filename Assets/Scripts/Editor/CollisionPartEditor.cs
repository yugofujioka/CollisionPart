using UnityEngine;
using UnityEditor;


/// <summary>
/// CollisionPart拡張
/// </summary>
[CustomEditor(typeof(CollisionPart)), CanEditMultipleObjects]
public sealed class CollisionPartEditor : Editor {
    /// <summary>
    /// Inspector表示
    /// </summary>
    public override void OnInspectorGUI() {
        SerializedProperty dataArrayProp = this.serializedObject.FindProperty("collisionDatas");

        GUILayout.Space(3f);

        int pointCount = dataArrayProp.arraySize;
        if (pointCount > 0) {
            for (int pointIndex = 0; pointIndex < pointCount; ++pointIndex) {
                SerializedProperty dataProp = dataArrayProp.GetArrayElementAtIndex(pointIndex);
                EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("range"), new GUIContent("Range", "半径"));
                EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("offset"), new GUIContent("Offset", "オフセット"));

                GUILayout.BeginHorizontal();
                // Collision挿入
                if (GUILayout.Button("+", EditorStyles.miniButtonLeft)) {
                    ++pointCount;
                    dataArrayProp.InsertArrayElementAtIndex(pointIndex);
                }
                // Collision削除
                if (pointCount <= 1)
                    GUI.color = Color.gray;
                if (GUILayout.Button("-", EditorStyles.miniButtonRight) && pointCount > 1) {
                    dataArrayProp.DeleteArrayElementAtIndex(pointIndex);
                    --pointCount;
                    --pointIndex;
                }
                GUILayout.EndHorizontal();
            }
        } else {
            // Collision追加
            if (GUILayout.Button("+", EditorStyles.miniButtonLeft)) {
                ++pointCount;
                dataArrayProp.InsertArrayElementAtIndex(0);
            }
        }
        this.serializedObject.ApplyModifiedProperties();
    }
    
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    private static void DrawWireGizmo(Transform transform, GizmoType gizmoType) {
        CollisionPart t = transform.GetComponent<CollisionPart>();
        if (t == null)
            return;

        SerializedObject so = new SerializedObject(t);
        SerializedProperty dataArrayProp = so.FindProperty("collisionDatas");

        for (int i = 0; i < dataArrayProp.arraySize; ++i) {
            Gizmos.color = new Color(0f, 1f, 0f, 1f);

            SerializedProperty dataProp = dataArrayProp.GetArrayElementAtIndex(i);
            Vector3 offset = dataProp.FindPropertyRelative("offset").vector3Value;
            float range = dataProp.FindPropertyRelative("range").floatValue;

            Quaternion rotation = t.transform.rotation;
            Vector3 position = t.transform.position + rotation * offset;
            Gizmos.matrix = Matrix4x4.TRS(position, rotation, new Vector3(1f, 1f, 0.01f));
            Gizmos.DrawWireSphere(Vector3.zero, range);
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;
        }
    }
}
