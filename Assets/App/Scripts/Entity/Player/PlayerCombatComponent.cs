using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerCombatComponent : MonoBehaviour
{
    private GunController         m_gunController;
    private PlayerMovementAbility m_playerMovementAbility;
    
    [SerializeField] 
    private float m_directSlamForce;

    [SerializeField]
    private float m_directSlamDamage;

    [SerializeField]
    private float m_indirectSlamForce;

    [SerializeField]
    private float m_indirectSlamDamage;



    private void Awake()
    {
        m_playerMovementAbility = GetComponent<PlayerMovementAbility>();
        m_gunController         = GetComponent<GunController>();

        m_playerMovementAbility.OnSlamDirectHit   += Slam;
        m_playerMovementAbility.OnSlamIndirectHit += SlamIndirect;
    }

    private void Slam(RaycastHit hit){
        EnemyBehavior enemyBehavior = hit.transform.GetComponent<EnemyBehavior>();

        if(enemyBehavior.GetIsPushable()){
            Vector3 dir = enemyBehavior.transform.position - gameObject.transform.position;
            dir = new Vector3(dir.x, 0, dir.z);
           

            enemyBehavior.GetSlammed(Vector3.up + dir, m_directSlamForce);
        }
    }

    private void SlamIndirect(RaycastHit hit)
    {
        EnemyBehavior enemyBehavior = hit.transform.GetComponent<EnemyBehavior>();
        
        
        if (enemyBehavior.GetIsPushable())
        {
            enemyBehavior.GetPropeled(Vector3.up, m_indirectSlamForce);
        }
    }


}