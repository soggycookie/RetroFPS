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

    [SerializeField]
    private MeshRenderer[] m_meshRenderers;

    private Explosion m_explosion;


    private float[] m_maxFadeOutTime;
    private Material[] m_materials;

    Coroutine lerpScale;
    Coroutine lerpFadeout;

    private void Awake()
    {
        m_explosion = GetComponent<Explosion>();

        m_materials = new Material[m_meshRenderers.Length];
        m_maxFadeOutTime = new float[m_meshRenderers.Length];

        for(int i =0; i < m_meshRenderers.Length; i++){
            m_materials[i] = m_meshRenderers[i].material;
            m_maxFadeOutTime[i] = m_materials[i].GetFloat("_MaxFadeOutTime");
        }

    }

    private void OnEnable()
    {
        for (int i = 0; i < m_meshRenderers.Length; i++)
        {
            m_materials[i].SetFloat("_FadeOutTime", 0f);
        }
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
            m_explosion.transform.localScale +=  (timer / m_maxFadeOutTime[2]) * Vector3.one * 0.1f;

            for (int i = 0; i < m_meshRenderers.Length; i++)
            {
                m_materials[i].SetFloat("_FadeOutTime", m_maxFadeOutTime[i] * m_animationCurve.Evaluate(timer / m_maxFadeOutTime[i]));

            }

            if (timer >= m_maxFadeOutTime[2])
            {
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }

        GlobalExplosionFactory.Instance.Pool.Release(m_explosion);
    }
}
