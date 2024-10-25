using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private bool      m_isPushable;
    private Rigidbody m_rigidbody;
    private Collider  m_collider;

    private bool m_isGrounded;

    private void Awake()
    {
        Physics.gravity = new Vector3(0, -31f, 0);

        m_rigidbody     = GetComponent<Rigidbody>();
                        
        m_collider      = GetComponent<Collider>();

        m_isPushable    = true;
    }

    private void Update()
    {
        CheckGround();
    }

    void CheckGround()
    {

        bool rayCheck = Physics.Raycast(transform.position + Vector3.up, Vector3.down, 0.1f + Vector3.up.magnitude, 1 << 3 | 1 << 6);

        if (rayCheck)
        {
            m_isGrounded = true;

        }else{
            m_isGrounded = false;
        }

    }

    public bool GetIsPushable(){
        return m_isPushable;
    }

    public void GetPropeled(Vector3 direction, float force){
        m_rigidbody.AddForce(direction * force, ForceMode.VelocityChange);
    }

    public void GetSlammed(Vector3 direction, float force)
    {
        if(m_isGrounded) 
            m_rigidbody.AddForce(direction * force, ForceMode.Impulse);

        TurnTriggeredCollider(true);
        StartCoroutine(TurnTriggerColliderOff());
    }

    private void TurnTriggeredCollider(bool status){
        m_rigidbody.useGravity = !status;
        m_collider.isTrigger   =  status;
    }

    IEnumerator TurnTriggerColliderOff(){
        yield return new WaitForSeconds(0.5f);

        TurnTriggeredCollider(false);
    }


}
