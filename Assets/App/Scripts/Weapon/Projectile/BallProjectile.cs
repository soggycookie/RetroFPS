using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BallProjectile : RigidProjectileBullet
{
    private bool m_isTriggered;


    public override void Shoot()
    {


        m_isShot = true;
        m_rigidbody.velocity = Vector3.zero;


        SetShootingBullettParams();
        Debug.Log(transform.position);

        DisableBulletAfterDelayAsync(m_cancellationTokenSource.Token);
    }

    private void FixedUpdate()
    {
        if(!m_isShot) return;
        Debug.Log("update:" + transform.position);
        m_rigidbody.AddForce(m_traverseDirection * m_projectileData.bulletSpeed, ForceMode.Force);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(!m_isShot) return ;

        if (other.gameObject.layer == 3)
        {
            Debug.Log(other.gameObject.name);
            m_isShot = false;

            RefreshTokenSource(ref m_cancellationTokenSource);
            CancelBullet();
        }
    }

}
