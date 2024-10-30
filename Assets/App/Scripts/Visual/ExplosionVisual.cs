using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplosionVisual : MonoBehaviour
{
    [SerializeField]
    private float m_timeToMaxExplosion;

    [SerializeField]
    private MeshRenderer[] m_meshRenderers;

    [SerializeField]
    private float[] m_maxFadeOutTime;
    
    [SerializeField]
    private float[] m_defaultColorAlpha;

    private Explosion m_explosion;
    private Material[] m_materials;

    private void Awake()
    {
        m_explosion = GetComponent<Explosion>();

        m_materials = new Material[m_meshRenderers.Length];

        for(int i =0; i < m_meshRenderers.Length; i++){
            m_materials[i] = m_meshRenderers[i].material;
            m_materials[i].SetFloat("_MaxFadeOutTime", m_maxFadeOutTime[i]);
        }

    }

    void SetMaterialsColorAlpha(){
        for (int i = 0; i < m_materials.Length; i++)
        {
            Vector4 color = (Vector4)m_materials[i].color;
            color = new Vector4(color.x , color.y, color.z, m_defaultColorAlpha[i]);

            m_materials[i].color = color;
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < m_meshRenderers.Length; i++)
        {
            m_materials[i].SetFloat("_FadeOutTime", 0f);
        }

        SetMaterialsColorAlpha();
    }

    public void Explode(float radius, Vector3 position){

        float scale = radius * 2f;
        
        Sequence explosionSequence;
        explosionSequence = CreateExplosionAnimation(scale);

        explosionSequence.Play();

    }

    Sequence CreateExplosionAnimation(float scale)
    {
        Sequence explosionSequence = DOTween.Sequence();

        transform.localScale = Vector3.zero;

        explosionSequence.Append(transform.DOScale(scale, m_timeToMaxExplosion).SetEase(Ease.InOutQuint));
        explosionSequence.AppendInterval(0.1f);
        explosionSequence.Append(FadeOut());
        explosionSequence.Insert(0f, transform.DOScale(scale * 1.3f , m_timeToMaxExplosion).SetEase(Ease.InQuad));

        explosionSequence.OnComplete(
            () => {
                GlobalExplosionFactory.Instance.Pool.Release(m_explosion);
            });

        return explosionSequence;
    }

    Sequence FadeOut(){
        Sequence fadeOutSeq = DOTween.Sequence();

        for (int i = 0;i < m_materials.Length; i++){
            Tween fadeOut = m_materials[i].DOFade(0 , m_maxFadeOutTime[i]);
            fadeOutSeq.Insert(0, fadeOut);
        }

        return fadeOutSeq;
    }
}
