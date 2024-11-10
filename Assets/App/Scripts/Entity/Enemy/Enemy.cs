using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity, IDamagable
{
    [SerializeField] 
    private Health m_enemyHealth;

    private Rigidbody m_rigidbody;


    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    public void ApplyDamage(float healthLost)
    {
        m_enemyHealth.LoseHealth(0f);
        m_rigidbody.AddForce(Vector3.up * 20f, ForceMode.Impulse);
    }
}
