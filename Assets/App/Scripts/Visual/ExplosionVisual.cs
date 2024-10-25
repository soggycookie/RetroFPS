using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionVisual : MonoBehaviour
{
    [SerializeField]
    private float m_timeToMaxExplosion;


    private Explosion m_explosion;

    private void Awake()
    {
        m_explosion = GetComponent<Explosion>();
    }

    public void Explode(float radius, Vector3 position){
        transform.position = position;

        StartCoroutine(LerpScale( radius));
    }


    IEnumerator LerpScale( float radius)
    {
        float timer = 0;
        float scale = radius * 2f;

        while (true)
        {
            transform.localScale = Vector3.one * ( timer/ m_timeToMaxExplosion * scale);
            
            if(timer >= m_timeToMaxExplosion){
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }

    }


}
