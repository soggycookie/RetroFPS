using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Projectile")]
public class ProjectileBulletSO : BulletSO
{

    public float bulletSpreadOffset;
    public float bulletSpeed;
    

    [Tooltip("Life time in second")]
    public float lifeTime;

    [Space(10)]
    [Header("Collision")]
    public float collisionRadius;



    public override ExecutionType GetBulletType()
    {
        return ExecutionType.PROJECTILE;
    }



    #if UNITY_EDITOR

    private void OnValidate()
    {
        if(bulletCoolDown <= 0){
            bulletCoolDown = 0.5f;
        }

        if(bulletsPerShot <= 0){
            bulletsPerShot = 1;
        }

        if(bulletSpreadOffset < 0){
            bulletSpreadOffset = 0;
        }

        if(bulletSpeed < 0){
            bulletSpeed = 0;
        }

        if(lifeTime <= 0){
            lifeTime = 1;
        }

        if(collisionRadius <= 0){
            collisionRadius = 0.1f;
        }

    }

    #endif
}
