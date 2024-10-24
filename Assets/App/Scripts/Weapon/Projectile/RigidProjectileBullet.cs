using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public abstract class RigidProjectileBullet : ProjectileBullet
{
    protected Rigidbody m_rigidbody;
    protected Collider  m_collider;

    private void Awake()
    {
        base.Initialize();
        Initialize();
    }

    protected override void Initialize()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
    }


}
