using UnityEditor;
using UnityEngine;

namespace BlueNoah.PhysicsEngine.Editor
{
    /// <summary>
    /// Provides a custom editor interface in Unity for FixedPointAACapsuleColliderPresenter.
    /// This class allows for the editing of FixedPoint axis-aligned capsule colliders in the Unity Editor.
    /// </summary>
    [CustomEditor(typeof(FPAACapsuleCollider))]
    [CanEditMultipleObjects]
    internal class AACapsuleColliderEditor : ColliderEditor
    {
        // SerializedProperty references for the capsule collider height and radius properties.
        protected SerializedFixedPoint64 height;
        protected SerializedFixedPoint64 radius;

        /// <summary>
        /// Initializes the editor by finding and setting up references to the serialized height and radius properties
        /// of the capsule collider when the collider editor is enabled.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // Finds and assigns the serialized 'heightInt' property from the target object.
            height = new SerializedFixedPoint64("Height",serializedObject.FindProperty("_height"));
            // Finds and assigns the serialized 'radiusInt' property from the target object.
            radius = new SerializedFixedPoint64("Radius",serializedObject.FindProperty("_radius"));
        }

        protected override void OnColliderChanged()
        {
            height.floatValue = Mathf.Max(0, height.floatValue);
            height.Apply();
            radius.floatValue = Mathf.Max(0, radius.floatValue);
            radius.Apply();
        }

        /// <summary>
        /// Renders the custom inspector GUI elements for the capsule collider.
        /// This method is called to draw the inspector fields for editing the height and radius properties of the collider.
        /// </summary>
        protected override void OnColliderInspectorGUI()
        {
            // Creates an inspector GUI field for editing the 'height' property.
            height.PropertyField();
            // Creates an inspector GUI field for editing the 'radius' property.
            radius.PropertyField();
        }
    }
}
