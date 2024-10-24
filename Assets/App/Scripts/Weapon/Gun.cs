using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{

    enum ShotCategory
    {
        STANDARD,
        SPECIAL
    }

    public bool IsGunDisable { get; set;}


    [Header("Gun Settings")]
    [SerializeField]
    private Transform          m_muzzlePoint;

    [SerializeField]
    private PlayerInputHandler m_playerInputHandler;

    [SerializeField]
    private GameObject         m_visualObject;

    //[SerializeField]
    //private float swapTime = 0.5f;

    [Space(10)]
    [Header("Standard")]

    [SerializeField]
    private TriggerMode  m_standardTriggerMode;
                         

    [SerializeField]
    private Bullet       m_standardBulletPrefab;

    [SerializeField]     
    private int          m_standardDefaultCapacity;
                         
    [SerializeField]     
    private int          m_standardMaxCapacity;
                         
    [SerializeField]     
    private bool         m_standardHasCollectionCheck;
                         
    [SerializeField]     
    private float        m_standardSwitchDelay;

    [Space(5)]
    [Header("Standard Charging Settings")]
    [SerializeField]
    private float        m_standardDamageMultiplier;

    [SerializeField]
    private float        m_standardTotalChargingTime;



    [Space(10)]
    [Header("Special")]

    [SerializeField]
    private TriggerMode  m_specialTriggetMode;

    
    [SerializeField]
    private Bullet       m_specialBulletPrefab;
    
    [SerializeField]
    private int          m_specialDefaultCapacity;

    [SerializeField]
    private int          m_specialMaxCapacity;

    [SerializeField]
    private bool         m_specialHasCollectionCheck;

    [SerializeField]
    private float        m_specialSwitchDelay;

    [Space(5)]
    [Header("Special Charging Settings")]
    [SerializeField]
    private float        m_specialDamageMultiplier;

    [SerializeField]
    private float        m_specialTotalChargingTime;


    [Space(10)]
    [SerializeField]
    private bool         m_specialDependOnStandardInput;


    private float        m_switchDelayTime;
    private float        m_currentExitTime;
    private bool         m_canTrigger;
    private GunInput     m_gunInput;
    private GunVisual    m_visual;
    private ChargingUI   m_chargingUI;


    private StandardShootingHandler m_standardShotHandler;
    private SpecialShootingHandler  m_specialShotHandler;


    private void Awake()
    {
        m_visual = GetComponentInChildren<GunVisual>();
        m_chargingUI = GetComponentInChildren<ChargingUI>();


        if (m_playerInputHandler == null)
            m_playerInputHandler = FindAnyObjectByType<PlayerInputHandler>();

        m_standardShotHandler = new StandardShootingHandler(this, m_standardBulletPrefab, m_muzzlePoint, m_standardTriggerMode,
            m_standardHasCollectionCheck, m_standardDefaultCapacity, m_standardMaxCapacity, 
            m_standardTotalChargingTime, m_standardDamageMultiplier);

        m_specialShotHandler  = new SpecialShootingHandler(this, m_specialBulletPrefab, m_muzzlePoint, m_specialTriggetMode, 
            m_specialHasCollectionCheck, m_specialDefaultCapacity, m_specialMaxCapacity ,
            m_specialTotalChargingTime, m_specialDamageMultiplier);

        //m_standardShotHandler.manualShot += m_visual.Play ;
        //m_specialShotHandler.manualShot += m_visual.Play;
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
            m_switchDelayTime = m_standardSwitchDelay;
            DisableShooting();
        }
        else if(m_specialShotHandler.HandleInput(m_gunInput)){
            m_switchDelayTime = m_specialSwitchDelay;
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




