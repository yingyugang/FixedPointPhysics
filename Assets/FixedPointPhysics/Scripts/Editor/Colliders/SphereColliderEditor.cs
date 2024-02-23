using UnityEditor;
using UnityEngine;

namespace BlueNoah.PhysicsEngine.Editor
{
    /// <summary>
    /// A custom editor for FixedPointSphereColliderPresenter, extending the ColliderPresenterEditor.
    /// This editor provides a user interface in the Unity Editor for editing the properties of a sphere collider
    /// with fixed-point precision, specifically focusing on the radius of the sphere.
    /// </summary>
    [CustomEditor(typeof(FPSphereCollider))]
    [CanEditMultipleObjects]
    internal sealed class SphereColliderEditor : ColliderEditor
    {
        private SerializedFixedPoint64 radius;

        /// <summary>
        /// Called when the editor is enabled. Initializes the editor by setting up references and preparing the serialized properties.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            radius = new SerializedFixedPoint64("Radius",serializedObject.FindProperty("_radius"));
        }
        
        /// <summary>
        /// Applies the changes made in the editor to the sphere collider radius, using a fixed-point conversion for precision.
        /// </summary>
        protected override void OnColliderChanged()
        {
            radius.floatValue = Mathf.Max(0, radius.floatValue);
            radius.Apply();
        }

        /// <summary>
        /// Renders the custom inspector GUI elements for the capsule collider.
        /// This method is called to draw the inspector fields for editing the height and radius properties of the collider.
        /// </summary>
        protected override void OnColliderInspectorGUI()
        {
            // Creates an inspector GUI field for editing the 'radius' property.
            radius.PropertyField();
        }
    }
}