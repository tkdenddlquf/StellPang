using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Object
{
    public DequeueFunc dequeueFunc;
    public EnqueueFunc enqueueFunc;

    private readonly T poolObject;
    private readonly Transform parent;

    private readonly Queue<T> poolObjects = new();

    public delegate void DequeueFunc(T _object);
    public delegate void EnqueueFunc(T _object);

    public ObjectPool(T _poolObject, Transform _parent)
    {
        poolObject = _poolObject;
        parent = _parent;
    }

    public T Dequeue()
    {
        if (poolObjects.Count == 0) poolObjects.Enqueue(Object.Instantiate(poolObject, parent));

        dequeueFunc?.Invoke(poolObjects.Peek());

        return poolObjects.Dequeue();
    }

    public void Enqueue(T _object)
    {
        enqueueFunc?.Invoke(_object);

        poolObjects.Enqueue(_object);
    }
}
