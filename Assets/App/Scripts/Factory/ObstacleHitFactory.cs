using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObstacleHitFactory : MonoBehaviour
{
    public ObjectPool<BulletHitVFX> Pool { get; private set; }
    
    [SerializeField]
    private int m_defaultCapacity;

    [SerializeField]
    private int m_maxCapacity;
   

    public void CreatePool(BulletHitVFX prefab)
    {
        GameObject poolGO = new GameObject("Paricle pool");
        poolGO.transform.position = Vector3.up * 100f;

        Pool = new ObjectPool<BulletHitVFX>(() => {

            //Debug.Log("before");
            BulletHitVFX obj= Instantiate(prefab, poolGO.transform);
            obj.Factory = this;

            //Debug.Log("after");
            //Debug.Log("pop");

            return obj;
        }, obj => {
            obj.gameObject.SetActive(true);
        }, obj => {
            obj.gameObject.SetActive(false);
        }, obj => {
            Destroy(obj);
        }, true, m_defaultCapacity, m_maxCapacity);

    }



}
