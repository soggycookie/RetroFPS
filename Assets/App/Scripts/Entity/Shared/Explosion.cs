using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ExplosionVisual))]
public class Explosion : MonoBehaviour{

    [SerializeField]
    private ExplosionVisual m_explosionVisual;

    private void Awake()
    {
        m_explosionVisual = GetComponent<ExplosionVisual>();
    }

    public void Explode(Vector3 position, float radius, LayerMask mask){
        Collider[] colliderHit = GetEntityInRadius(position,radius,mask);

        foreach (Collider collider in colliderHit){
            if(collider.gameObject.TryGetComponent<IDamagable>( out IDamagable damagable)){
                damagable.ApplyDamage(0f);
            }
        }

        m_explosionVisual.Explode(radius, position);
    }

    public static Collider[] GetEntityInRadius(Vector3 position, float radius, LayerMask mask){
        Collider[] result = Physics.OverlapSphere(position, radius, mask, QueryTriggerInteraction.Collide);

        return result;
    }

    
}
