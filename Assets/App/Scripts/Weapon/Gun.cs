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


    [Header("Gun Settings")]
    [SerializeField]
    private Transform          m_muzzlePoint;
    [SerializeField]
    private Transform          m_cameraTransform;

    [SerializeField]
    private PlayerInputHandler m_playerInputHandler;

    [SerializeField]
    private GameObject         m_visualObject;

    [SerializeField]
    private GunVariantSO       m_variantData;

    private float        m_switchDelayTime;
    private float        m_currentExitTime;
    private bool         m_canTrigger;
    private GunInput     m_gunInput;
    private ChargingUI   m_chargingUI;
    private GunVisual    m_gunVisual;

    private StandardShootingHandler m_standardShotHandler;
    private SpecialShootingHandler  m_specialShotHandler;


    private void Awake()
    {
        m_chargingUI = GetComponentInChildren<ChargingUI>();
        m_gunVisual  = GetComponentInChildren<GunVisual>();
        FPSCamera cam = m_cameraTransform.GetComponent<FPSCamera>();

        if (m_playerInputHandler == null)
            m_playerInputHandler = FindAnyObjectByType<PlayerInputHandler>();

        m_standardShotHandler = new StandardShootingHandler(this, m_variantData.standardBulletPrefab,m_cameraTransform, m_muzzlePoint, m_variantData.standardTriggerMode,
            true, m_variantData.standardDefaultCapacity, m_variantData.standardMaxCapacity,
            m_variantData.standardTotalChargingTime, m_variantData.standardDamageMultiplier);

        m_specialShotHandler  = new SpecialShootingHandler(this, m_variantData.specialBulletPrefab, m_cameraTransform, m_muzzlePoint, m_variantData.specialTriggetMode,
            true, m_variantData.specialDefaultCapacity, m_variantData.specialMaxCapacity ,
            m_variantData.specialTotalChargingTime, m_variantData.specialDamageMultiplier);

        m_standardShotHandler.OnShooting += m_gunVisual.OnShooting;
        m_specialShotHandler. OnShooting += m_gunVisual.OnShooting;

        m_specialShotHandler. OnShooting += cam.CameraShake;
        m_standardShotHandler.OnShooting += cam.CameraShake;

        m_standardShotHandler.OnCharging += m_gunVisual.OnCharging;
        m_specialShotHandler. OnCharging += m_gunVisual.OnCharging;
        m_canTrigger = true;

        m_chargingUI.ShootingHandler = m_specialShotHandler;
    }

    private void Update()
    {
        if(IsGunDisable) return;

        HandleGunInput();
        HandleBehaviorExecution();

        ResetShooting();

    }

    void HandleGunInput()
    {
        m_gunInput.lmbPressed  = m_playerInputHandler.GetLMBPressed();
        m_gunInput.lmbHeld     = m_playerInputHandler.GetLMBHeld();
        m_gunInput.lmbReleased = m_playerInputHandler.GetLMBReleased();
        m_gunInput.rmbPressed  = m_playerInputHandler.GetRMBPressed();
        m_gunInput.rmbHeld     = m_playerInputHandler.GetRMBHeld();
        m_gunInput.rmbReleased = m_playerInputHandler.GetRMBReleased();
    }

    void HandleBehaviorExecution(){
        if(!m_canTrigger) return;


        if(m_standardShotHandler.HandleInput(m_gunInput)){
            m_switchDelayTime = m_variantData.standardSwitchDelay;
            DisableShooting();
        }
        else if(m_specialShotHandler.HandleInput(m_gunInput)){
            m_switchDelayTime = m_variantData.specialSwitchDelay;
            DisableShooting();
        }
    }


    void ResetShooting(){
        if(m_canTrigger){
            return;
        }
        m_currentExitTime += Time.deltaTime;

        if(m_currentExitTime >= m_switchDelayTime){
            m_currentExitTime = 0;
            m_canTrigger      = true;
        }
    }

    void DisableShooting(){
        m_currentExitTime = 0;
        m_canTrigger      = false;

    }

    public void DisableVisual(){
        IsGunDisable = true;
        m_visualObject.SetActive(false);
    }

    public void EnableVisual()
    {
        IsGunDisable = false;
        m_visualObject.SetActive(true);
    }
}

public struct GunInput
{
    public bool lmbPressed;
    public bool lmbHeld;
    public bool lmbReleased;
    public bool rmbPressed;
    public bool rmbHeld;
    public bool rmbReleased;
}

public enum TriggerMode
{
    MANUAL,
    AUTOMATIC,
    CHARGING
}




