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
        SerializedProperty arrayProp = this.serializedObject.FindProperty("collisionDatas");

        GUILayout.Space(3f);

        int ptCount = arrayProp.arraySize;
        if (ptCount > 0) {
            SerializedProperty dataProp = null;
            SerializedProperty prop = null;
            for (int ptIndex = 0; ptIndex < ptCount; ++ptIndex) {
                dataProp = arrayProp.GetArrayElementAtIndex(ptIndex);

                prop = dataProp.FindPropertyRelative("range");
                EditorGUILayout.PropertyField(prop, new GUIContent("Range"));
                prop = dataProp.FindPropertyRelative("offset");
                Vector2 offset = prop.vector3Value;
                prop.vector3Value = EditorGUILayout.Vector2Field(new GUIContent("Offset"), offset);

                GUILayout.BeginHorizontal();
                // Collision挿入
                if (GUILayout.Button("+", EditorStyles.miniButtonLeft)) {
                    ++ptCount;
                    arrayProp.InsertArrayElementAtIndex(ptIndex);
                }
                // コリジョン削除
                if (ptCount <= 1)
                    GUI.color = Color.gray;
                if (GUILayout.Button("-", EditorStyles.miniButtonRight)
                    && ptCount > 1)
                {
                    arrayProp.DeleteArrayElementAtIndex(ptIndex);
                    --ptCount;
                    --ptIndex;
                }
                GUILayout.EndHorizontal();
            }
        } else {
            // コリジョン追加
            if (GUILayout.Button("+", EditorStyles.miniButtonLeft)) {
                ++ptCount;
                arrayProp.InsertArrayElementAtIndex(0);
            }
        }

        this.serializedObject.ApplyModifiedProperties();
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    private static void DrawWireGizmo(Transform trans, GizmoType gizmoType) {
        CollisionPart t = trans.GetComponent<CollisionPart>();
        if (t == null)
            return;

        SerializedObject so = new SerializedObject(t);
        SerializedProperty arrayProp = so.FindProperty("collisionDatas");

        SerializedProperty dataProp = null;
        SerializedProperty prop = null;
        for (int i = 0; i < arrayProp.arraySize; ++i) {
            Gizmos.color = new Color(0f, 1f, 0f, 1f);

            dataProp = arrayProp.GetArrayElementAtIndex(i);
            prop = dataProp.FindPropertyRelative("offset");
            Vector3 offset = prop.vector3Value;
            prop = dataProp.FindPropertyRelative("range");
            float range = prop.floatValue;
            
            Quaternion rotation = trans.rotation;
            Vector3 position = trans.position + rotation * offset;
            Vector3 scale = new Vector3(1f, 1f, 0.01f);
            Gizmos.matrix = Matrix4x4.TRS(position, rotation, scale);
            Gizmos.DrawWireSphere(Vector3.zero, range);
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;
        }
    }
}