using UnityEngine;
using System.Collections;

public class PooledObject<T> where T : PooledObject<T>
{
    [System.NonSerialized]
    public ObjectPool<T> pool;

    public void ReturnToPool(T instance)
    {
        if(pool != null)
        {
            OnDespawn();
            pool.returnPooledObject(instance);
        }
    }

    public T GetInstanceFromPool(T obj)
    {
        if(pool == null)
        {
            pool = new ObjectPool<T>();
        }
        T result = pool.GetPooledObject(obj);
        result.OnSpawn();
        return result;
    }

    public virtual void OnSpawn() {}
    public virtual void OnDespawn() {}

}

public class PooledMonoBehaviour<T> : MonoBehaviour where T : PooledMonoBehaviour<T>
{
    
    [System.NonSerialized]
    public MonoBehaviourPool<T> pool;

    public void ReturnToPool(T instance)
    {
        if (pool != null)
        {
            OnDespawn();
            pool.returnPooledObject(instance);
        }
    }

    public T GetInstanceFromPool(Transform parent = null)
    {
        if (pool == null)
        {
            pool = new MonoBehaviourPool<T>(this);
        }
        T result = pool.GetPooledObject(parent);
        result.OnSpawn();
        return result;
    }

    public virtual void OnSpawn() { }
    public virtual void OnDespawn() { }

}