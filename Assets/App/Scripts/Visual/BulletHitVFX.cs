using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletHitVFX : MonoBehaviour
{
    private ParticleSystem m_particleSystem;

    public ObstacleHitFactory Factory { get; set;}

    private void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
    }



    public void PlayVFX(Vector3 position, Vector3 normal)
    {
        m_particleSystem.transform.position = position;
        m_particleSystem.transform.forward  = normal;

        m_particleSystem.Play();
    }



    private void OnParticleSystemStopped()
    {
        Factory.Pool.Release(this);
    }


}
