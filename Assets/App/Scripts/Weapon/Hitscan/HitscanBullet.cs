using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HitscanBulletVisual))]
public class HitscanBullet : Bullet
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

    protected static (RaycastHit[], bool, RaycastHit) GetPiercingHitData(HitScanBulletSO hitscanData, Vector3 position, Vector3 direction)
    {
        RaycastHit[] hits;

        RaycastHit visualHit = new RaycastHit();
        bool isVisualRayHit = false;

        if(hitscanData.hitHasRadius){

            hits = Physics.SphereCastAll(position, hitscanData.hitRadius, direction, hitscanData.maxDistance, hitscanData.blockLayer | hitscanData.damagableLayer, QueryTriggerInteraction.Collide)
                .OrderBy(hit => hit.distance).ToArray();

            isVisualRayHit = Physics.Raycast(position, direction, out visualHit, hitscanData.maxDistance, hitscanData.blockLayer | hitscanData.damagableLayer, QueryTriggerInteraction.Collide);
        }
        else
        {
            hits = Physics.RaycastAll(position, direction, hitscanData.maxDistance, hitscanData.blockLayer | hitscanData.damagableLayer, QueryTriggerInteraction.Collide)
                .OrderBy(hit => hit.distance).ToArray();

            isVisualRayHit = hits.Length != 0;
        }


        return (hits, isVisualRayHit, visualHit);
    }


    protected static bool GetSingleHitData(HitScanBulletSO hitscanData, Vector3 position, Vector3 direction, out RaycastHit hit)
    {
        bool isHit = false;

        hit = new RaycastHit();


        isHit = Physics.Raycast(position, direction, out hit, hitscanData.maxDistance, hitscanData.blockLayer | hitscanData.damagableLayer, QueryTriggerInteraction.Collide);


        return isHit;
    }


    public override void Shoot()
    {
        HitScanBulletSO hitscanData = m_bulletData as HitScanBulletSO;
        Vector3[] linePos;

        if(hitscanData.isPiercing){
            linePos = PiercingShoot(hitscanData);
        }else{
            linePos = StandardShoot(hitscanData);
        }

        OnShoot?.Invoke(linePos);
    }

    private Vector3[] PiercingShoot(HitScanBulletSO data)
    {
        var hitsAndVisualRay = GetPiercingHitData(data, Manager.FPSCamera.position, Manager.FPSCamera.forward);

        Vector3[] linePos;


        RaycastHit[] hits = hitsAndVisualRay.Item1;
        Vector3 blockedPoint = Vector3.zero;
        bool isBlocked = false;


        bool isVisualRayHit = hitsAndVisualRay.Item2;
        RaycastHit visualRay = hitsAndVisualRay.Item3;


        if (hits.Length == 0)
        {
            linePos = new Vector3[] { Manager.MuzzlePoint.position, Manager.FPSCamera.position + Manager.FPSCamera.forward * data.maxDistance };


            return linePos;
        }

        foreach (RaycastHit hit in hits)
        {
            var bitmaskLayer = 1 << hit.transform.gameObject.layer;

            if ((bitmaskLayer | data.blockLayer.value) == data.blockLayer.value)
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
                linePos = new Vector3[] { Manager.MuzzlePoint.position, visualRay.point, Manager.FPSCamera.position + Manager.FPSCamera.forward * data.maxDistance };
            }
            else
            {
                linePos = new Vector3[] { Manager.MuzzlePoint.position, Manager.FPSCamera.position + Manager.FPSCamera.forward * data.maxDistance };

            }
        }

        return linePos;
    }

    private Vector3[] StandardShoot(HitScanBulletSO data)
    {
        
        RaycastHit hit;
        bool isHit;

        Vector3[] linePos;

        isHit = GetSingleHitData(data, Manager.FPSCamera.position, Manager.FPSCamera.forward, out hit);

        if (!isHit)
        {
            linePos = new Vector3[] { Manager.MuzzlePoint.position, Manager.FPSCamera.position + Manager.FPSCamera.forward * data.maxDistance };

        }
        else
        {
            linePos = new Vector3[] { Manager.MuzzlePoint.position, hit.point };

            Explosion explosion = GlobalExplosionFactory.Instance.Pool.Get();

            explosion.Explode(hit.point, m_bulletData.damagableLayer);
        }

        return linePos;
    }
}
