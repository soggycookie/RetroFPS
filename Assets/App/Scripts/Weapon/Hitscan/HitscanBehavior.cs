using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;


public static class HitscanBehavior 
{

    public static (RaycastHit[], bool, RaycastHit) GetPiercingHitData(HitScanBulletSO hitscanData,Vector3 position, Vector3 direction)
    {
        RaycastHit[] hits;

        RaycastHit visualHit = new RaycastHit();
        bool isVisualRayHit = false;
        

        hits = Physics.SphereCastAll(position, hitscanData.radius, direction, hitscanData.maxDistance, hitscanData.blockLayer | hitscanData.damagableLayer, QueryTriggerInteraction.Collide)
            .OrderBy(hit => hit.distance).ToArray();

        isVisualRayHit = Physics.Raycast(position, direction, out visualHit, hitscanData.maxDistance, hitscanData.blockLayer | hitscanData.damagableLayer, QueryTriggerInteraction.Collide);

        
        return (hits,isVisualRayHit, visualHit);
    }    
    

    public static bool GetSingleHitData(HitScanBulletSO hitscanData, Vector3 position, Vector3 direction, out RaycastHit hit)
    {
        bool isHit = false;

        hit = new RaycastHit();


         isHit = Physics.Raycast(position, direction, out hit, hitscanData.maxDistance, hitscanData.blockLayer | hitscanData.damagableLayer, QueryTriggerInteraction.Collide);


        return isHit;
    }




}
