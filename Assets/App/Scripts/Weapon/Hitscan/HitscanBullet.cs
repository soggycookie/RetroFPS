using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HitscanBulletVisual))]
public class HitscanBullet : Bullet
{

    public UnityAction<Vector3[]>           OnShoot;

    public UnityAction<Vector3, Vector3>    OnBlocked;
    
    [SerializeField]
    private HitscanBulletVisual m_hitscanBulletVisual;

    private HitScanBulletSO m_hitscanData;

    private void Awake()
    {

        base.Initialize();
        Initialize();

    }


    protected override void Initialize() {
        m_hitscanBulletVisual = GetComponent<HitscanBulletVisual>();

        if (m_bulletData is not HitScanBulletSO)
        {
            Debug.LogError("Bullet data is not hitscan data!");
        }

        m_hitscanBulletVisual.OnFinishFadeOut += () => {
            Manager.GetBulletFactory().Pool.Release(this);
        };

        m_hitscanData = m_bulletData as HitScanBulletSO;
    }

    public override void Shoot()
    {
        Transform cam = Manager.GetCameraTransform();
        RicochetData data;


        data = ManageShooting(cam.position, cam.forward);
        
        if(m_hitscanData.isRicochet)
        {
            RicochetHandler handler = new RicochetHandler(this, data);
            handler.Fire();
        }
    }

    private void GetHitData(Vector3 pos, Vector3 dir, out HitData data){
        
        RaycastHit[] hits;
        RaycastHit blockedHit;
        bool isBlocked;

        
        if(m_hitscanData.isPiercing){
            isBlocked = Physics.Raycast(pos, dir, out blockedHit, m_hitscanData.maxDistance, m_bulletData.blockLayer);
            
            Ray ray = new Ray(pos, dir);
            if(isBlocked)
            {

                hits = Physics.SphereCastAll(ray, m_hitscanData.hitRadius, (pos - blockedHit.point).magnitude, m_bulletData.damagableLayer, QueryTriggerInteraction.Collide);

            }else
            {
                hits = Physics.SphereCastAll(ray, m_hitscanData.hitRadius, m_hitscanData.maxDistance, m_bulletData.damagableLayer, QueryTriggerInteraction.Collide);
            }

        }
        else
        {
            isBlocked = Physics.Raycast(pos, dir, out blockedHit, m_hitscanData.maxDistance, m_bulletData.blockLayer | m_bulletData.damagableLayer);

            if(  (1 << blockedHit.transform.gameObject.layer | m_bulletData.damagableLayer) == m_bulletData.damagableLayer){
                hits = new RaycastHit[] { blockedHit};
            }else{
                hits = new RaycastHit[0];
            }
        }
        
        data = new HitData(isBlocked, blockedHit, hits);
    }

     RicochetData ManageShooting(Vector3 pos, Vector3 dir, bool firstRay = true){
        HitData hitData;
        RicochetData ricochetData = new RicochetData(Vector3.zero, Vector3.zero);
        Vector3[] linePos;

        GetHitData(pos, dir, out hitData);

        //damage
        InflictDamage(hitData.damageHits);

        //visual
        if (firstRay)
        {
            Vector3 muzzlePoint = Manager.GetMuzzlePoint();
            if (hitData.isBlocked)
            {
                linePos = ProcessVisualLinePos(muzzlePoint, hitData.blockedHit.point);
            }
            else
            {
                linePos = ProcessVisualLinePos(muzzlePoint, dir, m_hitscanData.maxDistance);
            }
        }
        else
        {
            if (hitData.isBlocked)
            {
                linePos = ProcessVisualLinePos(pos, hitData.blockedHit.point);
            }
            else
            {
                linePos = ProcessVisualLinePos(pos, dir, m_hitscanData.maxDistance);
            }
        }
        
        OnShoot?.Invoke(linePos);

        if(hitData.isBlocked){
            //particle
            Manager.GetParticleFactory().Pool.Get().PlayVFX(hitData.blockedHit.point, hitData.blockedHit.normal);

            //ricochet
            ricochetData.reflectPos = hitData.blockedHit.point;
            ricochetData.reflectDir = Vector3.Reflect(dir, hitData.blockedHit.normal);
        }

        return ricochetData;
    }

    private Vector3[] ProcessVisualLinePos(Vector3 originalPos, Vector3 blockPoint){
        Vector3[] linePos;
        
        linePos = new Vector3[] { originalPos, blockPoint };

        return linePos;
    }

    private Vector3[] ProcessVisualLinePos(Vector3 originalPos, Vector3 originalDir, float maxDist)
    {
        Vector3[] linePos;
        
        linePos = new Vector3[] { originalPos, originalPos + originalDir * maxDist };

        return linePos;
    }

    struct RicochetData{
        public Vector3 reflectPos; 
        public Vector3 reflectDir;

        public RicochetData(Vector3 pos, Vector3 dir){
            reflectPos = pos;
            reflectDir = dir;
        }
    }

    struct HitData{
        public bool         isBlocked;
        public RaycastHit   blockedHit;
        public RaycastHit[] damageHits;

        public HitData(bool blocked, RaycastHit blockHit, RaycastHit[] damage){
            isBlocked  = blocked;
            blockedHit = blockHit;
            damageHits = damage;
        }
    }

    class RicochetHandler{
        
        private HitscanBullet m_bullet;
        private RicochetData  m_data;

        public RicochetHandler(HitscanBullet bullet, RicochetData initialData)
        {
            m_bullet = bullet;
            m_data = initialData;
        }

        public void Fire(){


            for (int i = 1; i < m_bullet.m_hitscanData.maxRicochetTime; i++)
            {
                HitscanBullet newBullet = m_bullet.Manager.GetBulletFactory().Pool.Get() as HitscanBullet;

                if (m_data.reflectDir == Vector3.zero)
                {
                    break;
                }

                m_data = newBullet.ManageShooting(m_data.reflectPos, m_data.reflectDir, false);

            }
        }
    }
}


