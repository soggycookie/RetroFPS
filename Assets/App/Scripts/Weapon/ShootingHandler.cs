using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public abstract class ShootingHandler
{
    protected TriggerMode         m_shotTriggerMode;
    protected float               m_damageMultiplier;
    protected bool                m_canChargeEventSend = true;

    public    BulletManager       BulletManager     { get; protected set;}
    
    public    float               TotalChargeTime   { get; protected set;}
    public    float               CurrentChargeTime { get; protected set;} 

    public    UnityAction         OnShooting;
    public    UnityAction         OnCharging;

    public abstract bool HandleInput(PlayerInputHandler.MouseInput input);

    protected bool CheckShoot()
    {
        return BulletManager.CheckCanShoot();
    }

    public void ResetChargeTime(){
        CurrentChargeTime = 0f;
    }
}

public class StandardShootingHandler : ShootingHandler
{
    public StandardShootingHandler(BulletManager manager, TriggerMode mode, float totalChargeTime, float damageMultipler){
        
        BulletManager      = manager;
        m_shotTriggerMode  = mode;
        TotalChargeTime    = totalChargeTime;
        m_damageMultiplier = damageMultipler;

        if (m_shotTriggerMode == TriggerMode.CHARGING)
        {
            if (TotalChargeTime <= 0f || m_damageMultiplier <= 0f)
            {

                throw new ArgumentException("Must set totalChargeTime and damageMultipler when use 'CHARGE' mode ");
            }
        }
    }

    public override bool HandleInput(PlayerInputHandler.MouseInput input)
    {
        bool triggered = CheckShoot();

        if (!triggered)
            return false;

        switch (m_shotTriggerMode)
        {
            case TriggerMode.MANUAL:
                if(input.RmbHeld){
                    return false;
                }

                if (input.LmbPressed)
                {
                    CurrentChargeTime = 0f;
                    BulletManager.Shoot();

                    OnShooting?.Invoke();

                    return true;
                }
                break;
            case TriggerMode.AUTOMATIC:
                if (input.RmbHeld)
                {
                    return false;
                }

                if (input.LmbHeld || input.LmbPressed) {
                    CurrentChargeTime = 0f;

                    BulletManager.Shoot();

                    OnShooting?.Invoke();
                 
                    return true;
                }
                break;
            case TriggerMode.CHARGING:
                if (input.LmbHeld)
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

                if (input.LmbReleased)
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

    public SpecialShootingHandler(BulletManager manager, TriggerMode mode, float totalChargeTime, float damageMultipler)
    {
        BulletManager      = manager;
        m_shotTriggerMode  = mode;
        TotalChargeTime    = totalChargeTime;
        m_damageMultiplier = damageMultipler;

        if(m_shotTriggerMode == TriggerMode.CHARGING){
            if(TotalChargeTime <= 0f || m_damageMultiplier <= 0f){

                throw new ArgumentException("Must set totalChargeTime and damageMultipler when use 'CHARGE' mode ");
            }
        }
    }

    public override bool HandleInput(PlayerInputHandler.MouseInput input)
    {
        bool triggered = CheckShoot();

        if (!triggered)
            return false;

        switch (m_shotTriggerMode)
        {
            case TriggerMode.MANUAL:

                if (input.RmbPressed)
                {
                    CurrentChargeTime = 0;
                    OnShooting?.Invoke();
                    BulletManager.Shoot();
                    return true;
                }

                break;
            case TriggerMode.AUTOMATIC:
                if ( (input.RmbHeld || input.RmbPressed) && !input.LmbHeld){

                    CurrentChargeTime = 0;
                    
                    OnShooting?.Invoke();
                    BulletManager.Shoot();
                    return true; 
                }
                break;
            case TriggerMode.CHARGING:
                

                if(input.RmbHeld){
                    CurrentChargeTime += Time.deltaTime;

                    if(CurrentChargeTime >= TotalChargeTime){
                        CurrentChargeTime = TotalChargeTime;
                    }

                    if(m_canChargeEventSend){
                        OnCharging?.Invoke();

                        m_canChargeEventSend = false;
                    }
                }

                if( input.RmbReleased)
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
