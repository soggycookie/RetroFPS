using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleSystemFactory : BulletFactory, IPooling<BulletHitVFX>
{
    public ObjectPool<BulletHitVFX> Pool { get; set; }


    public ObjectPool<BulletHitVFX> CreatePool(bool hasCollectionCheck, int defaultPoolingCapacity, int maxPoolingCapacity, BulletHitVFX prefab, object creatorObj)
    {
        GameObject poolGO = new GameObject("Particle pool");

        if (creatorObj is not BulletManager)
            Debug.LogError("//ParticleSystemFactory// Not cast to the correct type");

        BulletManager bulletBehavior = creatorObj as BulletManager;

        ObjectPool<BulletHitVFX> bulletPool = new ObjectPool<BulletHitVFX>(() => {

            BulletHitVFX particleSystem = Instantiate(prefab, poolGO.transform);
            particleSystem.BulletBehavior = bulletBehavior;

            return particleSystem;
        }, obj => {

            obj.gameObject.SetActive(true);
        }, obj => {
            obj.gameObject.SetActive(false);
        }, obj => {
            Destroy(obj);
        }, hasCollectionCheck, defaultPoolingCapacity, maxPoolingCapacity);

        return bulletPool;
    }

    public ObjectPool<BulletHitVFX> CreatePool(bool hasCollectionCheck, int defaultPoolingCapacity, int maxPoolingCapacity, BulletHitVFX prefab)
    {
        throw new System.NotImplementedException();
    }
}
