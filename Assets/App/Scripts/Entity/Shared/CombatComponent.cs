using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health), typeof(Entity))]
public abstract class CombatComponent : MonoBehaviour
{
    private Health m_health;

    protected abstract void Die();

    private void Awake()
    {
        m_health = GetComponent<Health>();
        m_health.OnDeath += Die;
    }



}
