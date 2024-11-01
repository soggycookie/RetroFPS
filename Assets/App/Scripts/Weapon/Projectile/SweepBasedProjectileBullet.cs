using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class SweepBasedProjectileBullet : ProjectileBullet
{

    private Vector3 m_lastFramePosition;

    public UnityAction<RaycastHit> OnHit;


    private void Awake()
    {
        base.Initialize();
    }


    private void Update()
    {
        if (!m_isShot)
            return;

        Traverse();

        if (CheckHit(out RaycastHit hitObject))
        {
            if (hitObject.distance < 0)
            {
                hitObject.point = transform.position;
                hitObject.normal = -transform.forward;
            }

            //OnHit?.Invoke(hitObject);

            RefreshTokenSource(ref m_cancellationTokenSource);

            CancelBullet();
        }


        m_lastFramePosition = transform.position;
    }


    bool CheckHit(out RaycastHit hit)
    {
        Vector3 directionVector = (transform.position - m_lastFramePosition);
        float distance = directionVector.magnitude;

        RaycastHit[] hits = Physics.SphereCastAll(m_lastFramePosition, m_projectileData.collisionRadius, directionVector.normalized, distance,
            m_projectileData.blockLayer | m_projectileData.damagableLayer, QueryTriggerInteraction.Collide)
            .OrderBy(hit => hit.distance).ToArray();

        hit = new RaycastHit();

        if (hits.Length == 0) return false;

        hit = hits[0];

        return true;
    }

    void Traverse()
    {
        transform.position += m_traverseDirection * Time.deltaTime * m_projectileData.bulletSpeed;

    }

    public override void Shoot()
    {
        m_isShot = true;

        SetShootingBullettParams();

        m_lastFramePosition = transform.position;

        DisableBulletAfterDelayAsync(m_cancellationTokenSource.Token);

        
    }



}
