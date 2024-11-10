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


    [SerializeField]
    private BulletManager      m_standardBulletManager;

    [SerializeField]
    private BulletManager      m_specialBulletManager;

    private float        m_switchDelayTime;
    private float        m_currentExitTime;
    private bool         m_canTrigger;
    private GunInput     m_gunInput;
    private ChargingUI   m_chargingUI;
    private GunAnimation    m_gunAnimation;

    private StandardShootingHandler m_standardShotHandler;
    private SpecialShootingHandler  m_specialShotHandler;
  

    private void Awake()
    {
        m_chargingUI    = GetComponentInChildren<ChargingUI>();
        m_gunAnimation  = GetComponentInChildren<GunAnimation>();
        
        PlayerCameraController cam = m_cameraTransform.GetComponent<PlayerCameraController>();

        if (m_playerInputHandler == null)
            m_playerInputHandler = FindAnyObjectByType<PlayerInputHandler>();

        InitializeShootingHandler();

        m_canTrigger = true;
    
        m_gunAnimation.OnFinshSwap += () => { IsGunDisable = false; };    
        
    }
    
    private void InitializeShootingHandler(){

        m_standardShotHandler = new StandardShootingHandler(m_standardBulletManager ,m_variantData.standardTriggerMode, m_variantData.standardTotalChargingTime ,m_variantData.standardDamageMultiplier);

        m_specialShotHandler  = new SpecialShootingHandler( m_specialBulletManager , m_variantData.specialTriggetMode, m_variantData.specialTotalChargingTime, m_variantData.specialDamageMultiplier);

        m_standardShotHandler.OnShooting += m_gunAnimation.Shoot;
        m_specialShotHandler. OnShooting += m_gunAnimation.Shoot;

        m_standardShotHandler.OnCharging += m_gunAnimation.Charge;
        m_specialShotHandler. OnCharging += m_gunAnimation.Charge;

        m_chargingUI.ShootingHandler = m_specialShotHandler;
    }

    private void Start()
    {
        m_gunAnimation.MoveToSwapPos();
    }

    private void Update()
    {
        if(IsGunDisable) return;

        HandleGunInput();
        HandleBehaviorExecution();
        ResetShootingCoolDown();

    }

    private void HandleGunInput()
    {
        m_gunInput.lmbPressed  = m_playerInputHandler.GetLMBPressed();
        m_gunInput.lmbHeld     = m_playerInputHandler.GetLMBHeld();
        m_gunInput.lmbReleased = m_playerInputHandler.GetLMBReleased();
        m_gunInput.rmbPressed  = m_playerInputHandler.GetRMBPressed();
        m_gunInput.rmbHeld     = m_playerInputHandler.GetRMBHeld();
        m_gunInput.rmbReleased = m_playerInputHandler.GetRMBReleased();
    }

    private void HandleBehaviorExecution(){
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

    public void DisableGunVisual(){
        IsGunDisable = true;
        m_visualObject.SetActive(false);
        
        m_gunAnimation.CancelAll();
        m_gunAnimation.MoveToSwapPos();
    }

    public void EnableGunVisual()
    {
        IsGunDisable = false;
        m_visualObject.SetActive(true);
        
        IsGunDisable = true;
        Swap();
    }

    public void Swap(){
        m_gunAnimation.Swap();
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




