/*
* Create 2022/11/1
* 応彧剛　yingyugang@gmail.com
* It's used by fixedpoint physics system.
*/
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public static class FixedPointEditorPresenter
    {

        [MenuItem("GameObject/3D FixedPoint Object/AABB", priority = 1)]
        private static void CreateAABB()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.DestroyImmediate(go.GetComponent<UnityEngine.Collider>());
            go.transform.localScale = Vector3.one;
            go.AddComponent<FPAABBCollider>();
            go.name = "AABB";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/OBB", priority = 2)]
        private static void CreateOBB()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.DestroyImmediate(go.GetComponent<UnityEngine.Collider>());
            go.AddComponent<FPBoxCollider>();
            go.name = "OBB";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/Sphere", priority = 3)]
        private static void CreateSphere()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.DestroyImmediate(go.GetComponent<UnityEngine.Collider>());
            go.AddComponent<FPSphereCollider>();
            go.name = "Sphere";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/AACapsule", priority = 5)]
        private static void CreateAACapsule()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            Object.DestroyImmediate(go.GetComponent<UnityEngine.Collider>());
            go.AddComponent<FPAACapsuleCollider>();
            go.name = "AACapsule";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/Capsule", priority = 6)]
        private static void CreateCapsule()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            Object.DestroyImmediate(go.GetComponent<UnityEngine.Collider>());
            go.AddComponent<FPCapsuleCollider>();
            go.name = "Capsule";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/Cylinder", priority = 7)]
        private static void CreateCylinder()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Object.DestroyImmediate(go.GetComponent<UnityEngine.Collider>());
            go.AddComponent<FPCylinderCollider>();
            go.name = "Cylinder";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/Mesh", priority = 9)]
        private static void CreateMesh()
        {
            var prefab = Resources.Load("Prefabs/DefaultMeshCollider");
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "DefaultMeshCollider";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/Hammer", priority = 60)]
        private static void CreateHammer()
        {
            var prefab = Resources.Load("Gimmiks/Hammer");
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "Hammer";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/Vertical Bumper", priority = 61)]
        private static void CreateVerticalBumper()
        {
            var prefab = Resources.Load("Gimmiks/VerticalBumper");
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "VerticalBumper";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/Horizontal Bumper", priority = 62)]
        private static void CreateHorizontalBumper()
        {
            var prefab = Resources.Load("Gimmiks/HorizontalBumper");
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "HorizontalBumper";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/ConveyorBelt", priority = 62)]
        private static void CreateConveyorBelt()
        {
            var prefab = Resources.Load("Gimmiks/ConveyorBelt");
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "ConveyorBelt";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/FallPlatform", priority = 63)]
        private static void CreateFallPlatform()
        {
            var prefab = Resources.Load("Gimmiks/FallPlatform");
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "FallPlatform";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/RotateBeam", priority = 64)]
        private static void CreateRotateBeam()
        {
            var prefab = Resources.Load("Gimmiks/RotateBeam");
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "RotateBeam";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/CoinBox", priority = 100)]
        private static void CreateCoinBox()
        {
            var prefab = Resources.Load("Gimmiks/CoinBox");
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "CoinBox";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/GoalCube", priority = 200)]
        private static void CreateGoalCube()
        {
            var prefab = Resources.Load("Gimmiks/GoalCubeBlock");
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "GoalCubeBlock";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/GoalSphere", priority = 201)]
        private static void CreateGoalSphere()
        {
            var prefab = Resources.Load("Gimmiks/GoalSphereBlock");
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "GoalSphereBlock";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/CheckPointBlock", priority = 202)]
        private static void CreateCheckPointCube()
        {
            var prefab = Resources.Load("Gimmiks/CheckPointBlock");
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "CheckPointBlock";
            AddToCurrentOpen(go);
        }
        [MenuItem("GameObject/3D FixedPoint Object/MovingPlatform", priority = 62)]
        private static void CreateMovingPlatform()
        {
            var prefab = Resources.Load("Gimmiks/MovingPlatform");
            var go = Object.Instantiate(prefab) as GameObject;
            go.name = "MovingPlatform";
            AddToCurrentOpen(go);
        }

        private static void AddToCurrentOpen(GameObject go)
        {
            //Make sure put it into the prefab when prefab mode.
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                go.transform.SetParent(PrefabStageUtility.GetCurrentPrefabStage().scene.GetRootGameObjects()[0].transform);
            }
            Selection.activeGameObject = go;
        }
    }
}