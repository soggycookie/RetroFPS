using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletFactory : MonoBehaviour
{
    public ObjectPool<Bullet> Pool { get; set; }
    
    [SerializeField]
    private int m_defaultCapacity;

    [SerializeField]
    private int m_maxCapacity;
    
    public void CreatePool(Bullet prefab, BulletManager manager)
    {
        GameObject poolGO = new GameObject("Bullet pool");
        poolGO.transform.position = Vector3.up * 100f;

        Pool = new ObjectPool<Bullet>(() => {

            //Debug.Log("before");
            Bullet bullet = Instantiate(prefab, poolGO.transform);
            //Debug.Log("after");
            bullet.Manager = manager;
            //Debug.Log("pop");

            return bullet;
        }, obj => {
            obj.gameObject.SetActive(true);
        }, obj => {
            obj.gameObject.SetActive(false);
        }, obj => {
            Destroy(obj);
        }, true, m_defaultCapacity, m_maxCapacity);

    }
}
