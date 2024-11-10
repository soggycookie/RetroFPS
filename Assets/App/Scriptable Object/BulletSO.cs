using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletSO : ScriptableObject
{
    public enum ExecutionType{
        HITSCAN,
        PROJECTILE,
        EFFECT
    }
    
    [Header("VFX")]
    public BulletHitVFX wallHitParticle;

    public bool       usePooling;
    public int        defaultCapactiy;
    public int        maxCapactiy;

    [Space(10)]
    [Header("Config")]
    public LayerMask blockLayer;
    public LayerMask damagableLayer;

    public float bulletCoolDown;
    public float damagePerShot;
    public int   bulletsPerShot;

    public bool  explodeOnContact;
    public float explosionRadius;

    public abstract ExecutionType GetBulletType();

    
}
