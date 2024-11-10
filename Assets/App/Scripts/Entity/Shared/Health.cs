using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Entity))]
public class Health : MonoBehaviour
{
    [SerializeField]
    private float m_totalHealth;

    [SerializeField]
    private bool  m_canHeal;

    [SerializeField]
    private float m_healSpeed;

    public UnityAction OnDeath;
    public UnityAction OnBeingHit;

    private float m_currentHealth;

    private void Update()
    {
        if(m_canHeal && m_currentHealth < m_totalHealth){
            m_currentHealth += Time.deltaTime * m_healSpeed;

            m_currentHealth = m_currentHealth > m_totalHealth ? m_totalHealth : m_currentHealth;
        }
    }

    public void Heal(){
        if(!m_canHeal) return;
        
        if(m_currentHealth >= m_totalHealth){
            m_currentHealth = m_totalHealth;
            
            return;
        }

        m_currentHealth += m_healSpeed * Time.deltaTime;
    }

    public void LoseHealth(float healthLoss){
        m_currentHealth -= healthLoss;

        if(m_currentHealth <= 0){

            Die();

            return;
        }

        OnBeingHit?.Invoke();
    }

    public void Die(){
        OnDeath?.Invoke();
    }


}
