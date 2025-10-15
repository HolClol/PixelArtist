using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ObjectPooling : MonoBehaviour { }

public class Pooling
{
    public static Dictionary<string, PoolingData> poolingDictionary = new Dictionary<string, PoolingData>();

    public static GameObject Spawn(string type, GameObject go, string parentname)
    {
        PoolingData poolData;
        string goFolderName = parentname;
        // Put object into a specific folder
        if (goFolderName == "")
            goFolderName = "_Fill";

        // Create new pool data if it has not existed
        if (!poolingDictionary.ContainsKey(type))
            poolingDictionary[type] = new PoolingData();

        poolData = poolingDictionary[type];

        // Find if game object already existed and ready for pooling
        if (poolData.deactiveList.Count > 0)
        {
            GameObject pooledObject = poolData.deactiveList[0];
            poolData.deactiveList.Remove(pooledObject);
            poolData.activeList.Add(pooledObject);
            return pooledObject;
        }
        else
        {
            GameObject newObject = GameObject.Instantiate(go, go.transform.position, Quaternion.identity, GameObject.Find(goFolderName).transform);
            poolData.activeList.Add(newObject);
            return newObject;
        }

    }

    public static void Despawn(string type, GameObject go)
    {
        PoolingData poolData;
        
        poolData = poolingDictionary[type];
        poolData.activeList.Remove(go);
        poolData.deactiveList.Add(go);
    }
    public static void Clear()
    {
        poolingDictionary.Clear();
    }

    public static void Clear(string type)
    {
        if (poolingDictionary.ContainsKey(type))
        {
            poolingDictionary[type].activeList.Clear();
            poolingDictionary[type].deactiveList.Clear();
        }
    }
}

public class PoolingData
{
    public List<GameObject> activeList = new List<GameObject>();
    public List<GameObject> deactiveList = new List<GameObject>();

}

