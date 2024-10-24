using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public interface IPooling<T> where T : class
{
    public ObjectPool<T> Pool { get; set;}
    
    public ObjectPool<T> CreatePool(bool hasCollectionCheck, int defaultPoolingCapacity, int maxPoolingCapacity, T prefab, object creatorObj);
    
    public ObjectPool<T> CreatePool(bool hasCollectionCheck, int defaultPoolingCapacity, int maxPoolingCapacity, T prefab);
}
