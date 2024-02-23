#if UNITY_EDITOR
using UnityEditor; 
using UnityEditor.SceneManagement; 
using UnityEngine; 
using UnityEngine.SceneManagement; 

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// A class that automatically initializes and attaches to editor events for handling actions during scene saves and updates.
    /// </summary>
    [InitializeOnLoad]
    public class SceneSaveHandler
    {
        
        /// <summary>
        /// Static constructor to subscribe to editor events.
        /// </summary>
        static SceneSaveHandler()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            // Subscribe to the sceneSaving event to perform actions before a scene is saved.
            EditorSceneManager.sceneSaving += OnSceneSaving;
            // Subscribe to the editor update event, which is called with each editor tick (useful for ongoing checks or operations).
            EditorApplication.update += OnUpdate;
        }
        
        /// <summary>
        /// This method is called when a scene is opened in Unity.
        /// </summary>
        private static void OnSceneOpened(Scene scene, OpenSceneMode openSceneMode)
        {
            // Search for any existing instances of FixedPointPhysicsPresenter in the scene.
            // The search includes inactive objects by setting the second parameter to true.
            var fixedPointPhysicsPresenter = Object.FindObjectOfType<FPPhysicsPresenter>(true);

            // If an instance already exists, return immediately without creating a new one.
            if (fixedPointPhysicsPresenter != null) return;

            // If no instance is found, create a new GameObject to hold the FixedPointPhysicsPresenter component.
            var go = new GameObject("FixedPointPhysicsPresenter")
            {
                // Hide the GameObject in the hierarchy to keep the scene clean and organized.
                hideFlags = HideFlags.HideInHierarchy
            };

            // Add the FixedPointPhysicsPresenter component to the newly created GameObject.
            go.AddComponent<FPPhysicsPresenter>();
            EditorUtility.SetDirty(go);
        }

        
        /// <summary>
        /// Event handler called when a scene is being saved.
        /// </summary>
        /// <param name="scene">The scene that is being saved.</param>
        /// <param name="path">The path to the scene file.</param>
        private static void OnSceneSaving(Scene scene, string path)
        {
            // Find all objects of type FixedPointTransformPresenter, including inactive ones, without sorting.
            var fixedPointTransforms = Object.FindObjectsOfType<FPTransform>(true);
            foreach (var item in fixedPointTransforms)
            {
                // If the item successfully serializes (indicating changes), mark it as dirty to ensure it is saved.
                if (item.Serialize())
                {
                    EditorUtility.SetDirty(item);
                }
            }
        }

        /// <summary>
        /// Method regularly called by the EditorApplication.update event.
        /// </summary>
        private static void OnUpdate()
        {
            // Check and add missing FixedPointTransform components during editor updates.
            AddMissingFixedPointTransform();
        }
        
        /// <summary>
        /// Checks for and adds missing FixedPointTransformPresenter components.
        /// </summary>
        private static void AddMissingFixedPointTransform()
        {
            // Check if currently editing a prefab in the prefab stage.
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null && prefabStage.prefabContentsRoot != null)
            {
                // If editing a prefab, add missing components to the root of the prefab.
                AddMissingFixedPointTransform(prefabStage.prefabContentsRoot.transform);
                return;
            }
            // If not in prefab editing mode, get the active scene and selected objects.
            var scene = SceneManager.GetActiveScene();
            var gameObjects = Selection.gameObjects;
            foreach (var gameObject in gameObjects)
            {
                // Skip objects not in the active scene.
                if (gameObject.scene != scene) continue;
                if (gameObject.GetComponent<FPTransform>() == null)
                {
                    continue;
                }
                // Add missing components to the root object of the selection.
                AddMissingFixedPointTransform(gameObject.transform.root);
            }
        }

        /// <summary>
        /// Adds a FixedPointTransformPresenter component to a transform if it is missing.
        /// Recursively applies this check and addition to all child transforms.
        /// </summary>
        /// <param name="trans">The transform to check and potentially add the component to.</param>
        private static void AddMissingFixedPointTransform(Transform trans)
        {
            // If the transform does not have a FixedPointTransformPresenter, add one.
            if (trans.GetComponent<FPTransform>() == null)
            {
                trans.gameObject.AddComponent<FPTransform>();
            }
            // Serialize the component and mark it as dirty if serialization occurs.
            var fixedPointTransformPresenter = trans.GetComponent<FPTransform>();
            if (fixedPointTransformPresenter.Serialize())
            {
                EditorUtility.SetDirty(fixedPointTransformPresenter);
            }
            // Recursively perform this operation on all children.
            foreach (Transform child in trans)
            {
                AddMissingFixedPointTransform(child);
            }
        }
    }
}
#endif