using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public abstract class ShootingHandler
{
    protected Gun                 m_gunController;
    protected TriggerMode         m_shotTriggerMode;
    protected Transform           m_muzzlePoint;
    protected Transform           m_cameraTransform;
    protected Bullet              m_bullet;
    protected float               m_damageMultiplier;
    protected bool                m_canChargeEventSend = true;

    public    BulletManager       BulletManager     { get; protected set;}
    
    public    float               TotalChargeTime   { get; protected set;}
    public    float               CurrentChargeTime { get; protected set;} 

    private BulletSO.ExecutionType m_bulletType;


    public    UnityAction         OnShooting;
    public    UnityAction         OnCharging;

    public abstract bool HandleInput(GunInput input);

    protected BulletManager AddBulletBehaviorComponent(bool hasCollectionCheck , int defaultCapacity, int maxCapacity)
    {
        BulletManager manager = m_gunController.gameObject.AddComponent<BulletManager>();
        manager.FPSCamera     = m_cameraTransform;
        manager.MuzzlePoint   = m_muzzlePoint;
        manager.BulletPrefab  = m_bullet;
        manager.BulletData    = m_bullet.GetBulletData();
        manager.InitializePoolParameters(hasCollectionCheck, defaultCapacity, maxCapacity);

        return manager;
    }

    public void InitializeShotHandler(Gun controller, Bullet bullet, Transform camera, Transform muzzlePoint, TriggerMode mode,
        bool hasCollectionCheck, int defaultCapacity , int maxCapacity, float totalChargeTime, float damageMultiplier)
    {

        m_gunController    = controller;
        m_muzzlePoint      = muzzlePoint;
        m_shotTriggerMode  = mode;
        m_cameraTransform  = camera;

        m_damageMultiplier = damageMultiplier;
        TotalChargeTime    = totalChargeTime;

        m_bullet           = bullet;

        BulletManager = AddBulletBehaviorComponent( hasCollectionCheck, defaultCapacity, maxCapacity);
    }

    protected bool CheckShoot()
    {
        return BulletManager.CheckCanShoot();
    }

}

public class StandardShootingHandler : ShootingHandler
{
    public StandardShootingHandler(Gun controller, Bullet bullet, Transform camera, Transform muzzlePoint, TriggerMode mode,
        bool hasCollectionCheck , int defaultCapacity , int maxCapacity, float totalChargeTime, float damageMultipler ){

        InitializeShotHandler(controller, bullet, camera, muzzlePoint, mode, hasCollectionCheck, defaultCapacity, maxCapacity, totalChargeTime, damageMultipler);

        if (m_shotTriggerMode == TriggerMode.CHARGING)
        {
            if (TotalChargeTime <= 0f || m_damageMultiplier <= 0f)
            {

                throw new ArgumentException("Must set totalChargeTime and damageMultipler when use 'CHARGE' mode ");
            }
        }
    }

    public override bool HandleInput(GunInput input)
    {
        bool triggered = CheckShoot();

        if (!triggered)
            return false;

        switch (m_shotTriggerMode)
        {
            case TriggerMode.MANUAL:
                if (input.lmbPressed)
                {
                    BulletManager.Shoot();

                    OnShooting?.Invoke();

                    return true;
                }
                break;
            case TriggerMode.AUTOMATIC:
                if (input.lmbHeld || input.lmbPressed) {

                    BulletManager.Shoot();

                    OnShooting?.Invoke();
                 
                    return true;
                }
                break;
            case TriggerMode.CHARGING:
                if (input.lmbHeld)
                {
                    CurrentChargeTime += Time.deltaTime;

                    if (CurrentChargeTime >= TotalChargeTime)
                    {
                        CurrentChargeTime = TotalChargeTime;
                    }
                    if(m_canChargeEventSend){
                        OnCharging?.Invoke();
                        m_canChargeEventSend = false;
                    }
                }

                if (input.lmbReleased)
                {
                    CurrentChargeTime = 0;

                    BulletManager.Shoot();

                    OnShooting?.Invoke();

                    m_canChargeEventSend = true;

                    return true;
                }

                break;
        }

        return false;
    }
}

public class SpecialShootingHandler : ShootingHandler
{

    public SpecialShootingHandler(Gun controller, Bullet bullet, Transform camera, Transform muzzlePoint, TriggerMode mode,
        bool hasCollectionCheck, int defaultCapacity, int maxCapacity, 
         float totalChargeTime, float damageMultipler)
    {
        InitializeShotHandler(controller, bullet, camera, muzzlePoint, mode, hasCollectionCheck, defaultCapacity, maxCapacity, totalChargeTime, damageMultipler);

        if(m_shotTriggerMode == TriggerMode.CHARGING){
            if(TotalChargeTime <= 0f || m_damageMultiplier <= 0f){

                throw new ArgumentException("Must set totalChargeTime and damageMultipler when use 'CHARGE' mode ");
            }
        }


    }

    public override bool HandleInput(GunInput input)
    {
        bool triggered = CheckShoot();

        if (!triggered)
            return false;

        switch (m_shotTriggerMode)
        {
            case TriggerMode.MANUAL:
                if (input.rmbPressed)
                {
                    OnShooting?.Invoke();
                    BulletManager.Shoot();
                    return true;
                }

                break;
            case TriggerMode.AUTOMATIC:
                if ( (input.rmbHeld || input.rmbPressed) && !input.lmbHeld){
                    OnShooting?.Invoke();
                    BulletManager.Shoot();
                    return true; 
                }
                break;
            case TriggerMode.CHARGING:
                if(input.rmbHeld && !input.lmbHeld){
                    CurrentChargeTime += Time.deltaTime;

                    if(CurrentChargeTime >= TotalChargeTime){
                        CurrentChargeTime = TotalChargeTime;
                    }

                    if(m_canChargeEventSend){
                        OnCharging?.Invoke();

                        m_canChargeEventSend = false;
                    }
                }

                if(input.rmbReleased && !input.lmbHeld){
                    CurrentChargeTime = 0;

                    BulletManager.Shoot();

                    OnShooting?.Invoke();
                    m_canChargeEventSend = true;

                    return true;
                }

                break;
        }

        return false;
    }
}
