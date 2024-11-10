using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public bool IsGunDisable { get; set;}



    [SerializeField]
    private GunVisualHandler m_gunVisual;

    [SerializeField]
    private GunVariantSO       m_variantData;


    [SerializeField]
    private BulletManager      m_standardBulletManager;

    [SerializeField]
    private BulletManager      m_specialBulletManager;

    private float        m_switchDelayTime;
    private float        m_currentExitTime;
    private bool         m_canTrigger;
    private ChargingUI   m_chargingUI;
    

    private StandardShootingHandler m_standardShotHandler;
    private SpecialShootingHandler  m_specialShotHandler;

    private PlayerInputHandler.MouseInput m_input;  

    private void Awake()
    {
        m_chargingUI = GetComponentInChildren<ChargingUI>();

        InitializeShootingHandler();

        m_canTrigger = true;
    }
    
    private void InitializeShootingHandler(){

        m_standardShotHandler = new StandardShootingHandler(m_standardBulletManager ,m_variantData.standardTriggerMode, m_variantData.standardTotalChargingTime ,m_variantData.standardDamageMultiplier);

        m_specialShotHandler  = new SpecialShootingHandler( m_specialBulletManager , m_variantData.specialTriggetMode, m_variantData.specialTotalChargingTime, m_variantData.specialDamageMultiplier);

        m_standardShotHandler.OnShooting += m_gunVisual.PlayShootAnimation;
        m_specialShotHandler. OnShooting += m_gunVisual.PlayShootAnimation;

        m_standardShotHandler.OnCharging += m_gunVisual.PlayChargeAnimation;
        m_specialShotHandler. OnCharging += m_gunVisual.PlayChargeAnimation;

        m_standardShotHandler.OnShooting += () => { m_specialShotHandler.ResetChargeTime();};

        m_chargingUI.ShootingHandler = m_specialShotHandler;
    }

    private void Update()
    {
        if(IsGunDisable) return;

        Shoot();
        ResetShootingCoolDown();

    }

    public void HandleInput(PlayerInputHandler.MouseInput input)
    {
        m_input = input;
    }

    private void Shoot(){
        if(!m_canTrigger) return;


        if(m_standardShotHandler.HandleInput(m_input)){
            m_switchDelayTime = m_variantData.standardSwitchDelay;
            DisableShooting();
        }
        else if(m_specialShotHandler.HandleInput(m_input)){
            m_switchDelayTime = m_variantData.specialSwitchDelay;
            DisableShooting();
        }
    }


    private void ResetShootingCoolDown(){
        if(m_canTrigger){
            return;
        }
        m_currentExitTime += Time.deltaTime;

        if(m_currentExitTime >= m_switchDelayTime){
            m_currentExitTime = 0;
            m_canTrigger      = true;
        }
    }

    private void DisableShooting(){
        m_currentExitTime = 0;
        m_canTrigger      = false;

    }

    public void Swapped(){
        IsGunDisable = true;
        m_gunVisual.Disable();
    }

    public void SwapTo()
    {
        m_gunVisual.Enable();
    }


    private void OnValidate()
    {
        if(m_variantData){
            float duration = m_variantData.standardBulletPrefab.GetBulletData().bulletCoolDown;
            GunAnimation anim = GetComponentInChildren<GunAnimation>();
            anim.TotalDuration = duration;
        }
    }
}


public enum TriggerMode
{
    MANUAL,
    AUTOMATIC,
    CHARGING
}




