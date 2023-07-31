using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    private Queue<T> Pool;
    private T Prefab;
    private Transform ObjectsHolder;

    public void Setup(T prefab, Transform objectsHolder, int initSize = 0)
    {
        Pool = new Queue<T>();
        Prefab = prefab;
        ObjectsHolder = objectsHolder;

        for (int i = 0; i < initSize; i++)
            SetObject(Spawn());
    }

    public T GetObject()
    {
        if (Pool.Count > 0)
            return Pool.Dequeue();
        else
            return Spawn();
    }

    public void SetObject(T obj) => Pool.Enqueue(obj);

    private T Spawn() => Instantiate(Prefab, ObjectsHolder);
}
