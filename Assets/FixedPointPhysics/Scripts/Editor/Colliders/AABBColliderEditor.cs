using UnityEditor;
using UnityEngine;

namespace BlueNoah.PhysicsEngine.Editor
{
    /// <summary>
    /// Custom editor for the AABBCollider component, providing enhanced UI elements in the Unity Editor.
    /// This editor allows for more intuitive configuration of the AABBCollider's properties.
    /// </summary>
    [CustomEditor(typeof(FPAABBCollider))]
    [CanEditMultipleObjects]
    public class AABBColliderEditor : ColliderEditor
    {
        private SerializedFixedPointVector3 size; // Wrapper for serialized size property to enable editing within the Unity Editor.

        /// <summary>
        /// Initializes the custom editor, setting up serialized properties for editing.
        /// This method is called when the editor is enabled and ensures the UI reflects the current state of the AABBCollider component.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable(); // Calls the base class's OnEnable method to handle common setup tasks.
            // Initializes the size property wrapper with the "_size" field from the serialized object (AABBCollider).
            size = new SerializedFixedPointVector3("Size", serializedObject.FindProperty("_size"));
        }

        /// <summary>
        /// Applies changes to the collider's properties based on user input in the editor.
        /// This method ensures that the size values are non-negative and updates the serialized object to reflect these changes.
        /// </summary>
        protected override void OnColliderChanged()
        {
            // Ensures that each component of the size vector is non-negative.
            size.vector3Value = new Vector3(Mathf.Max(0, size.vector3Value.x), Mathf.Max(0, size.vector3Value.y),
                Mathf.Max(0, size.vector3Value.z));
            size.Apply(); // Applies the changes to the serialized object.
        }

        /// <summary>
        /// Defines the custom UI elements to be displayed in the Inspector for the AABBCollider component.
        /// This method is responsible for rendering the editable fields in the Unity Editor.
        /// </summary>
        protected override void OnColliderInspectorGUI()
        {
            size.PropertyField(); // Renders a property field in the editor for the size property, allowing it to be edited.
        }
    }
}