using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObstacleHitFactory : MonoBehaviour
{
    public ObjectPool<ObstacleHitParticle> Pool { get; private set; }
    
    [SerializeField]
    private int m_defaultCapacity;

    [SerializeField]
    private int m_maxCapacity;
   

    public void CreatePool(ObstacleHitParticle prefab)
    {
        GameObject poolGO = new GameObject("Paricle pool");
        poolGO.transform.position = Vector3.up * 100f;

        Pool = new ObjectPool<ObstacleHitParticle>(() => {

            //Debug.Log("before");
            ObstacleHitParticle obj= Instantiate(prefab, poolGO.transform);
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
