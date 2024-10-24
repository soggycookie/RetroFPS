using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField]
    protected BulletSO m_bulletData;
    
    public    BulletManager Manager { get; set; }

    public abstract void Shoot();

    public BulletSO GetBulletData(){
        return m_bulletData;
    }
}
