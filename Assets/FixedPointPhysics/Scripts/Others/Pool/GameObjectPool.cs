using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    [System.Serializable]
    public class GameObjectPool
    {
        public int ID { get; private set; }
        private GameObject parent;
        public GameObject prefab { get; private set; }
        public int totalObjCount { get; private set; }
        private FPFastList<GameObject> available = new ();
        private FPFastList<GameObject> allObject = new ();

        public GameObjectPool(GameObject obj, int num, int id, Transform specialParent = null)
        {
#if UNITY_EDITOR
            if (specialParent == null)
            {
                parent = new GameObject
                {
                    name = "Pool_" + obj.name
                };
            }
#endif
            if (specialParent != null)
            {
                if (parent != null) parent.transform.parent = specialParent;
                parent = specialParent.gameObject;
            }
            prefab = obj;
            ID = id;
            PrePopulate(num);
        }

        public void MatchPopulation(int num)
        {
            PrePopulate(num - totalObjCount);
        }

        public void PrePopulate(int num)
        {
            for (var i = 0; i < num; i++)
            {
                var obj = parent != null ? Object.Instantiate(prefab, parent.transform) : Object.Instantiate(prefab);
                obj.AddComponent<ObjectID>().SetID(ID);
                available.Add(obj);
                allObject.Add(obj);
                totalObjCount += 1;
                obj.SetActive(false);
            }
        }

        public GameObject Spawn(Vector3 pos, Quaternion rot)
        {
            GameObject spawnObj;
            if (available.Count > 0)
            {
                spawnObj = available[0];
                available.Remove(spawnObj);
                if (spawnObj == null)
                {
                    Debug.LogError($"[Pool] the gameObject:{prefab} has been destroyed!");
                    spawnObj = parent != null ? Object.Instantiate(prefab, pos, rot, parent.transform) : Object.Instantiate(prefab, pos, rot);
                    spawnObj.AddComponent<ObjectID>().SetID(ID);
                    allObject.Add(spawnObj);
                }
                else
                {
                    spawnObj.SetActive(true);
                    var tempT = spawnObj.transform;
                    if (parent != null)
                    {
                        tempT.localPosition = pos;
                        tempT.localRotation = rot;
                    }
                    else
                    {
                        tempT.position = pos;
                        tempT.rotation = rot;
                    }
                }
            }
            else
            {
                spawnObj = parent != null ? Object.Instantiate(prefab, pos, rot, parent.transform) : Object.Instantiate(prefab, pos, rot);
                spawnObj.AddComponent<ObjectID>().SetID(ID);
                allObject.Add(spawnObj);
                totalObjCount += 1;
            }
            return spawnObj;
        }

        public void UnSpawn(GameObject obj)
        {
            available.Add(obj);
            obj.SetActive(false);
        }

        public void UnSpawnAll()
        {
            foreach (GameObject obj in allObject)
            {
                if (obj != null) Object.Destroy(obj);
            }
        }

        public void HideInHierarchy(Transform t)
        {
            foreach (GameObject obj in allObject)
            {
                if (obj != null)
                {
                    obj.transform.parent = t;
                }
            }
        }
    }


    [System.Serializable]
    public class ObjectID : MonoBehaviour
    {
        public int ID = -1;

        public void SetID(int id)
        {
            ID = id;
        }

        public int GetID()
        {
            return ID;
        }
    }
}