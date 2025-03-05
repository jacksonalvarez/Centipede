/*Copyright © Spoiled Unknown*/
/*2024*/

using System.Collections.Generic;
using UnityEngine;

namespace XtremeFPS.PoolingSystem
{
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Pool Manager")]
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }
    
        [System.Serializable]
        public class ObjectPoolItem
        {
            public GameObject objectToPool;
            public int objectAmount;
            public bool canExpand;
            public bool canRecycle;
        }
    
        public List<ObjectPoolItem> itemsToPool;
        private Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
        private Dictionary<GameObject, GameObject> prefabParents = new Dictionary<GameObject, GameObject>();
        private int lastRecycledIndex;

        private void Awake()
        {
            if (Instance != null) Destroy(this);
            else Instance = this;

            InitializeObjectPools();
        }
    
        private void InitializeObjectPools()
        {
            foreach (ObjectPoolItem item in itemsToPool)
            {
                if (item.objectToPool == null)
                {
                    Debug.LogError("The \"Object To Pool\" is null!");
                    return;
                }
                GameObject parentGameObject = new GameObject(item.objectToPool.name + " Pool");
                parentGameObject.transform.parent = transform;

                List<GameObject> objectPool = new List<GameObject>();
                for (int i = 0; i < item.objectAmount; i++)
                {
                    GameObject obj = Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    obj.transform.parent = parentGameObject.transform;
                    objectPool.Add(obj);
                }
                pooledObjects.Add(item.objectToPool, objectPool);
                prefabParents.Add(item.objectToPool, parentGameObject);
            }
        }

        private ObjectPoolItem FindObjectPoolItem(GameObject objectToPool)
        {
            foreach (ObjectPoolItem item in itemsToPool)
            {
                if (item.objectToPool != objectToPool) continue;
                return item;
            }
            return null;
        }

public GameObject SpawnObject(GameObject objectToPool, Vector3 position, Quaternion rotation)
{
    // ✅ Validate that the object exists in the pooling system
    ObjectPoolItem item = FindObjectPoolItem(objectToPool);
    if (item == null)
    {
        Debug.LogWarning($"{objectToPool?.name ?? "Unknown"} passed in SpawnObject() not found in \"itemsToPool\"!");
        return null;
    }

    if (!pooledObjects.ContainsKey(objectToPool))
    {
        Debug.LogWarning($"{objectToPool?.name ?? "Unknown"} passed in SpawnObject() not found in the Pool!");
        return null;
    }

    List<GameObject> objectPool = pooledObjects[objectToPool];

    // ✅ Try to find an inactive object
    foreach (GameObject obj in objectPool)
    {
        if (obj == null)
        {
            Debug.LogWarning("A pooled object was destroyed externally! Consider using SetActive(false) instead of Destroy().");
            continue;
        }

        if (!obj.activeInHierarchy)
        {
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }
    }

    // ✅ If expandable, instantiate a new object
    if (item.canExpand)
    {
        if (!prefabParents.ContainsKey(objectToPool))
        {
            Debug.LogError($"Prefab parent for {objectToPool.name} is missing! Fix the pooling system.");
            return null;
        }

        GameObject newObj = Instantiate(objectToPool, position, rotation);
        newObj.transform.parent = prefabParents[objectToPool].transform;
        newObj.SetActive(true);
        objectPool.Add(newObj);
        return newObj;
    }

    // ✅ Cycle through to find an object to recycle
    int startIndex = (lastRecycledIndex + 1) % objectPool.Count;
    for (int i = 0; i < objectPool.Count; i++)
    {
        int index = (startIndex + i) % objectPool.Count;
        GameObject recycledObj = objectPool[index];

        if (recycledObj == null)
        {
            Debug.LogWarning("Skipping a destroyed object in the pool.");
            continue;
        }

        // Recycle the object
        recycledObj.transform.SetPositionAndRotation(position, rotation);
        recycledObj.SetActive(true);
        lastRecycledIndex = index;
        return recycledObj;
    }

    return null; // No available object
}

public void DespawnObject(GameObject obj)
{
    if (obj == null)
    {
        Debug.LogWarning("Tried to despawn a null object.");
        return;
    }

    bool foundInPool = false;
    foreach (var objectPool in pooledObjects.Values)
    {
        if (!objectPool.Contains(obj)) continue;

        obj.SetActive(false);
        obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity); // ✅ Reset position to avoid lingering effects
        foundInPool = true;
        break;
    }

    if (!foundInPool)
    {
        Debug.LogWarning($"The object {obj.name} is not managed by the object pool system.");
    }
}}}
