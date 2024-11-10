using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Gun[] m_guns;

    [SerializeField]
    private PlayerInputHandler m_playerInputHandler;

    private int m_currentIndex = 0;

    private void Awake()
    {
        m_playerInputHandler = FindAnyObjectByType<PlayerInputHandler>();
        for (int i = 0; i < m_guns.Length; i++)
        {
            if (i == 0)
            {
                m_guns[i].EnableGunVisual();
            }
            else
            {
                m_guns[i].DisableGunVisual();
            }
        }
    }


    private void Update()
    {
        SwapGuns();
    }

    void SwapGuns(){
        if (m_playerInputHandler.GetQPress())
        {
            m_guns[m_currentIndex].DisableGunVisual();
            m_currentIndex++;

            if (m_currentIndex >= m_guns.Length)
            {
                m_currentIndex = 0;
            }

            m_guns[m_currentIndex].EnableGunVisual();
        }
    }


}
