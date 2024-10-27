using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ExplosionVisual))]
public class Explosion : MonoBehaviour
{

    [SerializeField]
    private ExplosionVisual m_explosionVisual;

    [SerializeField]
    private float m_radius;

    private void Awake()
    {
        m_explosionVisual = GetComponent<ExplosionVisual>();
    }

    public void Explode(Vector3 position, LayerMask mask)
    {
        transform.position = position;


        Collider[] colliderHit = GetEntityInRadius(position, m_radius, mask);

        foreach (Collider collider in colliderHit)
        {
            if (collider.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
            {
                damagable.ApplyDamage(0f);


                if (collider.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.velocity = Vector3.zero;
                    rb.AddExplosionForce(40f, transform.position, 0, 3f, ForceMode.Impulse);
                }


            }




        }

        m_explosionVisual.Explode(m_radius, position);

    }

    public static Collider[] GetEntityInRadius(Vector3 position, float radius, LayerMask mask)
    {
        Collider[] result = Physics.OverlapSphere(position, radius, mask | 1 << 9, QueryTriggerInteraction.Collide);

        return result;
    }


}
