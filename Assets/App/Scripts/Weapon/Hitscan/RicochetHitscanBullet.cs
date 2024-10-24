using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicochetHitscanBullet : HitscanBullet
{
    struct RichochetData
    {
        public Vector3 reflectDir;
        public Vector3 contactPoint;

        public RichochetData(Vector3 dir, Vector3 pos)
        {
            reflectDir = dir;
            contactPoint = pos;
        }
    }

    public override void Shoot()
    {



        //StartCoroutine(ShootRicochet(FPSCamera.position, FPSCamera.forward));

    }

    private void ShootPiercingAndOutRicochetData(Vector3 position, Vector3 direction, bool isCenterCamera, out RichochetData data)
    {
        data.reflectDir = Vector3.zero;
        data.contactPoint = Vector3.zero;

        HitScanBulletSO m_hitscanData = m_bulletData as HitScanBulletSO;
        Vector3[] linePos;

        var hitsAndVisualRay = HitscanBehavior.GetPiercingHitData(m_hitscanData, Manager.FPSCamera.position, Manager.FPSCamera.forward);


        RaycastHit[] hits = hitsAndVisualRay.Item1;
        Vector3 blockedPoint = Vector3.zero;
        bool isBlocked = false;


        bool isVisualRayHit = hitsAndVisualRay.Item2;
        RaycastHit visualRay = hitsAndVisualRay.Item3;




        if (hits.Length == 0 && isCenterCamera)
        {
            linePos = new Vector3[] { Manager.MuzzlePoint.position, Manager.FPSCamera.position + Manager.FPSCamera.forward * m_hitscanData.maxDistance };

            return;
        }

        foreach (RaycastHit hit in hits)
        {
            var bitmaskLayer = 1 << hit.transform.gameObject.layer;

            if ((bitmaskLayer | m_bulletData.blockLayer.value) == m_bulletData.blockLayer.value)
            {

                blockedPoint = hit.point;
                data.reflectDir = Vector3.Reflect(direction, hit.normal);
                data.contactPoint = blockedPoint;
                isBlocked = true;

                break;
            }

        }
        if (isBlocked)
        {
            if (isCenterCamera)
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
                linePos = new Vector3[] { position, blockedPoint };
            }
        }
        else
        {
            linePos = new Vector3[] { position, position + direction * m_hitscanData.maxDistance };
        }
    }

    //IEnumerator ShootRicochet(Vector3 position, Vector3 direction)
    //{
    //    RichochetData data = new RichochetData(Vector3.zero, Vector3.zero);

    //    for (int i = 0; i < HitscanData.maxRicochetTime; i++)
    //    {
    //        if (i == 0)
    //        {
    //            ShootPiercingAndOutRicochetData(position, direction, true, out data);
    //        }
    //        else
    //        {
    //            ShootPiercingAndOutRicochetData(data.contactPoint, data.reflectDir, false, out data);
    //        }
    //        if (data.contactPoint == Vector3.zero || data.reflectDir == Vector3.zero)
    //        {
    //            break;
    //        }

    //        yield return new WaitForSeconds(0.05f);
    //    }
    //}
}
