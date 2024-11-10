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

    protected virtual void Initialize(){

    }


    protected void InflictDamage(RaycastHit[] hits, float explosionRadiusMul =  1f){

        foreach(RaycastHit hit in hits){
            if(m_bulletData.explodeOnContact){
                Explosion explosion  = GlobalExplosionFactory.Instance.Pool.Get();

                explosion.Explode(hit.point, m_bulletData.explosionRadius * explosionRadiusMul, m_bulletData.damagePerShot, m_bulletData.damagableLayer);

            }
            else
            {
                if(hit.transform.TryGetComponent<IDamagable>(out IDamagable damagableObj))
                {
                    damagableObj.ApplyDamage(m_bulletData.damagePerShot);
                }
            }
        }
    }


}

