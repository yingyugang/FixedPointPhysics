using UnityEditor;
using UnityEngine;

namespace BlueNoah.PhysicsEngine.Editor
{
    [CustomEditor(typeof(FPCharacterController))]
    [CanEditMultipleObjects]
    internal sealed class CharacterControllerEditor : AACapsuleColliderEditor
    {
        private SerializedProperty characterColliderType;
        
        /// <summary>
        /// Called when the script instance is loaded. Initializes SerializedProperty objects for further use in the custom editor.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            characterColliderType = serializedObject.FindProperty("characterColliderType");
        }
        
        protected override void OnColliderChanged()
        {
            if (characterColliderType.enumValueIndex == 1)
            {
                height.floatValue = Mathf.Max(0, height.floatValue);
                height.Apply();
            }
            radius.floatValue = Mathf.Max(0, radius.floatValue);
            radius.Apply();
        }
        
        /// <summary>
        /// Renders the custom inspector GUI elements for the capsule collider.
        /// This method is called to draw the inspector fields for editing the height and radius properties of the collider.
        /// </summary>
        protected override void OnColliderInspectorGUI()
        {
            EditorGUILayout.PropertyField(characterColliderType);
            if (characterColliderType.enumValueIndex == 1)
            {
                // Creates an inspector GUI field for editing the 'height' property.
                height.PropertyField();
            }
            // Creates an inspector GUI field for editing the 'radius' property.
            radius.PropertyField();
        }
    }
}