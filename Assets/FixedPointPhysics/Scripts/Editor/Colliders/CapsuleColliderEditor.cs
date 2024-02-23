using UnityEditor;

namespace BlueNoah.PhysicsEngine.Editor
{
    /// <summary>
    /// A custom editor for the FixedPointCapsuleColliderPresenter, extending the functionality of the BaseCapsuleColliderPresenterEditor.
    /// This editor provides a specialized interface for editing FixedPointCapsuleCollider properties within the Unity Editor.
    /// </summary>
    [CustomEditor(typeof(FPCapsuleCollider))]
    [CanEditMultipleObjects]
    internal sealed class CapsuleColliderEditor : AACapsuleColliderEditor
    {

    }
}