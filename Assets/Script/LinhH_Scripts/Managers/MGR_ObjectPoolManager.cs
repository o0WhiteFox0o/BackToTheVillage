// 
// Member: LinhH
// Date: 05/11/2025
// 


using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Thay thế việc tạo/hủy object trong trò chơi bằng việc bặt/tắt các object có sẳn. Giúp tăng hiệu năng của hệ thống.
/// </summary>
public class MGR_ObjectPoolManager : MonoBehaviour
{
    public static List<PooledObjectInfo> objectPools = new List<PooledObjectInfo>();

    private GameObject _objectPoolHolder;
    private static GameObject _plantHolder;
    private static GameObject _gameObjectHolder;
    public static ObjectPoolType poolType;


    void Awake()
    {
        SetupObjectPoolsEmpty();
    }
    

    private void Start() {
        DontDestroyOnLoad(this);
    }


    private void SetupObjectPoolsEmpty()
    {
        _objectPoolHolder = new GameObject("Object Pools Holder");

        _plantHolder = new GameObject("Plants Holder");
        _plantHolder.transform.SetParent(_objectPoolHolder.transform);

        _gameObjectHolder = new GameObject("Game Objects Holder");
        _gameObjectHolder.transform.SetParent(_objectPoolHolder.transform);
    }


    public static GameObject SpawnObject(GameObject spawnObj, Vector3 spawnPos, Quaternion spawnRot, ObjectPoolType objPoolType = ObjectPoolType.None)
    {
        PooledObjectInfo pool = objectPools.Find(p => p.lookupString == spawnObj.name);

        // check if the pool doesn't exist and create new one
        if (pool == null)
        {
            pool = new PooledObjectInfo() { lookupString = spawnObj.name };
            objectPools.Add(pool);
        }

        // check if there is any inactive object in the pool
        GameObject spawnableObj = pool.inactiveObjects.FirstOrDefault();

        // check if the spawnable object is null and create new object
        if (spawnableObj == null)
        {
            // find the parent of the empty object
            GameObject parentObject = SetParentObject(objPoolType);

            spawnableObj = Instantiate(spawnObj, spawnPos, spawnRot);

            if (parentObject != null)
            {
                spawnableObj.transform.SetParent(parentObject.transform);
            }
        }
        // if there is any inactive object in the pool, reuse it
        else
        {
            spawnableObj.transform.position = spawnPos;
            spawnableObj.transform.rotation = spawnRot;

            pool.inactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }


    /// <summary>
    /// Sinh một game object trong một transform truyền vào.
    /// </summary>
    public static GameObject SpawnObject(GameObject spawnObj, Transform parentTransform)
    {
        PooledObjectInfo pool = objectPools.Find(p => p.lookupString == spawnObj.name);

        // check if the pool doesn't exist and create new one
        if (pool == null)
        {
            pool = new PooledObjectInfo() { lookupString = spawnObj.name };
            objectPools.Add(pool);
        }

        // check if there is any inactive object in the pool
        GameObject spawnableObj = pool.inactiveObjects.FirstOrDefault();

        // check if the spawnable object is null and create new object
        if (spawnableObj == null)
        {
            spawnableObj = Instantiate(spawnObj, parentTransform);
        }
        // if there is any inactive object in the pool, reuse it
        else
        {
            pool.inactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }


    public static void ReturnObjectToPool(GameObject gameObj)
    {
        // remove the "(Clone)" word from the name of the passed object
        string gameObjName = gameObj.name.Substring(0, gameObj.name.Length - 7);

        PooledObjectInfo pool = objectPools.Find(p => p.lookupString == gameObjName);

        if (pool == null)
        {
            Debug.LogWarning("Try to release an object that is not pooled: " + gameObjName);
        }
        // inactivate the game object and add it to the pool
        else
        {
            gameObj.SetActive(false);
            pool.inactiveObjects.Add(gameObj);
        }
    }


    private static GameObject SetParentObject(ObjectPoolType poolType)
    {
        switch (poolType)
        {
            case ObjectPoolType.Plant:
                return _plantHolder;

            case ObjectPoolType.GameObject:
                return _gameObjectHolder;

            case ObjectPoolType.None:
                return null;

            default:
                return null;
        }
    }
}


public class PooledObjectInfo
{
    public string lookupString;
    public List<GameObject> inactiveObjects = new List<GameObject>();
}


public enum ObjectPoolType
{
    Plant,
    GameObject,
    None
}