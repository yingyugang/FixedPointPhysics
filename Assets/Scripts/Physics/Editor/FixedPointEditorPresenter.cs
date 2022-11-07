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
        [MenuItem("GameObject/3D FixedPoint Object/OBB", priority = 2)]
        static void CreateOBB()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.AddComponent<FixedPointOBBColliderPresenter>();
            go.name = "OBB";
        }
        [MenuItem("GameObject/3D FixedPoint Object/Sphere", priority = 3)]
        static void CreateSphere()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.AddComponent<FixedPointSphereColliderPresenter>();
            go.name = "Sphere";
        }
        [MenuItem("GameObject/3D FixedPoint Object/Triangle", priority = 4)]
        static void CreateTriangle()
        {
            var go = new GameObject("Triangle");
            go.AddComponent<MeshFilter>();
            var renderer = go.AddComponent<MeshRenderer>();
            var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Default.mat");
            renderer.material = mat;
            var triangle = go.AddComponent<FixedPointTriangleColliderPresenter>();
            go.transform.position = Vector3.zero;
            triangle.SetVertices(new Math.FixedPoint.FixedPointVector3(0,0,0), new Math.FixedPoint.FixedPointVector3(0, 10, 0), new Math.FixedPoint.FixedPointVector3(10, 10, 0));
        }
    }
}