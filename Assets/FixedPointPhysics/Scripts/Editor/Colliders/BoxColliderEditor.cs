using BlueNoah.Math.FixedPoint;
using UnityEditor;

namespace BlueNoah.PhysicsEngine.Editor
{
    /// <summary>
    /// A custom editor for the FixedPointOBBColliderPresenter, extending the BaseBoxColliderPresenterEditor.
    /// It provides a specialized interface for editing properties of an Oriented Bounding Box (OBB) collider with fixed-point precision in the Unity Editor.
    /// </summary>
    [CustomEditor(typeof(FPBoxCollider))]
    [CanEditMultipleObjects]
    internal sealed class BoxColliderEditor : AABBColliderEditor
    {
    }
}