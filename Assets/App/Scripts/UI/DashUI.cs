using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    [SerializeField]
    private Image      m_energyBarPrefab;
    
    [SerializeField]
    private Transform  m_eneryBarHolder;
    
    [SerializeField]
    private Image      m_backgroundBarPrefab;
   
    [SerializeField]
    private Transform  m_backgroundBarHolder;

    [SerializeField]
    private PlayerMovementAbility m_playerMovement;


    private int     m_maxDashCharge;
    private float   m_totalDashEnergy;
    private float   m_consumePerCharge;

    private Image[] m_energyBars;

    private void Awake()
    {

        m_maxDashCharge    = m_playerMovement.GetMaxDashCharge();
        m_totalDashEnergy  = m_playerMovement.GetTotalDashEnergy();

        m_consumePerCharge = m_totalDashEnergy / m_maxDashCharge;

        m_energyBars       = new Image[m_maxDashCharge];


        for (int i = 0; i < m_maxDashCharge; i++) {
            Instantiate(m_backgroundBarPrefab, m_backgroundBarHolder);
            
            m_energyBars[i] = Instantiate(m_energyBarPrefab, m_eneryBarHolder);
        }
    }

    private void Update()
    {
        for (int i = 0; i < m_maxDashCharge; i++)
        {
            float min =  i      * m_consumePerCharge;
            float max = (i + 1) * m_consumePerCharge;

            m_energyBars[i].fillAmount = Mathf.InverseLerp(min, max, m_playerMovement.CurrentDashEnergy);
        }
    }
}
