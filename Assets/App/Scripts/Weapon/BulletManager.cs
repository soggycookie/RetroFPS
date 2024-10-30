
using UnityEngine;


[RequireComponent(typeof(Gun))]
public class BulletManager : MonoBehaviour 
{

    public Transform FPSCamera         { get; set; }
    public Transform MuzzlePoint       { get; set; }
   

    public float     CurrentCoolDownTime { get; protected set; }
    public float     BulletCoolDown      { get; protected set; }

    protected bool   m_canShoot;

    protected int    m_defaultPoolingCapacity;
    protected int    m_maxPoolingCapacity;
    protected bool   m_hasCollectionCheck;

    public Bullet    BulletPrefab { get; set; }

    //public ParticleSystemFactory f_ParticleSystem { get; private set; }
    public BulletFactory f_bullet { get; private set; }

    public BulletSO BulletData { get; set;}

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        InstantiateBulletFactory();
        BulletCoolDown = BulletData.bulletCoolDown;
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

        for (int i = 0; i < BulletData.bulletsPerShot; i++)
        {
            Bullet bullet = f_bullet.Pool.Get();
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
    }

    public void InitializePoolParameters(bool collisionCheck, int defaultCapacity, int maxCapacity)
    {
        m_defaultPoolingCapacity = defaultCapacity;
        m_maxPoolingCapacity     = maxCapacity;
        m_hasCollectionCheck     = collisionCheck;
    }


    private void InstantiateBulletFactory(){
        f_bullet = gameObject.AddComponent<BulletFactory>();

        f_bullet.CreatePool(m_hasCollectionCheck, m_defaultPoolingCapacity, m_maxPoolingCapacity, BulletPrefab, this);
    }

}
