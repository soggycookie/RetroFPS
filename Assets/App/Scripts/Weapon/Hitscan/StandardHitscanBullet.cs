using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(HitscanBulletVisual))]
public class StandardHitscanBullet : HitscanBullet
{



    public override void Shoot()
    {
        HitScanBulletSO m_hitscanData = m_bulletData as HitScanBulletSO;
        
        Vector3[] linePos;
        RaycastHit hit;
        bool isHit;

        isHit = HitscanBehavior.GetSingleHitData(m_hitscanData, Manager.FPSCamera.position, Manager.FPSCamera.forward, out hit);

        if (!isHit)
        {
            linePos = new Vector3[] { Manager.MuzzlePoint.position, Manager.FPSCamera.position + Manager.FPSCamera.forward * m_hitscanData.maxDistance };

        }
        else
        {
            linePos = new Vector3[] { Manager.MuzzlePoint.position, hit.point};
            
            Explosion explosion = FindAnyObjectByType<Explosion>();
            
            explosion.Explode(hit.point, 5, m_bulletData.damagableLayer);
        }

        OnShoot?.Invoke(linePos);


    }
}
