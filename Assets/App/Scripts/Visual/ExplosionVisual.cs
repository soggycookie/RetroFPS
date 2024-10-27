using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplosionVisual : MonoBehaviour
{
    [SerializeField]
    private float m_timeToMaxExplosion;

    [SerializeField]
    private AnimationCurve m_animationCurve;


    private Explosion m_explosion;
    private Material  m_material;

    private float maxFadeOutTime;

    Coroutine lerpScale;
    Coroutine lerpFadeout;

    private void Awake()
    {
        m_explosion = GetComponent<Explosion>();
        m_material = GetComponentInChildren<MeshRenderer>().material;

        maxFadeOutTime = m_material.GetFloat("_MaxFadeOutTime");

    }

    public void Explode(float radius, Vector3 position){

        if(lerpScale != null)
            StopCoroutine(lerpScale);

        lerpScale = StartCoroutine(LerpScale( radius));
        
    }



    IEnumerator LerpScale(float radius)
    {
        float timer = 0;
        float scale = radius * 2f;
        m_material.SetFloat("_FadeOutTime", 0);
        m_material.SetFloat("_Alpha", 0.5f);

        while (true)
        {
            m_explosion.transform.localScale = Vector3.one * (timer / m_timeToMaxExplosion * scale);

            if (timer >= m_timeToMaxExplosion)
            {
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }


        if (lerpFadeout != null)
            StopCoroutine(lerpFadeout);

        lerpFadeout = StartCoroutine(LerpFadeOut());
    }

    IEnumerator LerpFadeOut()
    {
        yield return new WaitForSeconds(0.1f);
        float timer = 0;

        while (true)
        {
            m_material.SetFloat("_FadeOutTime", maxFadeOutTime * m_animationCurve.Evaluate(timer / maxFadeOutTime));

            if (timer >= maxFadeOutTime)
            {
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }

        GlobalExplosionFactory.Instance.Pool.Release(m_explosion);
    }
}
