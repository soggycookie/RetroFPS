using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitscanBulletVisual))]
public class PiercingHitscanBullet : HitscanBullet
{
    public override void Shoot()
    {
        HitScanBulletSO m_hitscanData = m_bulletData as HitScanBulletSO;

        var hitsAndVisualRay = HitscanBehavior.GetPiercingHitData(m_hitscanData, Manager.FPSCamera.position, Manager.FPSCamera.forward);

        Vector3[] linePos;


        RaycastHit[] hits = hitsAndVisualRay.Item1;
        Vector3 blockedPoint = Vector3.zero;
        bool isBlocked = false;


        bool isVisualRayHit = hitsAndVisualRay.Item2;
        RaycastHit visualRay = hitsAndVisualRay.Item3;


        if (hits.Length == 0)
        {
            linePos = new Vector3[] { Manager.MuzzlePoint.position, Manager.FPSCamera.position + Manager.FPSCamera.forward * m_hitscanData.maxDistance };

            OnShoot?.Invoke(linePos);

            return;
        }

        foreach (RaycastHit hit in hits)
        {
            var bitmaskLayer = 1 << hit.transform.gameObject.layer;

            if ((bitmaskLayer | m_hitscanData.blockLayer.value) == m_hitscanData.blockLayer.value)
            {

                blockedPoint = hit.point;
                isBlocked = true;

                break;
            }

        }

        if (isBlocked)
        {

            if (isVisualRayHit)
            {
                linePos = new Vector3[] { Manager.MuzzlePoint.position, visualRay.point, blockedPoint };
            }
            else
            {
                linePos = new Vector3[] { Manager.MuzzlePoint.position, blockedPoint };
            }
        }
        else
        {
            if (isVisualRayHit)
            {
                linePos = new Vector3[] { Manager.MuzzlePoint.position, visualRay.point, Manager.FPSCamera.position + Manager.FPSCamera.forward * m_hitscanData.maxDistance };
            }
            else
            {
                linePos = new Vector3[] { Manager.MuzzlePoint.position, Manager.FPSCamera.position + Manager.FPSCamera.forward * m_hitscanData.maxDistance };

            }
        }

        OnShoot?.Invoke(linePos);
    }
}
