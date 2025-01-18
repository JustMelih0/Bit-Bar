using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    public static PoolManager Instance;
    public List<Pool> pools = new List<Pool>();
    public Dictionary<string, List<GameObject>> poolDictioanry = new Dictionary<string, List<GameObject>>();
    public Dictionary<string, Transform> poolParents = new Dictionary<string, Transform>();


    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start() {
        CreatePool();
    }
    private void CreatePool()
    {
        foreach (Pool pool in pools)
        {
            List<GameObject> objectPool = new List<GameObject>();
            GameObject parentObject = new GameObject(pool.tag + " Pool");
            parentObject.transform.SetParent(this.transform);
            poolParents[pool.tag] = parentObject.transform;

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(parentObject.transform);
                objectPool.Add(obj);
            }
            poolDictioanry.Add(pool.tag, objectPool);

        }
    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictioanry.ContainsKey(tag))
        {
            Debug.Log("Havuzda bu taga sahip bir nesne yok");
            return null;
        }
        GameObject objectSpawn = null;

        foreach (GameObject item in poolDictioanry[tag])
        {
            if (!item.activeInHierarchy)
            {
                objectSpawn = item;
                break;
            }
        }
        if (objectSpawn == null)
        {
            objectSpawn = ExpandPool(tag);
        }
        if (objectSpawn != null)
        {
            objectSpawn.SetActive(true);
            objectSpawn.transform.position = position;
            objectSpawn.transform.rotation = rotation;
        }
        return objectSpawn;
    }
    private GameObject ExpandPool(string tag)
    {
        if(!poolParents.ContainsKey(tag)) return null;

        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(poolParents[tag]);
                poolDictioanry[tag].Add(obj);
                return obj;
            }
        }

        return null;
    }
}
