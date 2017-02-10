using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool<T> where T : PooledObject<T>  {

    List<T> availableObjects = new List<T>();

    public ObjectPool()
    {

    }

    public T GetPooledObject(T obj)
    {
        T result;
        int lastIndex = availableObjects.Count - 1;
        if(lastIndex >= 0)
        {
            result = availableObjects[lastIndex];
            result.OnDespawn();
            availableObjects.RemoveAt(lastIndex);
            result = obj;
        }
        else
        {
            result = obj;
            result.pool = this;
        }
        result.OnSpawn();
        return result;
    }

    public void returnPooledObject(T obj)
    {
        availableObjects.Add(obj);
    }
}

public class MonoBehaviourPool<T> where T : PooledMonoBehaviour<T>
{

    List<T> availableObjects = new List<T>();

    PooledMonoBehaviour<T> poolPrefab;

    public MonoBehaviourPool(PooledMonoBehaviour<T> prefab)
    {
        poolPrefab = prefab;
    }

    public T GetPooledObject(Transform parent = null)
    {
        T result;
        int lastIndex = availableObjects.Count - 1;
        if (lastIndex >= 0)
        {
            result = availableObjects[lastIndex];
            result.gameObject.SetActive(true);
            availableObjects.RemoveAt(lastIndex);
        }
        else
        {
            result = (T)((parent != null ) ? Object.Instantiate(poolPrefab, parent) : Object.Instantiate(poolPrefab));
            result.pool = this;
        }
        result.OnSpawn();
        return result;
    }

    public void returnPooledObject(T obj)
    {
        obj.OnDespawn();
        obj.gameObject.SetActive(false);
        availableObjects.Add(obj);
    }
}
