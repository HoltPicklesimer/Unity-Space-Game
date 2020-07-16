using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************
 * ObjectPooler
 * The object pool that is used to create and spawn objects so
 * objects don't have to be spawned at runtime.
 * *************************************************************/
public class ObjectPooler : MonoBehaviour
{
    /*********************************************
     * Pool
     * The base pool class that ObjectPooler uses
     * to create each object pool.
     * *******************************************/
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        // Instantiate the object pools
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; ++i)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                // Add the object to the queue of the pool
                objectPool.Enqueue(obj);
            }

            // Add the pool to the dictionary/list of pools
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool (string tag, Vector3 position, Quaternion rotation)
    {
        // Check if the pool exists in the pool dictionary
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " does not exists.");
            return null;
        }

        // Remove the top object from the queue/pool
        var spawnObj = poolDictionary[tag].Dequeue();

        spawnObj.SetActive(true);
        spawnObj.transform.position = position;
        spawnObj.transform.rotation = rotation;

        var pooledObj = spawnObj.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            // Execute a start-like method for the object when it is spawned.
            pooledObj.OnObjectSpawn();
        }

        // Add the object back to the bottom of the queue/pool
        poolDictionary[tag].Enqueue(spawnObj);

        return spawnObj;
    }
}
