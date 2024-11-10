using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementAbility), typeof(PlayerCombatComponent))]
public class Player : Entity, IDamagable
{
    private PlayerMovementAbility m_playerMovementAbility;
    private PlayerCombatComponent m_playerCombatComponent;
    private Health                m_health;

    public void ApplyDamage(float healthLost)
    {
        m_health.LoseHealth(healthLost);
    }

    private void Awake()
    {
        m_playerCombatComponent = GetComponent<PlayerCombatComponent>();
        m_playerMovementAbility = GetComponent<PlayerMovementAbility>();
        m_health                = GetComponent<Health>();
    }
}
