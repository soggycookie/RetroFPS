using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Entity))]
public class Health : MonoBehaviour, IDamagable
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
    


    public void Heal(){
        if(!m_canHeal) return;
        
        if(m_currentHealth >= m_totalHealth){
            m_currentHealth = m_totalHealth;
            
            return;
        }

        m_currentHealth += m_healSpeed * Time.deltaTime;
    }

    private void LoseHealth(float healthLoss){
        m_currentHealth -= healthLoss;

        if(m_currentHealth <= 0){
            Die();
        }

    }

    public void Die(){
        OnDeath?.Invoke();
    }

    public virtual void ApplyDamage(float healthLost, Vector3 forceDir, float force)
    {
        LoseHealth(healthLost);
        OnBeingHit?.Invoke();
    }
}
