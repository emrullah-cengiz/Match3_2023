using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    private Queue<T> Pool;
    private T Prefab;
    private Transform ObjectsHolder;

    public void Setup(T prefab, Transform objectsHolder)
    {
        Pool = new Queue<T>();
        Prefab = prefab;
        ObjectsHolder = objectsHolder;
    }

    public T GetObject()
    {
        if (Pool.Count > 0)
            return Pool.Dequeue();
        else
            return Instantiate(Prefab, ObjectsHolder);
    }

    public void SetObject(T obj) => Pool.Enqueue(obj);
}
