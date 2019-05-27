using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    [System.Serializable]
    public class Pool {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    public List<Pool> pools;
    public Dictionary<string, Queue<IPooledObject>> poolDictionary;
    public static ObjectPooler Instance;

    private void Awake() {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start() {
        poolDictionary = new Dictionary<string, Queue<IPooledObject>>();
        foreach(Pool pool in pools) {
            Queue<IPooledObject> objectPool = new Queue<IPooledObject>();
            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj.GetComponent<IPooledObject>());
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public void SpawnFromPool(string tag, Vector3 position, Quaternion rotation) {
        if (!poolDictionary.ContainsKey(tag)) {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return;
        }
        IPooledObject objectToSpawn = poolDictionary[tag].Dequeue();
        //objectToSpawn.SetActive(true);
        //objectToSpawn.transform.position = position;
        //objectToSpawn.transform.rotation = rotation;
        poolDictionary[tag].Enqueue(objectToSpawn);
    }

    public IPooledObject SpawnFromPool(string tag) {
        if (!poolDictionary.ContainsKey(tag)) {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }
        IPooledObject objectToSpawn = poolDictionary[tag].Dequeue();
        //objectToSpawn.SetActive(true);
        //objectToSpawn.onObjectSpawn();
        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
