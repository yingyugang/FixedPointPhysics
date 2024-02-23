using UnityEditor;

namespace BlueNoah.PhysicsEngine.Editor
{
    /// <summary>
    /// A custom editor for the MeshCollider component.
    /// </summary>
    [CustomEditor(typeof(FPMeshCollider))]
    [CanEditMultipleObjects]
    public class MeshColliderEditor : ColliderEditor
    {
    }
}