using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GlobalExplosionFactory : MonoBehaviour
{
    public ObjectPool<Explosion> Pool { get; set; }
    
    [SerializeField]
    private Explosion m_explosionPrefab;

    [SerializeField]
    private int m_defaultPoolingCapacity;

    [SerializeField]
    private int m_maxPoolingCapacity;

    public static GlobalExplosionFactory Instance { get; private set; }
    

    private void CreatePool()
    {
        Pool = new ObjectPool<Explosion>(() => {
            Explosion explosion = Instantiate(m_explosionPrefab, transform);


            return explosion;
        }, obj => {
            obj.gameObject.SetActive(true);
        }, obj => {
            obj.gameObject.SetActive(false);
        }, obj => {
            Destroy(obj);
        }, true, m_defaultPoolingCapacity, m_maxPoolingCapacity);

    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        CreatePool();
    }

    private void OnDisable()
    {
        Destroy(this);
    }
}
