using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class HitscanBullet : Bullet
{
    
    public UnityAction<Vector3[]>    OnShoot;

    [SerializeField]
    private HitscanBulletVisual m_hitscanBulletVisual;

    private void Awake()
    {
        Initialze();
    }

    protected virtual void Initialze() {
        m_hitscanBulletVisual = GetComponent<HitscanBulletVisual>();

        if (m_bulletData is not HitScanBulletSO)
        {
            Debug.LogError("Bullet data is not hitscan data!");
        }

        m_hitscanBulletVisual.OnFinishFadeOut += () => {
            Manager.f_bullet.Pool.Release(this);
        };
    }
}
