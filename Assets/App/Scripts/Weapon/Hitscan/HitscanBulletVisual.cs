using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitscanBulletVisual : MonoBehaviour
{
    private HitscanBullet m_bullet; 

    public UnityAction OnFinishFadeOut;


    [SerializeField]
    private Gradient        m_fadeOutGradient;

    [SerializeField]
    private float           m_fadeOutTime;

    [SerializeField]
    private Gradient        m_baseGradient;
    private AnimationCurve  m_baseCurve;

    private LineRenderer    m_lineRenderer;
    private float           m_currentTime;



    private void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        m_bullet = GetComponent<HitscanBullet>();


        m_baseGradient  = m_lineRenderer.colorGradient;
        m_baseCurve     = m_lineRenderer.widthCurve;

        m_bullet.OnShoot += SetLineRendererPositions;
    }

    private void Update()
    {
        m_currentTime += Time.deltaTime;

        if (m_currentTime >= m_fadeOutTime)
        {
            OnFinishFadeOut?.Invoke();
        }

        FadeOutWidth();
        FadeOutGradient();

    }

    private void OnDisable()
    {
        ResetParameters();
    }

    public void ResetParameters(){
        m_currentTime = 0;

        SetLineRendererParameters();

        //Debug.Log("reset");
    }


    private void FadeOutWidth(){
        AnimationCurve curve = new AnimationCurve(m_baseCurve.keys);
        Keyframe[] frames    = curve.keys; 

        for (int i = 0; i < frames.Length; i++) {
            frames[i].value = m_baseCurve.keys[i].value - (m_currentTime/m_fadeOutTime) * m_baseCurve.keys[i].value;

            curve.MoveKey(i, frames[i]);
        }
        

        m_lineRenderer.widthCurve = curve;
    }

    private void FadeOutGradient(){
        Gradient gradient = new Gradient();
        
        GradientColorKey[] colors = new GradientColorKey[m_baseGradient.colorKeys.Length];
        for(int i = 0;i < colors.Length;i++) {
            Vector4 colorDiff = (Vector4) m_fadeOutGradient.colorKeys[i].color - (Vector4) m_baseGradient.colorKeys[i].color;
            colors[i] = new GradientColorKey((Vector4) m_baseGradient.colorKeys[i].color + colorDiff * m_currentTime/m_fadeOutTime, m_baseGradient.colorKeys[i].time);
            
        }
        
        gradient.SetKeys(colors, m_baseGradient.alphaKeys);

        m_lineRenderer.colorGradient = gradient;
    }

    public void SetLineRendererParameters()
    {
        m_lineRenderer.colorGradient = m_baseGradient;
        m_lineRenderer.widthCurve    = m_baseCurve;
    }

    public void SetLineRendererPositions(Vector3[] positions)
    {
        m_lineRenderer.SetPositions(positions);
    }

}
