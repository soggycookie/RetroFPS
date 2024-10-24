using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/HitScan")]
public class HitScanBulletSO : BulletSO
{

    public float maxDistance;

    public bool  isPiercing;
    public bool  isRicochet;
    public int   maxRicochetTime;

    public bool  hasRadius;
    public float radius;


    public override ExecutionType GetBulletType()
    {
        return ExecutionType.HITSCAN;
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (bulletCoolDown <= 0)
        {
            bulletCoolDown = 0.5f;
        }

        if(maxDistance <=0){
            maxDistance = 1f;
        }

        if(maxRicochetTime < 0){
            maxRicochetTime = 0;
        }


    }

#endif
}
