using UnityEditor;

namespace BlueNoah.PhysicsEngine.Editor
{
    [CustomEditor(typeof(FPTransform))]
    public sealed class FixedPointTransformEditor : UnityEditor.Editor
    {
        private SerializedFixedPointVector3 position;
        private SerializedFixedPointVector3 euler;
        private SerializedFixedPointVector3 scale;
        private SerializedProperty parent;

        private  void OnEnable()
        {
            parent = serializedObject.FindProperty("parent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(parent);
                position = new SerializedFixedPointVector3("Position",serializedObject.FindProperty("_position"));
                euler = new SerializedFixedPointVector3("Euler",serializedObject.FindProperty("_euler"));
                scale = new SerializedFixedPointVector3("Scale",serializedObject.FindProperty("_scale"));
                position.PropertyField();
                euler.PropertyField();
                scale.PropertyField();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}