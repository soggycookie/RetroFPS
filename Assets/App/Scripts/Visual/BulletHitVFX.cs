using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletHitVFX : MonoBehaviour
{
    private ParticleSystem m_particleSystem;

    public  BulletManager BulletBehavior   { get; set; }

    private void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
    }


    private void Start()
    {
        //instatiate (awake and enable run before the call)
        //so the populated data can only be accessed after enable
    }

    public void PlayVFX(Vector3 position, Vector3 normal)
    {
        m_particleSystem.transform.position = position;
        m_particleSystem.transform.forward  = normal;

        m_particleSystem.Play();
    }


    private void OnDestroy()
    {
        //the behavior and this script get disable at the same time anyway
        //so no chance of the event get called when the script was disable                                              
    }

    //private void OnParticleSystemStopped()
    //{
    //    BulletBehavior.f_ParticleSystem.Pool.Release(this);
    //}


}
