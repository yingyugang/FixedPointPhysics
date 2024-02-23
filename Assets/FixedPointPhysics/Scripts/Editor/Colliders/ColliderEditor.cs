using UnityEditor;
using UnityEngine;

namespace BlueNoah.PhysicsEngine.Editor
{
    /// <summary>
    /// Serves as a base class for creating custom editor scripts for collider components within a custom physics engine. 
    /// It provides common functionality and a framework for extending the Unity Editor's GUI to support specialized collider properties.
    /// </summary>
    public abstract class ColliderEditor : UnityEditor.Editor
    {
        // SerializedProperty fields are used to interact with script properties in the Unity Editor.
        protected SerializedProperty isTrigger;
        protected SerializedProperty layer;
        protected SerializedFixedPoint64 rebound;
        
        protected SerializedProperty gizmosColor;
        protected SerializedProperty drawAABB;
        protected SerializedProperty drawDebugInfo;

        private SerializedFixedPointVector3 center;

        /// <summary>
        /// Called when the script instance is loaded. Initializes SerializedProperty objects for further use in the custom editor.
        /// </summary>
        protected virtual void OnEnable()
        {
            center = new SerializedFixedPointVector3("Center",serializedObject.FindProperty("center"));
            isTrigger = serializedObject.FindProperty("isTrigger");
            layer = serializedObject.FindProperty("layer");
            rebound = new SerializedFixedPoint64("Rebound",serializedObject.FindProperty("rebound"));
            gizmosColor = serializedObject.FindProperty("gizmosColor");
            drawAABB = serializedObject.FindProperty("drawAABB");
            drawDebugInfo = serializedObject.FindProperty("drawDebugInfo");
        }

        /// <summary>
        /// Overrides the Inspector GUI to provide a custom UI for editing collider properties. Ensures changes are serialized properly.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update(); // Prepare the object for inspector GUI updates.
            EditorGUILayout.PropertyField(isTrigger);
            EditorGUILayout.PropertyField(layer);
            rebound.PropertyField();
            center.PropertyField();
            OnColliderInspectorGUI(); // Abstract method for additional collider-specific GUI elements.
            EditorGUILayout.PropertyField(gizmosColor);
            EditorGUILayout.PropertyField(drawAABB);
            EditorGUILayout.PropertyField(drawDebugInfo);
            // End the code block and update the label if a change occurred
            if (EditorGUI.EndChangeCheck())
            {
                OnColliderChanged();
                center.Apply();
                layer.intValue = Mathf.Clamp(layer.intValue, 0, 31);
                rebound.floatValue = Mathf.Max(0, rebound.floatValue);
                rebound.Apply();
            }
            serializedObject.ApplyModifiedProperties(); // Apply modifications after inspector GUI updates.
        }

        /// <summary>
        /// An abstract method to be implemented by subclasses to perform additional change when EditorGUI changed.
        /// </summary>
        protected virtual void OnColliderChanged(){}
        
        /// <summary>
        /// An abstract method to be implemented by subclasses to add custom UI elements specific to the collider type to the inspector.
        /// </summary>
        protected virtual void OnColliderInspectorGUI(){}
    }
}
