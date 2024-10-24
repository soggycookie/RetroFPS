using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public abstract class ProjectileBullet : Bullet
{
    protected ProjectileBulletSO m_projectileData;
    protected Transform m_parent;
    protected CancellationTokenSource m_cancellationTokenSource;
    protected bool m_isShot;
    protected Vector3 m_traverseDirection;


    private void Awake()
    {
        Initialize();
    }


    protected virtual void Initialize(){
        m_projectileData = m_bulletData as ProjectileBulletSO;
        m_parent = transform.parent;
        m_cancellationTokenSource = new CancellationTokenSource();

    }

    protected void RefreshTokenSource(ref CancellationTokenSource tokenSource)
    {
        tokenSource?.Cancel();
        tokenSource?.Dispose();

        tokenSource = new CancellationTokenSource();
    }

    protected async void DisableBulletAfterDelayAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Awaitable.WaitForSecondsAsync(m_projectileData.lifeTime, cancellationToken);

        CancelBullet();

    }

    public void CancelBullet()
    {
        //transform.parent = m_parent;
        Manager.f_bullet.Pool.Release(this);
        m_isShot = false;

        transform.position = Manager.FPSCamera.position;
    }

    Vector3 CalculateDirection()
    {
        if (m_projectileData.bulletSpreadOffset == 0)
        {
            return Manager.FPSCamera.forward;
        }

        Vector2 random = MathUtils.RandomInsideCircleWithBias(3f, m_projectileData.bulletSpreadOffset);

        return (Manager.FPSCamera.forward + random.x * Manager.FPSCamera.right + random.y * Manager.FPSCamera.up).normalized;
    }

    protected void SetShootingBullettParams()
    {
        //transform.parent    = null;

        m_traverseDirection  = CalculateDirection();
        transform.position   = Manager.FPSCamera.position + m_traverseDirection * m_projectileData.collisionRadius;
        Debug.Log(transform.position);

        transform.forward    = m_traverseDirection;
        
        Physics.SyncTransforms();
    }


}