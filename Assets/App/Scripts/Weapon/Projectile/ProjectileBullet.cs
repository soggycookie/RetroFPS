using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ProjectileVisual))]
public abstract class ProjectileBullet : Bullet
{
    protected ProjectileBulletSO m_projectileData;
    protected Transform m_parent;
    protected CancellationTokenSource m_cancellationTokenSource;
    protected bool m_isShot;
    protected Vector3 m_traverseDirection;

    private void Awake()
    {
        base.Initialize();
        Initialize();
    }

    protected override void Initialize(){
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
        Manager.GetBulletFactory().Pool.Release(this);
        m_isShot = false;

        transform.position = Manager. GetCameraTransform().position;
    }

    Vector3 CalculateDirection()
    {
        if (m_projectileData.bulletSpreadOffset == 0)
        {
            return Manager. GetCameraTransform().forward;
        }

        Vector2 random = MathUtils.RandomInsideCircleWithBias(3f, m_projectileData.bulletSpreadOffset);

        return (Manager. GetCameraTransform().forward + random.x * Manager. GetCameraTransform().right + random.y * Manager. GetCameraTransform().up).normalized;
    }

    protected void SetShootingBullettParams()
    {
        //transform.parent    = null;

        m_traverseDirection  = CalculateDirection();
        transform.position   = Manager. GetCameraTransform().position + m_traverseDirection * m_projectileData.collisionRadius;

        transform.forward    = m_traverseDirection;
        
        Physics.SyncTransforms();
    }


}