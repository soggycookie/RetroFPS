using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargingUI : MonoBehaviour
{


    public SpecialShootingHandler ShootingHandler { get; set; }

    [SerializeField]
    private int chargeStep = 5;

    [SerializeField]
    private Image m_chargeTimeImage;

    [SerializeField]
    private Image m_holdChargeTImeImage;

    private float m_chargeStepAmount;

    private void Awake()
    {
        m_chargeStepAmount = 1f / chargeStep;
    }

    private void Update()
    {
        UpdateCoolDownTime();
        UpdateHoldChargeTime();
    }

    void UpdateCoolDownTime(){

        float fillAmount =  Mathf.InverseLerp(0, ShootingHandler.BulletManager.BulletCoolDown, ShootingHandler.BulletManager.CurrentCoolDownTime);

        fillAmount = fillAmount == 0 ? 1 : fillAmount;

        m_chargeTimeImage.fillAmount = fillAmount;
    }

    void UpdateHoldChargeTime(){

        float fillAmount = Mathf.InverseLerp(0, ShootingHandler.TotalChargeTime,ShootingHandler.CurrentChargeTime);

        fillAmount = Mathf.Floor(fillAmount / m_chargeStepAmount) * m_chargeStepAmount;

        m_holdChargeTImeImage.fillAmount = fillAmount;
    }
}
