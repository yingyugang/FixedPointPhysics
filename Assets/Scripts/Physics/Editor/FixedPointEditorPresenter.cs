using UnityEditor;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class FixedPointEditorPresenter 
    {
        [MenuItem("GameObject/3D FixedPoint Object/AABB", priority = 1)]
        static void CreateAABB()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.AddComponent<FixedPointAABBColliderPresenter>();
            go.name = "AABB";
        }
        [MenuItem("GameObject/3D FixedPoint Object/OBB", priority = 1)]
        static void CreateOBB()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.AddComponent<FixedPointOBBColliderPresenter>();
            go.name = "OBB";
        }
        [MenuItem("GameObject/3D FixedPoint Object/Sphere", priority = 1)]
        static void CreateSphere()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.AddComponent<FixedPointSphereColliderPresenter>();
            go.name = "Sphere";
        }
    }
}