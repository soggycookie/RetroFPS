using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class InputMediator : MonoBehaviour
{
    [SerializeField]
    private PlayerInputHandler m_inputHandler;

    [SerializeField]
    private PlayerMovementAbility m_movement;

    [SerializeField]
    private Gun[] m_gun;

    private void Update()
    {
        m_movement.HandleInput(m_inputHandler.GetKeyboardInput());

        foreach (Gun gun in m_gun){
            gun.HandleInput(m_inputHandler.GetMouseInput());
        }
    }

}
