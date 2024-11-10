
using UnityEngine;


[RequireComponent(typeof(Gun))]
public class BulletManager : MonoBehaviour 
{

    [SerializeField]
    private Transform m_playerCamera;
     
    [SerializeField]
    private Transform m_muzzlePoint;
   

    public float     CurrentCoolDownTime { get; protected set; }
    public float     BulletCoolDown      { get; protected set; }

    protected bool   m_canShoot;

    [SerializeField]
    protected BulletFactory m_BulletPoolFactory;

    [SerializeField]
    private Bullet m_bullet;
    
    [SerializeField]
    private ObstacleHitFactory m_particleFactory;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        BulletCoolDown = m_bullet.GetBulletData().bulletCoolDown;
    }

    private void Update()
    {
        ResetShootCooldown();
    }

    private void ResetShootCooldown()
    {
        if (!m_canShoot)
        {
            CurrentCoolDownTime += Time.deltaTime;

            if (CurrentCoolDownTime >= BulletCoolDown)
            {
                m_canShoot = true;
                CurrentCoolDownTime = 0;
            }
        }
    }

    public void Shoot(){

        if(!m_canShoot) return;

        for (int i = 0; i < m_bullet.GetBulletData().bulletsPerShot; i++)
        {
            Bullet bullet = m_BulletPoolFactory.Pool.Get();
            bullet.Shoot();
        }

        DisableShooting();
    }

    private void DisableShooting()
    {
        CurrentCoolDownTime = 0;
        m_canShoot  = false;
    }

    public bool CheckCanShoot()
    {
        return m_canShoot;
    }

    private void Initialize()
    {
        m_canShoot = true;

        m_BulletPoolFactory.CreatePool(m_bullet, this);
        m_particleFactory.CreatePool(m_bullet.GetBulletData().wallHitParticle);
    }
    
    public ObstacleHitFactory GetParticleFactory(){
        return m_particleFactory;
    }

    public BulletFactory GetBulletFactory(){
        return m_BulletPoolFactory;
    }

    public Transform GetCameraTransform(){
        return m_playerCamera;
    }

    public Vector3 GetMuzzlePoint(){
        return m_muzzlePoint.position;
    }
}
