using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BlueNoah.PhysicsEngine
{
    public class GameObjectPoolManager : SimpleSingleMonoBehaviour<GameObjectPoolManager>
    {
        public readonly Dictionary<GameObject, GameObjectPool> poolDictionary = new ();
        public readonly Dictionary<int, GameObjectPool> poolDictionary1 = new ();
        public List<GameObjectPool> pools = new ();
        public int currentId { get; private set; }

        public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion qua, float unSpawnDelay)
        {
            var go = Spawn(prefab, pos, qua);
            UnSpawn(unSpawnDelay, go);
            return go;
        }

        public void Spawn(GameObject prefab, Vector3 pos, Quaternion qua, float delay, float unSpawnDelay)
        {
            StartCoroutine(_SpawnAndUnSpawn(prefab, pos, qua, delay, unSpawnDelay));
        }

        private IEnumerator _SpawnAndUnSpawn(GameObject prefab, Vector3 pos, Quaternion qua, float delay, float unSpawnDelay)
        {
            yield return new WaitForSeconds(delay);
            var go = Spawn(prefab, pos, qua);
            yield return new WaitForSeconds(unSpawnDelay);
            UnSpawn(go);
        }

        public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion qua)
        {
            if (!poolDictionary.ContainsKey(prefab))
            {
                AddPool(prefab, 10);
            }
            return poolDictionary[prefab].Spawn(pos, qua);
        }

        public GameObject SpawnParticleSystem(GameObject prefab, Vector3 pos, Quaternion qua, float unSpawn = 0)
        {
            if (!poolDictionary.ContainsKey(prefab))
            {
                AddPool(prefab, 10);
            }
            var go = poolDictionary[prefab].Spawn(pos, qua);
            var particle = go.GetComponentInChildren<ParticleSystem>();
            if (particle != null)
            {
                particle.Play(true);
            }
            if (unSpawn != 0)
            {
                UnSpawnParticleSystem(go, unSpawn);
            }
            return go;
        }

        public void UnSpawnParticleSystem(GameObject go, float delay)
        {
            //StopParticleSystem(go);
            UnSpawn(delay, go);
        }

        public void UnSpawn(float delay, GameObject go)
        {
            StartCoroutine(_UnSpawn(delay, go));
        }

        private IEnumerator _UnSpawn(float delay, GameObject go)
        {
            yield return new WaitForSeconds(delay);
            StopParticleSystem(go);
            yield return new WaitForSeconds(delay);
            UnSpawn(go);
        }

        public void UnSpawn(GameObject go)
        {
            var objectID = go.GetComponent<ObjectID>();
            if (objectID != null && poolDictionary1.TryGetValue(objectID.ID, out var value))
            {
                value.UnSpawn(go);
            }
            else
            {
                Destroy(go);
            }
        }

        public void AddPool(GameObject prefab, int num, Transform parentNode = null)
        {
            if (poolDictionary.TryGetValue(prefab, out var gameObjectPool1))
            {
                gameObjectPool1.PrePopulate(num);
            }
            else
            {
                var gameObjectPool = new GameObjectPool(prefab, num, currentId, parentNode);
                poolDictionary.Add(prefab, gameObjectPool);
                poolDictionary1.Add(currentId, gameObjectPool);
                pools.Add(gameObjectPool);
                currentId++;
            }
        }

        private static void StopParticleSystem(GameObject obj)
        {
            var particle = obj.GetComponentInChildren<ParticleSystem>();
            if (particle != null)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}