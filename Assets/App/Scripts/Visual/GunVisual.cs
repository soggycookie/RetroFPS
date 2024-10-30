using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GunVisual : MonoBehaviour
{
    [SerializeField]
    protected Transform m_visualTransform;


    protected Transform m_defaultRootTransform;
    protected bool m_isCharging;
    protected bool m_isShooting;

    private void Awake()
    {
        m_defaultRootTransform = m_visualTransform;
    }

    public abstract void OnShooting();
    public abstract void OnCharging();

}
