using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementAbility), typeof(PlayerCombatComponent))]
public class Player : Entity
{
    private PlayerMovementAbility m_playerMovementAbility;
    private PlayerCombatComponent m_playerCombatComponent;

    private void Awake()
    {
        m_playerCombatComponent = GetComponent<PlayerCombatComponent>();
        m_playerMovementAbility = GetComponent<PlayerMovementAbility>();
    }
}
