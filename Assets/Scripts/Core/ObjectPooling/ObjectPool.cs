using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Deleting and creating objects in unity is a potentially expensive operation, think back to
// our C++ stuff with how we were told to initalise everything at the beginning of our programs and not to delete stuff til closing.
// Same deal here, although unity is more lenient, it's still expensive if the objects are being created in large amounts very quickly.
//
// The ObjectPool a generic solution to this issue, as instead of creating the objects, we reuse objects stored in the pool instead.


public class ObjectPool<T> where T : PooledObject<T>  {
    // Non-monobehavior pools REQUIRE the use of OnSpawn and OnDespawn to make it anymore efficent, as you'll need to disable it's functionality yourself, rather than using GameObject.SetActive

    List<T> availableObjects = new List<T>();

    public T GetPooledObject(T obj) // Get Object from pool if possible, otherwise add to pool.
    {
        T result;
        int lastIndex = availableObjects.Count - 1;
        if(lastIndex >= 0) 
        {
            result = availableObjects[lastIndex];
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

    public void returnPooledObject(T obj) //Return spawned object to it's pool
    {
        obj.OnDespawn();
        availableObjects.Add(obj);
    }
}

public class MonoBehaviourPool<T> where T : PooledMonoBehaviour<T>
{

    bool parented;
    List<T> availableObjects = new List<T>();

    PooledMonoBehaviour<T> poolPrefab;
    GameObject poolContainer;

    public MonoBehaviourPool(PooledMonoBehaviour<T> prefab)
    {
        poolPrefab = prefab;
        poolContainer = new GameObject(poolPrefab.name + " Pool");
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
            result = (T)((parent != null ) ? Object.Instantiate(poolPrefab, parent) : Object.Instantiate(poolPrefab, poolContainer.transform));
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
