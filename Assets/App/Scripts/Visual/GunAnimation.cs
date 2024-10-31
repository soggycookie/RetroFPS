using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class GunAnimation : MonoBehaviour
{
    [SerializeField]
    private Transform m_visualTransform;

    [SerializeField]
    GunAnimationSO    m_animationData;

    [SerializeField]
    private float duration;
    
    public float TotalDuration { get; set;}

    public UnityAction<Vector3, float, float, float> OnShooting;

    private Transform m_defaultRootTransform;
    Sequence shakeTween;

    private void Awake()
    {
        m_defaultRootTransform = m_visualTransform;
        m_animationData.Initialize(m_visualTransform, m_defaultRootTransform, duration);
    }

    public void Shoot(){
        if(shakeTween != null)
            shakeTween.Kill();

        //reset pos
        m_visualTransform = m_defaultRootTransform;
        m_animationData.AnimateShooting();

        OnShooting?.Invoke(m_animationData.pivotOffset, m_animationData.amplitude, m_animationData.frequency, m_animationData.shakeDuration);
    }
    public void Charge(){
        shakeTween = m_animationData.AnimateCharging();
    }

}
